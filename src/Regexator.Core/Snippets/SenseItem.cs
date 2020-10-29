// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Drawing;

namespace Regexator.Snippets
{
    public abstract class SenseItem
    {
        public abstract bool IsMatch(string text);

        public abstract string Text { get; }
        public abstract Icon Icon { get; }

        public virtual string Description
        {
            get { return ""; }
        }

        public virtual int Rank
        {
            get { return 0; }
        }

        public virtual bool IsExtensible
        {
            get { return false; }
        }

        public virtual SnippetCodeKind ExtendedKind
        {
            get { return SnippetCodeKind.None; }
        }

        public virtual bool Visible
        {
            get { return true; }
        }

        public virtual bool Favorite { get; set; }
        public virtual bool UseExtended { get; set; }
    }
}
