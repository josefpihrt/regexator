// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Regexator.FileSystem
{
    public interface INode
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        RootNode GetRootNode();

        string RootPath { get; }
        NodeKind Kind { get; }
    }
}
