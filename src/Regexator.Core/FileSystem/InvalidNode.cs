// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public abstract class InvalidNode : TreeNode
    {
        protected InvalidNode(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public abstract FileSystemNode CreateNode();

        public string Path { get; }
    }
}
