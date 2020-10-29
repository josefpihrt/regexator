// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization
{
    public static class XmlSerializationManager
    {
        private static readonly Encoding _defaultEncoding = new UTF8Encoding(true);

        private static readonly XmlWriterSettings _defaultSettings = new XmlWriterSettings()
        {
            IndentChars = "    ",
            Indent = true,
            CloseOutput = true
        };

        public static TResult Deserialize<TResult>(string filePath)
        {
            return Deserialize<TResult>(filePath, _defaultEncoding);
        }

        public static TResult Deserialize<TResult>(string filePath, string defaultContent)
        {
            return Deserialize<TResult>(filePath, _defaultEncoding, defaultContent);
        }

        public static TResult Deserialize<TResult>(string filePath, Encoding encoding)
        {
            return Deserialize<TResult>(filePath, encoding, null);
        }

        public static TResult Deserialize<TResult>(string filePath, Encoding encoding, string defaultContent)
        {
            return Deserialize<TResult>(filePath, encoding, defaultContent, new XmlSerializer(typeof(TResult)));
        }

        internal static TResult Deserialize<TResult>(
            string filePath,
            Encoding encoding,
            string defaultContent,
            XmlSerializer serializer)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (defaultContent != null && !File.Exists(filePath))
            {
                using (var sw = new StreamWriter(filePath, false, encoding))
                    sw.Write(defaultContent);
            }

            using (var sr = new StreamReader(filePath, encoding))
                return (TResult)serializer.Deserialize(sr);
        }

        public static TResult DeserializeText<TResult>(string input)
        {
            return DeserializeText<TResult>(input, new XmlSerializer(typeof(TResult)));
        }

        internal static TResult DeserializeText<TResult>(string input, XmlSerializer serializer)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            using (var sr = new StringReader(input))
                return (TResult)serializer.Deserialize(sr);
        }

        public static void Serialize<TResult>(string filePath, TResult item)
        {
            Serialize<TResult>(filePath, item, default(TResult));
        }

        public static void Serialize<TResult>(string filePath, TResult item, TResult oldItem)
        {
            Serialize<TResult>(filePath, _defaultEncoding, item, oldItem);
        }

        public static void Serialize<TResult>(string filePath, Encoding encoding, TResult item)
        {
            Serialize<TResult>(filePath, encoding, item, default(TResult));
        }

        public static void Serialize<TResult>(string filePath, Encoding encoding, TResult item, TResult oldItem)
        {
            Serialize<TResult>(filePath, encoding, item, oldItem, new XmlSerializer(typeof(TResult)));
        }

        internal static void Serialize<TResult>(
            string filePath,
            Encoding encoding,
            TResult item,
            TResult oldItem,
            XmlSerializer serializer)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (oldItem == null || !SerializedEquals(item, oldItem, serializer))
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(filePath, false, encoding);
                    using (XmlWriter xw = XmlWriter.Create(sw, _defaultSettings))
                    {
                        sw = null;
                        serializer.Serialize(xw, item);
                    }
                }
                finally
                {
                    sw?.Dispose();
                }
            }
        }

        private static bool SerializedEquals<T>(T first, T second, XmlSerializer serializer)
        {
            using (var ms = new MemoryStream())
            {
                using (var ms2 = new MemoryStream())
                {
                    serializer.Serialize(ms, first);
                    serializer.Serialize(ms2, second);
                    return ms.ToArray().SequenceEqual(ms2.ToArray());
                }
            }
        }

        public static string Serialize<TResult>(TResult item)
        {
            return Serialize<TResult>(item, new XmlSerializer(typeof(TResult)));
        }

        internal static string Serialize<TResult>(TResult item, XmlSerializer serializer)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb, CultureInfo.CurrentCulture))
                serializer.Serialize(sw, item);

            return sb.ToString();
        }
    }
}
