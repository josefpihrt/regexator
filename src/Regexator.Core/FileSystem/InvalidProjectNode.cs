// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Regexator.FileSystem
{
    public sealed class InvalidProjectNode : InvalidNode
    {
        internal InvalidProjectNode(string path)
            : base(path)
        {
            SetImage(NodeImageIndex.ProjectError);
            Text = GetText();
        }

        private string GetText()
        {
            try
            {
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }
            catch (ArgumentException)
            {
                return Path;
            }
        }

        public override FileSystemNode CreateNode()
        {
            var info = new FileInfo(Path);
            var node = new ProjectNode(info);
            node.LoadInputNodes();
            return node;
        }

        public void SetImage(NodeImageIndex index)
        {
            ImageIndex = (int)index;
            SelectedImageIndex = (int)index;
        }

        public RootNode RootNode
        {
            get { return Parent as RootNode; }
        }
    }
}