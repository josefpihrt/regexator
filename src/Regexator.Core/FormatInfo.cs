// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Regexator.Text;

namespace Regexator
{
    public class FormatInfo : ICloneable
    {
        public FormatInfo(Color backColor, Color foreColor, bool bold)
        {
            Controls = new ObservableCollection<Control>();
            BackColorEnabled = true;
            ForeColorEnabled = true;
            BoldEnabled = true;
            _backColor = backColor;
            _foreColor = foreColor;
            Bold = bold;
            Name = "";
            Text = "";
            Controls.CollectionChanged += (f, f2) => Controls_CollectionChanged(f, f2);
        }

        public void LoadValues(FormatInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            BackColorEnabled = info.BackColorEnabled;
            BackColor = info.BackColor;
            ForeColorEnabled = info.ForeColorEnabled;
            ForeColor = info.ForeColor;
            BoldEnabled = info.BoldEnabled;
            Bold = info.Bold;
        }

        public void LoadEnabledValues(FormatInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (BackColorEnabled)
                BackColor = info.BackColor;

            if (ForeColorEnabled)
                ForeColor = info.ForeColor;

            if (BoldEnabled)
                Bold = info.Bold;
        }

        private void Controls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                SetFontAndColors(e.NewItems.Cast<Control>());
        }

        public void Highlight(RichTextBox rtb, IEnumerable<TextSpan> textSpans)
        {
            foreach (TextSpan selection in textSpans.Where(f => f.Length > 0))
                Highlight(rtb, selection);
        }

        public void Highlight(RichTextBox rtb, TextSpan textSpan)
        {
            if (textSpan == null)
                throw new ArgumentNullException(nameof(textSpan));

            Highlight(rtb, textSpan.Index, textSpan.Length);
        }

        public void Highlight(RichTextBox rtb, int index, int length)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            rtb.Select(index, length);

            if (BackColorEnabled)
                rtb.SelectionBackColor = BackColor;

            if (ForeColorEnabled)
                rtb.SelectionColor = ForeColor;

            if (Font != null)
                rtb.SelectionFont = Font;
        }

        private void SetBackColor()
        {
            SetBackColor(Controls);
        }

        private void SetBackColor(IEnumerable<Control> items)
        {
            if (BackColorEnabled)
            {
                foreach (Control item in items)
                    item.BackColor = BackColor;
            }
        }

        private void SetForeColor()
        {
            SetForeColor(Controls);
        }

        private void SetForeColor(IEnumerable<Control> items)
        {
            if (ForeColorEnabled)
            {
                foreach (Control item in items)
                    item.ForeColor = ForeColor;
            }
        }

        private void SetFont()
        {
            SetFont(Controls);
        }

        private void SetFont(IEnumerable<Control> items)
        {
            if (Font != null)
            {
                foreach (Control item in items)
                    item.Font = Font;
            }
        }

        private void SetFontAndColors(IEnumerable<Control> items)
        {
            SetBackColor(items);
            SetForeColor(items);
            SetFont(items);
        }

        public object Clone()
        {
            return new FormatInfo(BackColor, ForeColor, Bold)
            {
                Group = Group,
                Text = Text,
                Name = Name,
                BackColorEnabled = BackColorEnabled,
                ForeColorEnabled = ForeColorEnabled,
                BoldEnabled = BoldEnabled
            };
        }

        public override string ToString()
        {
            return Text;
        }

        private void OnGroupPropertyChanged(object sender, EventArgs e)
        {
            SetNewFont();
        }

        private void SetNewFont()
        {
            Font = (Group != null)
                ? new Font(Group.FontFamily, Group.FontSize, FontStyle)
                : null;
        }

        protected virtual void OnFontChanged(EventArgs e)
        {
            SetFont();

            FontChanged?.Invoke(this, e);

            OnFormatChanged(EventArgs.Empty);
        }

        protected virtual void OnBackColorChanged(EventArgs e)
        {
            SetBackColor();

            BackColorChanged?.Invoke(this, e);

            OnFormatChanged(EventArgs.Empty);
        }

        protected virtual void OnForeColorChanged(EventArgs e)
        {
            SetForeColor();

            ForeColorChanged?.Invoke(this, e);

            OnFormatChanged(EventArgs.Empty);
        }

        protected virtual void OnFormatChanged(EventArgs e)
        {
            FormatChanged?.Invoke(this, e);
        }

        public FontStyle FontStyle
        {
            get { return (Bold && BoldEnabled) ? FontStyle.Bold : FontStyle.Regular; }
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

        public FormatGroup Group
        {
            get { return _group; }
            set
            {
                if (!Equals(_group, value))
                {
                    if (_group != null)
                    {
                        _group.FontFamilyChanged -= (f, f2) => OnGroupPropertyChanged(f, f2);
                        _group.FontSizeChanged -= (f, f2) => OnGroupPropertyChanged(f, f2);
                    }

                    _group = value;
                    if (_group != null)
                    {
                        _group.FontFamilyChanged += (f, f2) => OnGroupPropertyChanged(f, f2);
                        _group.FontSizeChanged += (f, f2) => OnGroupPropertyChanged(f, f2);
                    }

                    SetNewFont();
                }
            }
        }

        public bool Bold
        {
            get { return _bold; }
            set
            {
                if (_bold != value)
                {
                    _bold = value;
                    SetNewFont();
                }
            }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                if (_foreColor != value)
                {
                    _foreColor = value;
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        public Font Font
        {
            get { return _font; }
            private set
            {
                if (!Equals(_font, value))
                {
                    _font = value;
                    OnFontChanged(EventArgs.Empty);
                }
            }
        }

        public bool BackColorEnabled { get; set; }
        public bool ForeColorEnabled { get; set; }
        public bool BoldEnabled { get; set; }
        public ObservableCollection<Control> Controls { get; }

        private FormatGroup _group;
        private Color _backColor;
        private Color _foreColor;
        private bool _bold;
        private string _text;
        private string _name;
        private Font _font;

        public event EventHandler FontChanged;
        public event EventHandler BackColorChanged;
        public event EventHandler ForeColorChanged;
        public event EventHandler FormatChanged;
    }
}
