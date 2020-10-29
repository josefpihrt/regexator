// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Regexator.Xml.Serialization.Projects
{
    public class ProjectSerializationEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public ProjectSerializationEventArgs(Regexator.Project project, string filePath)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public Regexator.Project Project { get; }
        public string FilePath { get; }
    }
}
