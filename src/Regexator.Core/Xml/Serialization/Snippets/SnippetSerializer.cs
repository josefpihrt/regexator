// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Regexator.FileSystem;
using Regexator.Snippets;

namespace Regexator.Xml.Serialization.Snippets
{
    public static class SnippetSerializer
    {
        public static IEnumerable<SnippetInfo> Deserialize(DirectoryInfo root, SearchOption option)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            return IO.FileSystem.EnumerateFileInfos(root, Regexator.Snippets.RegexSnippet.SnippetSearchPattern, option)
                .Select(f => f.FullName)
                .SelectMany(f => Deserialize(f));
        }

        public static IEnumerable<SnippetInfo> Deserialize(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            return Deserialize2();

            IEnumerable<SnippetInfo> Deserialize2()
            {
                Exception exception = null;
                RegexSnippetsElement element = null;
                try
                {
                    element = DeserializeElement(filePath);
                }
                catch (Exception ex)
                {
                    exception = ex;

                    if (!(ex is InvalidOperationException) && !Executor.IsFileSystemException(ex))
                        throw;
                }

                if (element != null)
                {
                    foreach (SnippetInfo item in LoadFromElement(element, filePath))
                        yield return item;
                }
                else
                {
                    yield return new SnippetInfo(new SnippetErrorInfo(filePath, exception));
                }
            }
        }

        public static IEnumerable<SnippetInfo> DeserializeString(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return DeserializeString2();

            IEnumerable<SnippetInfo> DeserializeString2()
            {
                RegexSnippetsElement element = null;
                SnippetInfo errorInfo = null;
                try
                {
                    element = DeserializeElementFromString(value);
                }
                catch (Exception ex) when (ex is InvalidOperationException)
                {
                    errorInfo = new SnippetInfo(new SnippetErrorInfo("", ex));
                }

                if (errorInfo != null)
                {
                    yield return errorInfo;
                }
                else if (element != null)
                {
                    foreach (SnippetInfo item in LoadFromElement(element, ""))
                        yield return item;
                }
            }
        }

        private static IEnumerable<SnippetInfo> LoadFromElement(RegexSnippetsElement element, string filePath)
        {
            foreach (RegexSnippet snippet in element.Snippets)
            {
                SnippetInfo info;
                try
                {
                    info = new SnippetInfo(filePath, RegexSnippet.FromSerializable(snippet));
                }
                catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
                {
                    info = new SnippetInfo(new SnippetErrorInfo(filePath, ex));
                }

                yield return info;
            }
        }

        public static RegexSnippetsElement DeserializeElementFromString(string value)
        {
            StringReader input = null;
            try
            {
                input = new StringReader(value);
                using (XmlReader xmlReader = XmlReader.Create(input, _readerSettings))
                {
                    input = null;
                    return (RegexSnippetsElement)_serializer.Deserialize(xmlReader);
                }
            }
            finally
            {
                input?.Dispose();
            }
        }

        public static RegexSnippetsElement DeserializeElement(string filePath)
        {
            StreamReader input = null;
            try
            {
                input = new StreamReader(filePath, _encoding);
                using (XmlReader xmlReader = XmlReader.Create(input, _readerSettings))
                {
                    input = null;
                    return (RegexSnippetsElement)_serializer.Deserialize(xmlReader);
                }
            }
            finally
            {
                input?.Dispose();
            }
        }

        public static void Serialize(string dirPath, Regexator.Snippets.RegexSnippet[] snippets)
        {
            Serialize(dirPath, snippets, false);
        }

        public static void Serialize(string dirPath, Regexator.Snippets.RegexSnippet[] snippets, bool singleFile)
        {
            if (dirPath == null)
                throw new ArgumentNullException(nameof(dirPath));

            if (snippets == null)
                throw new ArgumentNullException(nameof(snippets));

            if (singleFile)
            {
                Directory.CreateDirectory(dirPath);
                string filePath = Path.Combine(
                    dirPath,
                    Path.ChangeExtension(Guid.NewGuid().ToString(), Regexator.Snippets.RegexSnippet.FileExtension));
                var element = new RegexSnippetsElement(snippets.Select(f => RegexSnippet.ToSerializable(f)).ToArray());
                Serialize(filePath, element);
            }
            else
            {
                foreach (Regexator.Snippets.RegexSnippet snippet in snippets)
                {
                    string snippetDirPath = Path.Combine(dirPath, snippet.Category ?? "");
                    Directory.CreateDirectory(snippetDirPath);
                    string filePath = Path.Combine(
                        snippetDirPath,
                        Path.ChangeExtension(snippet.Name, Regexator.Snippets.RegexSnippet.FileExtension));
                    var element = new RegexSnippetsElement(RegexSnippet.ToSerializable(snippet));
                    Serialize(filePath, element);
                }
            }
        }

        private static void Serialize(string filePath, RegexSnippetsElement element)
        {
            StreamWriter output = null;
            try
            {
                output = new StreamWriter(filePath, false, _encoding);
                using (XmlWriter xmlWriter = XmlWriter.Create(output, _writerSettings))
                {
                    output = null;
                    _serializer.Serialize(xmlWriter, element);
                }
            }
            finally
            {
                output?.Dispose();
            }
        }

        private static XmlSerializer CreateSerializer()
        {
#if DEBUG
            var serializer = new XmlSerializer(typeof(RegexSnippetsElement));
            serializer.UnknownElement += (object sender, XmlElementEventArgs e) => Debugger.Break();
            serializer.UnknownAttribute += (object sender, XmlAttributeEventArgs e) => Debugger.Break();
            serializer.UnknownNode += (object sender, XmlNodeEventArgs e) => Debugger.Break();
            serializer.UnreferencedObject += (object sender, UnreferencedObjectEventArgs e) => Debugger.Break();
            return serializer;
#else
            return new XmlSerializer(typeof(RegexSnippetsElement));
#endif
        }

        private static readonly XmlSerializer _serializer = CreateSerializer();
        private static readonly Encoding _encoding = Encoding.UTF8;

        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            IndentChars = "    ",
            Indent = true,
            NewLineOnAttributes = false
        };

        private static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings()
        {
            CloseInput = true
        };
    }
}
