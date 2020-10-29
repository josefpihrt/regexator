// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Text;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
{
    [XmlRoot("Pattern")]
    public class ProjectContainer
    {
        public static ProjectContainer ToSerializable(Regexator.ProjectContainer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new ProjectContainer()
            {
                ProjectInfo = ProjectInfo.ToSerializable(item.ProjectInfo),
                Mode = item.Mode,
                RegexOptions = item.Pattern.RegexOptions.ToString(),
                Groups = GroupSettings.ToSerializable(item.OutputInfo.Groups),
                Text = XmlUtility.EncodeCDataEnd(item.Pattern.Text).Enclose("\n"),
                Replacement = Replacement.ToSerializable(item.Replacement),
                PatternOptions = item.Pattern.PatternOptions.ToString(),
                OutputInfo = OutputInfo.ToSerializable(item.OutputInfo),
                CurrentLine = item.Pattern.CurrentLine
            };
        }

        public static Regexator.ProjectContainer FromSerializable(ProjectContainer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var container = new Regexator.ProjectContainer() { Mode = item.Mode };

            container.Pattern.RegexOptions = item.ParseRegexOptions();
            container.Pattern.PatternOptions = item.ParsePatternOptions();
            container.Pattern.CurrentLine = Math.Max(item.CurrentLine, 0);

            if (item.Text != null)
            {
                container.Pattern.Text = PatternLibrary.FirstLastEmptyLine.Replace(XmlUtility.DecodeCDataEnd(item.Text), "")
                    .EnsureCarriageReturnLinefeed();
            }

            if (item.ProjectInfo != null)
                container.ProjectInfo = ProjectInfo.FromSerializable(item.ProjectInfo);

            if (item.Groups != null)
                container.OutputInfo.Groups = GroupSettings.FromSerializable(item.Groups);

            if (item.Replacement != null)
                container.Replacement = Replacement.FromSerializable(item.Replacement);

            if (item.OutputInfo != null)
                container.OutputInfo = OutputInfo.FromSerializable(item.OutputInfo);

            return container;
        }

        public RegexOptions ParseRegexOptions()
        {
            if (Enum.TryParse(RegexOptions, out RegexOptions value))
                return value;

            return System.Text.RegularExpressions.RegexOptions.None;
        }

        public PatternOptions ParsePatternOptions()
        {
            if (Enum.TryParse(PatternOptions, out PatternOptions value))
                return value;

            return EnumHelper.ParsePatternOptions(PatternOptions);
        }

        [XmlElement("Header")]
        public ProjectInfo ProjectInfo { get; set; }

        public EvaluationMode Mode { get; set; }

        public string RegexOptions { get; set; }

        [XmlIgnore]
        public string Text { get; set; }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        [XmlElement("Text")]
        public XmlNode TextCData
        {
            get { return (Text == null) ? null : new XmlDocument().CreateCDataSection(Text); }
            set { Text = (value == null) ? "" : value.Value; }
        }

        [XmlElement("Options")]
        public string PatternOptions { get; set; }

        public int CurrentLine { get; set; }

        public Replacement Replacement { get; set; }

        [XmlElement("Groups")]
        public GroupSettings Groups { get; set; }

        [XmlElement("Output")]
        public OutputInfo OutputInfo { get; set; }
    }
}
