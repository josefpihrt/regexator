// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Regexator.FileSystem;

namespace Regexator.Xml.Serialization.Projects
{
    public class ProjectSerializer
    {
        private static XmlSerializer _xmlSerializerVersionLessThan1;
        private static XmlSerializer _xmlSerializerVersionLessThan2;
        private static XmlSerializer _xmlSerializer;
        private static ProjectSerializer _projectSerializer;
        private static readonly Encoding _encoding = Encoding.UTF8;

        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            IndentChars = "    ",
            Indent = true
        };

        private static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings()
        {
            CloseInput = true,
            IgnoreWhitespace = true
        };

        public event EventHandler<ProjectSerializationCancelEventArgs> Serializing;
        public event EventHandler<ProjectSerializationEventArgs> Serialized;
        public event EventHandler<ProjectSerializationEventArgs> Deserialized;

        public ProjectSerializer()
        {
            InitialXmlComment = " Do not modify the contents of this file by hand. ";
        }

        public void Serialize(string filePath, Regexator.Project project)
        {
            using (FileStream stream = FileConnection.Open(filePath))
            {
                Serialize(stream, project);
            }
        }

        public void Serialize(FileStream stream, Regexator.Project project)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (project == null)
                throw new ArgumentNullException(nameof(project));

            string filePath = stream.Name;
            var e = new ProjectSerializationCancelEventArgs(project, filePath);
            OnSerializing(e);
            if (!e.Cancel)
            {
                var projects = new ProjectsElement(Project.ToSerializable(project, filePath));
                if (e.ApplicationVersion != null)
                    projects.ApplicationVersion = e.ApplicationVersion.ToString(2);

                SerializeProjects(stream, projects, InitialXmlComment);
                project.FilePath = filePath;
                OnSerialized(new ProjectSerializationEventArgs(project, filePath));
            }
        }

        private static void SerializeProjects(FileStream stream, ProjectsElement projects, string initialXmlComment)
        {
            StreamWriter output = null;
            try
            {
                var ms = new MemoryStream();
                output = new StreamWriter(ms, _encoding);
                using (XmlWriter xmlWriter = XmlWriter.Create(output, _writerSettings))
                {
                    output = null;
                    xmlWriter.WriteStartDocument();

                    if (!string.IsNullOrEmpty(initialXmlComment))
                        xmlWriter.WriteComment(initialXmlComment);

                    XmlSerializer.Serialize(xmlWriter, projects);
                    stream.SetLength(0);
                    ms.WriteTo(stream);
                }
            }
            finally
            {
                output?.Dispose();
            }
        }

        public Regexator.Project Deserialize(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            Regexator.Project project = DeserializeProject(filePath);
            OnDeserialized(new ProjectSerializationEventArgs(project, filePath));
            return project;
        }

        private static Regexator.Project DeserializeProject(string filePath)
        {
            StreamReader input = null;

            try
            {
                input = new StreamReader(filePath, _encoding);

                using (XmlReader xmlReader = XmlReader.Create(input, _readerSettings))
                {
                    input = null;

                    return DeserializeProject(xmlReader, filePath, GetAppVersion(xmlReader));
                }
            }
            finally
            {
                input?.Dispose();
            }
        }

        private static Regexator.Project DeserializeProject(XmlReader xmlReader, string filePath, Version version)
        {
            if (version != null)
            {
                if (version.Major < 1)
                    return DeserializeProjectWhenAppVersionLessThan1(xmlReader, filePath);

                if (version.Major < 2)
                    return DeserializeProjectWhenAppVersionLessThan2(xmlReader, filePath);
            }

            return DeserializeProject(xmlReader, filePath);
        }

        private static Regexator.Project DeserializeProject(XmlReader reader, string filePath)
        {
            var projects = (ProjectsElement)XmlSerializer.Deserialize(reader);
            Project item = projects.FirstProject;

            if (item == null)
                throw new InvalidOperationException(Resources.ProjectFileContainsNoProjectElementMsg);

            Regexator.Project project = Project.FromSerializable(item, filePath);
            project.FilePath = filePath;
            return project;
        }

        private static Regexator.Project DeserializeProjectWhenAppVersionLessThan2(XmlReader reader, string filePath)
        {
            var projects = (Projects.VersionLessThan2.ProjectsElement)XmlSerializerVersionLessThan2.Deserialize(reader);
            VersionLessThan2.Project item = projects.FirstProject;

            if (item == null)
                throw new InvalidOperationException(Resources.ProjectFileContainsNoProjectElementMsg);

            Regexator.Project project = VersionLessThan2.Project.FromSerializable(item, filePath);
            project.FilePath = filePath;
            return project;
        }

        private static Regexator.Project DeserializeProjectWhenAppVersionLessThan1(XmlReader reader, string filePath)
        {
            var projects = (Projects.VersionLessThan1.ProjectsElement)XmlSerializerVersionLessThan1.Deserialize(reader);
            VersionLessThan1.Project item = projects.FirstProject;

            if (item == null)
                throw new InvalidOperationException(Resources.ProjectFileContainsNoProjectElementMsg);

            Regexator.Project project = VersionLessThan1.Project.FromSerializable(item, filePath);
            project.FilePath = filePath;
            return project;
        }

        private static Version GetAppVersion(XmlReader xmlReader)
        {
            if (xmlReader.Read()
                && xmlReader.NodeType == XmlNodeType.XmlDeclaration
                && xmlReader.Read()
                && (xmlReader.NodeType != XmlNodeType.Comment || xmlReader.Read())
                && xmlReader.NodeType == XmlNodeType.Element
                && xmlReader.Name == "Projects")
            {
                string value = xmlReader.GetAttribute("AppVersion");

                if (value != null && Version.TryParse(value, out Version version))
                {
                    return version;
                }
            }

            return null;
        }

        protected virtual void OnSerializing(ProjectSerializationCancelEventArgs e)
        {
            Serializing?.Invoke(this, e);
        }

        protected virtual void OnSerialized(ProjectSerializationEventArgs e)
        {
            Serialized?.Invoke(this, e);
        }

        protected virtual void OnDeserialized(ProjectSerializationEventArgs e)
        {
            Deserialized?.Invoke(this, e);
        }

        [Conditional("DEBUG")]
        private static void SubscribeEvents(XmlSerializer serializer)
        {
            serializer.UnknownElement += (object sender, XmlElementEventArgs e) => Debug.Fail("");
            serializer.UnknownAttribute += (object sender, XmlAttributeEventArgs e) => Debug.Fail("");
            serializer.UnknownNode += (object sender, XmlNodeEventArgs e) => Debug.Fail("");
            serializer.UnreferencedObject += (object sender, UnreferencedObjectEventArgs e) => Debug.Fail("");
        }

        public static ProjectSerializer Default
        {
            get { return _projectSerializer ?? (_projectSerializer = new ProjectSerializer()); }
        }

        private static XmlSerializer XmlSerializer
        {
            get
            {
                if (_xmlSerializer == null)
                {
                    _xmlSerializer = new XmlSerializer(typeof(ProjectsElement));
                    SubscribeEvents(_xmlSerializer);
                }

                return _xmlSerializer;
            }
        }

        private static XmlSerializer XmlSerializerVersionLessThan2
        {
            get
            {
                if (_xmlSerializerVersionLessThan2 == null)
                {
                    _xmlSerializerVersionLessThan2 = new XmlSerializer(typeof(VersionLessThan2.ProjectsElement));
                    SubscribeEvents(_xmlSerializerVersionLessThan2);
                }

                return _xmlSerializerVersionLessThan2;
            }
        }

        private static XmlSerializer XmlSerializerVersionLessThan1
        {
            get
            {
                if (_xmlSerializerVersionLessThan1 == null)
                {
                    _xmlSerializerVersionLessThan1 = new XmlSerializer(typeof(VersionLessThan1.ProjectsElement));
                    SubscribeEvents(_xmlSerializerVersionLessThan1);
                }

                return _xmlSerializerVersionLessThan1;
            }
        }

        public string InitialXmlComment { get; set; }
    }
}
