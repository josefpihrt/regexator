// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Regexator.FileSystem
{
    public sealed class InvalidInputNode : InvalidNode
    {
        internal InvalidInputNode(string path, Input input)
            : base(path)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            SetImage(NodeImageIndex.FileInputError);
            Text = GetText();
        }

        private string GetText()
        {
            try
            {
                return System.IO.Path.GetFileName(Path);
            }
            catch (ArgumentException)
            {
                return Path;
            }
        }

        public override FileSystemNode CreateNode()
        {
            var info = new FileInfo(Path);
            return new FileInputNode(info, Input);
        }

        public void SetImage(NodeImageIndex index)
        {
            ImageIndex = (int)index;
            SelectedImageIndex = (int)index;
        }

        public ProjectNode ProjectNode
        {
            get { return Parent as ProjectNode; }
        }

        public Input Input { get; }
    }
}
