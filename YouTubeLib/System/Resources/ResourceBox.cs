﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources.Converter;
using System.Text;

namespace System.Resources
{
    public static class ResourceBox
    {
        internal static Dictionary<Type, IResourceConverter> converters =
            new Dictionary<Type, IResourceConverter>()
            {
                { typeof(byte[]), new BytesConverter() },
                { typeof(string), new StringConverter() },
                { typeof(Bitmap), new BitmapConverter() }
            };

        private static string appName;
        private static Assembly executedAssembly;
        private static string[] resourceNames;

        static ResourceBox()
        {
            executedAssembly = Assembly.GetExecutingAssembly();
            resourceNames = executedAssembly.GetManifestResourceNames();
            appName = executedAssembly.GetName().Name.Replace(" ", "_");
        }

        public static void AddResourceConverter<TConverter>(bool typeOverride = false) where TConverter : IResourceConverter
        {
            TConverter converter = Activator.CreateInstance<TConverter>();

            var baseType = converter.GetType().BaseType;

            if (baseType.GetGenericArguments().Count() == 0)
                throw new ArgumentException();

            Type type = baseType.GetGenericArguments()[0];

            if ((converters.ContainsKey(type) && typeOverride) || !converters.ContainsKey(type))
            {
                converters[type] = converter;
            }
        }

        private static T ConvertTo<T>(Stream s)
        {
            return (T)converters[typeof(T)].ToResource(s);
        }

        private static bool CanConvert<T>()
        {
            return converters.ContainsKey(typeof(T));
        }

        public static T FindResource<T>(string name)
        {
            if (!CanConvert<T>())
                throw new Exception();

            name = $"{appName}.{name}";

            var query = resourceNames.Where(r => r.AnyEquals(name));

            if (query.Count() > 0)
            {
                return ConvertTo<T>(executedAssembly.GetManifestResourceStream(query.First()));
            }

            return default(T);
        }
    }
}