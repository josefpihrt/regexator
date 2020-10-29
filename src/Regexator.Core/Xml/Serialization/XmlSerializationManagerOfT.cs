// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization
{
    public sealed class XmlSerializationManager<T>
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));

        public T Deserialize(string filePath)
        {
            return XmlSerializationManager.Deserialize<T>(filePath);
        }

        public T Deserialize(string filePath, string defaultContent)
        {
            return XmlSerializationManager.Deserialize<T>(filePath, defaultContent);
        }

        public T Deserialize(string filePath, Encoding encoding)
        {
            return XmlSerializationManager.Deserialize<T>(filePath, encoding);
        }

        public T Deserialize(string filePath, Encoding encoding, string defaultContent)
        {
            return XmlSerializationManager.Deserialize<T>(filePath, encoding, defaultContent, _serializer);
        }

        public void Serialize(string filePath, T item)
        {
            XmlSerializationManager.Serialize<T>(filePath, item);
        }

        public void Serialize(string filePath, Encoding encoding, T item)
        {
            XmlSerializationManager.Serialize<T>(filePath, encoding, item);
        }

        public void Serialize(string filePath, T item, T oldItem)
        {
            XmlSerializationManager.Serialize<T>(filePath, item, oldItem);
        }

        public void Serialize(string filePath, Encoding encoding, T item, T oldItem)
        {
            XmlSerializationManager.Serialize(filePath, encoding, item, oldItem, _serializer);
        }

        public string Serialize(T item)
        {
            return XmlSerializationManager.Serialize(item, _serializer);
        }

        public T DeserializeText(string input)
        {
            return XmlSerializationManager.DeserializeText<T>(input, _serializer);
        }
    }
}
