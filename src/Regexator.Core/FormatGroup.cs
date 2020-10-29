// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;

namespace Regexator
{
    public class FormatGroup : ICloneable
    {
        public FormatGroup(FontFamily fontFamily, int fontSize)
        {
            if (fontSize < 1)
                throw new ArgumentOutOfRangeException(nameof(fontSize));

            _fontFamily = fontFamily ?? throw new ArgumentNullException(nameof(fontFamily));
            _fontSize = fontSize;
            Text = "";
            Name = "";
        }

        public void LoadValues(FormatGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            FontFamily = group.FontFamily;
            FontSize = group.FontSize;
        }

        public object Clone()
        {
            return new FormatGroup(FontFamily, FontSize)
            {
                Text = Text,
                Name = Name
            };
        }

        public override string ToString()
        {
            return Text;
        }

        protected virtual void OnFontFamilyChanged(EventArgs e)
        {
            FontFamilyChanged?.Invoke(this, e);
        }

        protected virtual void OnFontSizeChanged(EventArgs e)
        {
            FontSizeChanged?.Invoke(this, e);
        }

        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");

                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnFontSizeChanged(EventArgs.Empty);
                }
            }
        }

        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    OnFontFamilyChanged(EventArgs.Empty);
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value ?? ""; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        private string _text;
        private string _name;
        private int _fontSize;
        private FontFamily _fontFamily;

        public event EventHandler FontFamilyChanged;
        public event EventHandler FontSizeChanged;
    }
}
