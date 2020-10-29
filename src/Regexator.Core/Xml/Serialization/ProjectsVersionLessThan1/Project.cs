// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
{
    [XmlRoot("Project")]
    public class Project
    {
        internal static Regexator.Project FromSerializable(Project item, string filePath)
        {
            var project = new Regexator.Project(ProjectContainer.FromSerializable(item.Pattern));
            string dirPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(item.InputPath))
            {
                Match match = _inputPathRegex.Match(item.InputPath);
                if (match.Success)
                {
                    project.InputName = match.Value;
                    project.InputNameKind = InputKind.Included;
                }
                else
                {
                    project.InputName = (FileSystem.FileSystemUtility
                        .TryCreateAbsolutePath(dirPath, item.InputPath, out string result))
                        ? result
                        : item.InputPath;
                    project.InputNameKind = InputKind.File;
                }
            }

            if (item.Inputs != null)
                project.FileInputs.AddRange(Input.FromSerializable(item.Inputs, dirPath));

            if (item.IncludedInputs != null)
                project.Inputs.AddRange(Input.FromSerializable(item.IncludedInputs, dirPath, InputKind.Included));

            return project;
        }

        [XmlAttribute("AppVersion")]
        public string ApplicationVersion { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1041:ProvideObsoleteAttributeMessage")]
        [Obsolete]
        public string FilePath { get; set; }

        public ProjectContainer Pattern { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Input")]
        public Input[] Inputs { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Input")]
        public Input[] IncludedInputs { get; set; }

        public string InputPath { get; set; }

        private static readonly Regex _inputPathRegex = new Regex("(?<=^%).*(?=%$)");
    }
}
