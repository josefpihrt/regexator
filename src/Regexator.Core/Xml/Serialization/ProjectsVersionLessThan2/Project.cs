// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan2
{
    public class Project
    {
        internal static Project ToSerializable(Regexator.Project item, string filePath)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var project = new Project()
            {
                Mode = item.Container.Mode,
                Attributes = (item.Container.Attributes != ItemAttributes.None)
                    ? item.Container.Attributes.ToString()
                    : null,
                ProjectInfo = ProjectInfo.ToSerializable(item.Container.ProjectInfo),
                Pattern = Pattern.ToSerializable(item.Container.Pattern),
                Replacement = Replacement.ToSerializable(item.Container.Replacement),
                OutputInfo = OutputInfo.ToSerializable(item.Container.OutputInfo),
                Inputs = Input.ToSerializable(item.Inputs, filePath, InputKind.Included).ToArray(),
                FileInputs = Input.ToSerializable(item.FileInputs, filePath).ToArray()
            };
            if (item.Inputs.Count > 0 || item.FileInputs.Count > 0)
            {
                switch (item.InputNameKind)
                {
                    case InputKind.File:
                        {
                            project.InputName = (!string.IsNullOrEmpty(item.InputName)
                                && (FileSystem.FileSystemUtility
                                    .TryCreateRelativePath(filePath, item.InputName, out string result)))
                                ? result
                                : item.InputName;
                            break;
                        }
                    case InputKind.Included:
                        {
                            project.InputName = item.InputName.Enclose("|");
                            break;
                        }
                }
            }

            return project;
        }

        internal static Regexator.Project FromSerializable(Project item, string filePath)
        {
            var container = new Regexator.ProjectContainer();

            if (item.ProjectInfo != null)
                container.ProjectInfo = ProjectInfo.FromSerializable(item.ProjectInfo);

            if (item.Pattern != null)
                container.Pattern = Pattern.FromSerializable(item.Pattern);

            if (item.Replacement != null)
                container.Replacement = Replacement.FromSerializable(item.Replacement);

            if (item.OutputInfo != null)
                container.OutputInfo = OutputInfo.FromSerializable(item.OutputInfo);

            container.Mode = item.Mode;
            container.Attributes = item.ParseAttributes();
            var project = new Regexator.Project(container);
            string dirPath = Path.GetDirectoryName(filePath);

            if (item.Inputs != null)
                project.Inputs.AddRange(Input.FromSerializable(item.Inputs, dirPath, InputKind.Included));

            if (item.FileInputs != null)
                project.FileInputs.AddRange(Input.FromSerializable(item.FileInputs, dirPath));

            if (!string.IsNullOrEmpty(item.InputName))
            {
                Match match = _inputPathRegex.Match(item.InputName);
                if (match.Success)
                {
                    project.InputName = match.Value;
                    project.InputNameKind = InputKind.Included;
                }
                else
                {
                    project.InputName = (FileSystem.FileSystemUtility
                        .TryCreateAbsolutePath(dirPath, item.InputName, out string result))
                        ? result
                        : item.InputName;
                    project.InputNameKind = InputKind.File;
                }
            }

            return project;
        }

        public ItemAttributes ParseAttributes()
        {
            if (Enum.TryParse(Attributes, out ItemAttributes value))
                return value;

            return EnumHelper.ParseAttributes(Attributes);
        }

        [XmlAttribute("AppVersion")]
        public string ApplicationVersion { get; set; }

        public string Attributes { get; set; }

        [XmlElement("Header")]
        public ProjectInfo ProjectInfo { get; set; }

        [XmlAttribute]
        public EvaluationMode Mode { get; set; }

        public Pattern Pattern { get; set; }
        public Replacement Replacement { get; set; }

        [XmlElement("Output")]
        public OutputInfo OutputInfo { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Input")]
        public Input[] Inputs { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("FileInput")]
        public Input[] FileInputs { get; set; }

        public string InputName { get; set; }

        private static readonly Regex _inputPathRegex = new Regex(@"(?<=^\|).*(?=\|$)");
    }
}
