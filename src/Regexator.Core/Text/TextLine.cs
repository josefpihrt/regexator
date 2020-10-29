// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text
{
    public class TextLine
    {
        public TextLine()
            : this("")
        {
        }

        public TextLine(string value)
            : this(value, -1)
        {
        }

        public TextLine(string value, int index)
        {
            Value = value;
            Index = index;
        }

        public int Index { get; set; }

        public string Value
        {
            get { return _value; }
            set { _value = value ?? ""; }
        }

        private string _value;
    }
}
