// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Regexator.Drawing;
using Regexator.Text;

namespace Regexator.Windows.Forms
{
    public class ColorComboBox : ComboBox
    {
        private static readonly BrightnessColorSorter _brightnessSorter = new BrightnessColorSorter();
        private static readonly HueColorSorter _hueSorter = new HueColorSorter();

        public ColorComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        public void LoadColors()
        {
            LoadColors(ColorSortMode.Hue);
        }

        public void LoadColors(ColorSortMode sortMode)
        {
            object item = SelectedItem;
            Items.Clear();
            Items.AddRange(GetColors(sortMode).Select(f => f.Name).ToArray());
            SelectedItem = item;
        }

        private static IEnumerable<Color> GetColors(ColorSortMode mode)
        {
            IEnumerable<Color> colors = DrawingUtility.Colors.Where(f => f != Color.Transparent);
            switch (mode)
            {
                case ColorSortMode.Brightness:
                    return colors.OrderBy(f => f, _brightnessSorter);
                case ColorSortMode.Hue:
                    return colors.OrderBy(f => f, _hueSorter);
                case ColorSortMode.Name:
                    return colors.OrderBy(f => f.Name);
            }

            return colors;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Index >= 0)
            {
                e.DrawBackground();
                var s = (string)Items[e.Index];
                using (var brush = new SolidBrush(Color.FromName(s)))
                {
                    Point p = e.Bounds.Location;
                    p.Offset(1, 1);
                    int height = e.Bounds.Height - 3;
                    var r = new Rectangle(p.X, p.Y, height, height);
                    if (Enabled)
                    {
                        e.Graphics.DrawRectangle(Pens.Black, r);
                        e.Graphics.FillRectangle(brush, new Rectangle(r.X + 1, r.Y + 1, r.Width - 1, r.Height - 1));
                        e.Graphics.DrawString(
                            TextUtility.SplitCamelCase(s),
                            Font,
                            Brushes.Black,
                            e.Bounds.Height + 2,
                            e.Bounds.Top);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(Pens.DarkGray, r);
                    }
                }

                e.DrawFocusRectangle();
            }
        }

        public Color SelectedColor
        {
            get
            {
                return (Enabled && SelectedItem != null)
                    ? Color.FromName(SelectedItem.ToString())
                    : Color.Empty;
            }
            set
            {
                if (value.IsNamedColor)
                    SelectedItem = value.Name;
            }
        }
    }
}
