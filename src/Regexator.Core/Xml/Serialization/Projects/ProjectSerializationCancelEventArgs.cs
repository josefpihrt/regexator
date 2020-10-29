// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Regexator.Xml.Serialization.Projects
{
    public class ProjectSerializationCancelEventArgs : CancelEventArgs
    {
        [DebuggerStepThrough]
        public ProjectSerializationCancelEventArgs(Regexator.Project project, string filePath)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public Regexator.Project Project { get; }
        public string FilePath { get; }
        public Version ApplicationVersion { get; set; }
    }
}
