// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
{
    public class ProjectInfo
    {
        public static ProjectInfo ToSerializable(Regexator.ProjectInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new ProjectInfo()
            {
                Title = (!string.IsNullOrEmpty(item.Title)) ? item.Title : null,
                Author = (!string.IsNullOrEmpty(item.Author)) ? item.Author : null,
                Description = (!string.IsNullOrEmpty(item.Description)) ? item.Description : null,
                Version = (item.Version != _noVersion) ? item.Version.ToString() : null
            };
        }

        public static Regexator.ProjectInfo FromSerializable(ProjectInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var info = new Regexator.ProjectInfo()
            {
                Title = item.Title,
                Author = item.Author,
                Description = item.Description
            };

            if (System.Version.TryParse(item.Version, out Version version))
                info.Version = version;

            return info;
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string HelpUrl { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Keyword")]
        public string[] Keywords { get; set; }

        private static readonly Version _noVersion = new Version();
    }
}
