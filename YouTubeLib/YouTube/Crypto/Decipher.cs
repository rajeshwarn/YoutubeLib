using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using YouTubeLib.Utilitiy;

namespace YouTubeLib
{
    internal static class Decipher
    {
        public static async Task<string> Decrypt(string cipher, string jsUrl)
        {
            string js;

            if (NetCache.HasCache(jsUrl))
            {
                js = NetCache.Get(jsUrl);
            }
            else
            {
                js = await (new HttpHelper()).GET(jsUrl);
                NetCache.Set(jsUrl, js);
            }

            string functNamePattern = @"\.sig\s*\|\|([a-zA-Z0-9\$]+)\(";

            var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;

            if (funcName.Contains("$"))
            {
                funcName = "\\" + funcName;
            }

            string funcPattern = @"(?!h\.)" + @funcName + @"=function\(\w+\)\{.*?\}";
            var funcBody = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value;
            var lines = funcBody.Split(';');

            string idReverse = "", idSlice = "", idCharSwap = "";
            string functionIdentifier = "";
            string operations = "";

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break;
                }

                functionIdentifier = GetFunctionFromLine(line);
                string reReverse = string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier);
                string reSlice = string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.", functionIdentifier);
                string reSwap = string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b", functionIdentifier);

                if (Regex.Match(js, reReverse).Success)
                {
                    idReverse = functionIdentifier;
                }

                if (Regex.Match(js, reSlice).Success)
                {
                    idSlice = functionIdentifier;
                }

                if (Regex.Match(js, reSwap).Success)
                {
                    idCharSwap = functionIdentifier;
                }
            }

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " ";
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " ";
                }

                if (functionIdentifier == idReverse)
                {
                    operations += "r ";
                }
            }

            operations = operations.Trim();

            return DecipherWithOperations(cipher, operations);
        }

        private static string ApplyOperation(string cipher, string op)
        {
            switch (op[0])
            {
                case 'r':
                    return new string(cipher.ToCharArray().Reverse().ToArray());

                case 'w':
                    {
                        int index = GetOpIndex(op);
                        return SwapFirstChar(cipher, index);
                    }

                case 's':
                    {
                        int index = GetOpIndex(op);
                        return cipher.Substring(index);
                    }

                default:
                    throw new NotImplementedException("Couldn't find cipher operation.");
            }
        }

        private static string DecipherWithOperations(string cipher, string operations)
        {
            return operations.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Aggregate(cipher, ApplyOperation);
        }

        private static string GetFunctionFromLine(string currentLine)
        {
            Regex matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\(");
            Match rgMatch = matchFunctionReg.Match(currentLine);
            return rgMatch.Groups["functionID"].Value;
        }

        private static int GetOpIndex(string op)
        {
            string parsed = new Regex(@".(\d+)").Match(op).Result("$1");
            return int.Parse(parsed);
        }

        private static string SwapFirstChar(string cipher, int index)
        {
            var builder = new StringBuilder(cipher);
            builder[0] = cipher[index];
            builder[index] = cipher[0];

            return builder.ToString();
        }
    }
}
