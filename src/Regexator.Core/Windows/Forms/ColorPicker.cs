// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.Drawing;

namespace Regexator.Windows.Forms
{
    public sealed class ColorPicker : IDisposable
    {
        private const int _buttonWidthHeight = 30;
        private const string _defaultSorterKey = "Hue";

        private static ReadOnlyCollection<Color> _colors;
        private static readonly Regex _splitCamelCaseRegex = new Regex(@"(?<=\p{Ll})(?=\p{Lu})");

        private readonly ColorPickerForm _frm;
        private readonly ColorPickerTableLayoutPanel _tlp;
        private bool _disposed;
        private List<ButtonInfo> _buttons;
        private readonly Dictionary<string, IComparer<Color>> _sorters;
        private bool _showColorToolTip;
        private string _sorterKey;
        private bool _loaded;

        public ColorPicker()
        {
            _frm = new ColorPickerForm();

            var cms = new ContextMenuStrip();
            cms.Opening += (object sender, CancelEventArgs e) =>
            {
                cms.Items.Clear();
                cms.Items.AddRange(CreateToolStripMenuItems().ToArray());
                e.Cancel = (cms.Items.Count == 0);
            };

            _frm.ContextMenuStrip = cms;

            _tlp = new ColorPickerTableLayoutPanel();
            _frm.Controls.Add(_tlp);

            _buttons = new List<ButtonInfo>();

            _sorters = new Dictionary<string, IComparer<Color>>()
            {
                ["Hue"] = new HueColorSorter(),
                ["Brightness"] = new BrightnessColorSorter(),
                ["Name"] = new NameColorSorter()
            };

            ShowColorToolTip = true;
        }

        public DialogResult ShowDialog()
        {
            Load();
            return _frm.ShowDialog();
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            Load();
            return _frm.ShowDialog(owner);
        }

        private void Load()
        {
            LoadTable(10, 14);
            LoadColors();

            Button btn = SelectButton(SelectedColor);
            if (btn != null)
                _frm.InitialButton = btn;

            _loaded = true;
        }

        private void LoadTable(int rowCount, int columnCount)
        {
            _tlp.RowStyles.Clear();
            _tlp.ColumnStyles.Clear();

            _tlp.RowCount = rowCount;
            _tlp.ColumnCount = columnCount;

            for (int i = 0; i < columnCount; i++)
                _tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, _buttonWidthHeight));

            for (int i = 0; i < rowCount; i++)
                _tlp.RowStyles.Add(new ColumnStyle(SizeType.Absolute, _buttonWidthHeight));

            _tlp.Size = _tlp.PreferredSize;
        }

        private void LoadColors()
        {
            _tlp.SuspendLayout();
            _tlp.Controls.Clear();

            if (_buttons.Count == 0)
                CreateButtons();

            _buttons = _buttons.OrderBy(f => f.Color, GetSorter()).ToList();

            int rowIndex = 0;
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (i >= ((rowIndex + 1) * _tlp.ColumnCount))
                    rowIndex++;

                _tlp.Controls.Add(_buttons[i].Button, i - (rowIndex * _tlp.ColumnCount), rowIndex);
            }

            Control btn = _tlp.GetControlFromPosition(0, 0);

            btn?.Select();

            _tlp.ResumeLayout(true);

            if (_frm.ActiveControl is Button selectedButton)
                SelectButton(selectedButton.BackColor);
        }

        private IComparer<Color> GetSorter()
        {
            if (!string.IsNullOrEmpty(SorterKey) && _sorters.ContainsKey(SorterKey))
                return _sorters[SorterKey];

            if (!_sorters.ContainsKey(_defaultSorterKey))
                _sorters.Add(_defaultSorterKey, new HueColorSorter());

            return _sorters[_defaultSorterKey];
        }

        private Button SelectButton(Color color)
        {
            ButtonInfo info = _buttons.Find(f => f.Color.Name == color.Name);
            if (info != null)
            {
                info.Button.Select();
                return info.Button;
            }

            return null;
        }

        private void CreateButtons()
        {
            _buttons = new List<ButtonInfo>(Colors.Count);
            int i = 0;
            int rowIndex = 0;
            foreach (Color color in Colors)
            {
                if (i >= ((rowIndex + 1) * _tlp.ColumnCount))
                    rowIndex++;

                _buttons.Add(CreateButton(color));
                i++;
            }
        }

        private ButtonInfo CreateButton(Color color)
        {
            var button = new Button()
            {
                Name = color.Name,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(1)
            };

            button.FlatAppearance.MouseOverBackColor = color;
            button.FlatAppearance.BorderColor = Color.Black;

            button.Click += (object sender, EventArgs e) =>
            {
                SelectedColor = ((Button)sender).BackColor;
                _frm.DialogResult = DialogResult.OK;
                _frm.Close();
            };

            var toolTip = new ToolTip();
            toolTip.SetToolTip(button, _splitCamelCaseRegex.Replace(color.Name, " "));
            toolTip.Active = ShowColorToolTip;

            return new ButtonInfo() { Button = button, ToolTip = toolTip, Color = color };
        }

        private IEnumerable<ToolStripMenuItem> CreateToolStripMenuItems()
        {
            foreach (KeyValuePair<string, IComparer<Color>> sorter in _sorters)
            {
                yield return new ToolStripMenuItem(
                    "Sort by " + sorter.Key,
                    null,
                    (object sender, EventArgs e) =>
                    {
                        if (!((ToolStripMenuItem)sender).Checked)
                            SorterKey = sorter.Key;
                    })
                {
                    Checked = (SorterKey == sorter.Key)
                };
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _frm.Dispose();
                    _tlp.Dispose();
                }

                _disposed = true;
            }
        }

        public Icon Icon
        {
            get { return _frm.Icon; }
            set { _frm.Icon = value; }
        }

        public string Text
        {
            get { return _frm.Text; }
            set { _frm.Text = value; }
        }

        public string SorterKey
        {
            get { return _sorterKey; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_sorterKey != value)
                {
                    _sorterKey = value;

                    if (_loaded)
                        LoadColors();
                }
            }
        }

        public bool CloseOnEscapeKey
        {
            get { return _frm.CloseOnEscapeKey; }
            set { _frm.CloseOnEscapeKey = value; }
        }

        public bool ShowColorToolTip
        {
            get { return _showColorToolTip; }
            set
            {
                if (_showColorToolTip != value)
                {
                    _showColorToolTip = value;

                    foreach (ButtonInfo info in _buttons)
                        info.ToolTip.Active = _showColorToolTip;
                }

                _showColorToolTip = value;
            }
        }

        public static ReadOnlyCollection<Color> Colors
        {
            get
            {
                if (_colors == null)
                {
                    PropertyInfo[] properties = typeof(Color).GetProperties(
                        BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);

                    var lst = new List<Color>(properties.Length - 1);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var color = (Color)properties[i].GetValue(null, null);

                        if (color != Color.Transparent)
                            lst.Add(color);
                    }

                    _colors = new ReadOnlyCollection<Color>(lst);
                }

                return _colors;
            }
        }

        public Color SelectedColor { get; set; }

        private class ButtonInfo
        {
            public Button Button { get; set; }
            public Color Color { get; set; }
            public ToolTip ToolTip { get; set; }
        }
    }
}
