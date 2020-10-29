// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Regexator
{
    [Serializable]
    [DefaultProperty("Description")]
    public class ProjectInfo : ICloneable
    {
        public ProjectInfo()
        {
            Title = "";
            Author = "";
            Description = "";
            Version = new Version();
            Keywords = new SortedSet<string>();
        }

        public ProjectInfo(ProjectInfo value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Author = value.Author;
            Description = value.Description;
            HelpUrl = (value.HelpUrl != null) ? new Uri(HelpUrl.OriginalString) : null;
            Keywords = new SortedSet<string>(value.Keywords);
            Title = value.Title;
            Version = (Version)value.Version.Clone();
        }

        public static ContainerProps GetChangedProps(ProjectInfo first, ProjectInfo second, ContainerProps props)
        {
            if (first == null || second == null || ReferenceEquals(first, second))
                return ContainerProps.None;

            var value = ContainerProps.None;

            if (props.Contains(ContainerProps.Author) && !string.Equals(first.Author ?? "", second.Author ?? ""))
                value |= ContainerProps.Author;

            if (props.Contains(ContainerProps.Description)
                && !string.Equals(first.Description ?? "", second.Description ?? ""))
            {
                value |= ContainerProps.Description;
            }

            if (props.Contains(ContainerProps.HelpUrl) && first.HelpUrl != second.HelpUrl)
                value |= ContainerProps.HelpUrl;

            if (props.Contains(ContainerProps.Title) && !string.Equals(first.Title ?? "", second.Title ?? ""))
                value |= ContainerProps.Title;

            if (props.Contains(ContainerProps.Version) && first.Version != second.Version)
                value |= ContainerProps.Version;

            if (props.Contains(ContainerProps.Keywords) && !first.Keywords.SequenceEqual(second.Keywords))
                value |= ContainerProps.Keywords;

            return value;
        }

        public object Clone()
        {
            return new ProjectInfo(this);
        }

        public void IncrementVersionRevision()
        {
            Version v = Version;
            Version = new Version(v.Major, v.Minor, Math.Max(v.Build, 0), Math.Max(v.Revision, 0) + 1);
        }

        [LocalizedDisplayName("Title")]
        public string Title
        {
            get { return _title; }
            set { _title = value ?? ""; }
        }

        [LocalizedDisplayName("Author")]
        public string Author
        {
            get { return _author; }
            set { _author = value ?? ""; }
        }

        [LocalizedDisplayName("Description")]
        public string Description
        {
            get { return _description; }
            set { _description = value ?? ""; }
        }

        [TypeConverter(typeof(VersionConverter))]
        [LocalizedDisplayName("Version")]
        public Version Version
        {
            get { return _version; }
            set { _version = value ?? throw new ArgumentNullException("value"); }
        }

        [Browsable(false)]
        public Uri HelpUrl { get; set; }

        [Browsable(false)]
        public SortedSet<string> Keywords { get; }

        private string _title;
        private string _author;
        private string _description;
        private Version _version;

        public static readonly ContainerProps AllProps = ContainerProps.Author
            | ContainerProps.Description
            | ContainerProps.HelpUrl
            | ContainerProps.Keywords
            | ContainerProps.Title
            | ContainerProps.Version;
    }
}
