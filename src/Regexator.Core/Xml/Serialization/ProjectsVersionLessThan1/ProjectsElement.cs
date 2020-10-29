// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
{
    [XmlRoot("Projects")]
    public class ProjectsElement
    {
        public ProjectsElement()
        {
        }

        public ProjectsElement(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            Projects = new Project[] { project };
        }

        public ProjectsElement(IEnumerable<Project> projects)
        {
            if (projects == null)
                throw new ArgumentNullException(nameof(projects));

            Projects = projects.ToArray();
        }

        public Project FirstProject
        {
            get { return Projects?.FirstOrDefault(); }
        }

        [XmlAttribute("AppVersion")]
        public string ApplicationVersion { get; set; }

        [XmlAttribute]
        public string FilePath { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlElement("Project")]
        public Project[] Projects { get; set; }
    }
}