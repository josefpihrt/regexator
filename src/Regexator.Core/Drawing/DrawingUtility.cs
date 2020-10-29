// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Regexator.Collections.Generic;

namespace Regexator.Drawing
{
    public static class DrawingUtility
    {
        public static ReadOnlyCollection<Color> Colors
        {
            get
            {
                return _colors
                    ?? (_colors = typeof(Color).GetProperties(
                        BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                        .Select(f => f.GetValue(null, null))
                        .Cast<Color>()
                        .ToReadOnly());
            }
        }

        public static ReadOnlyCollection<Color> BasicColors
        {
            get
            {
                return _basicColors
                    ?? (_basicColors = Array.AsReadOnly(new Color[] {
                        Color.Black,
                        Color.Blue,
                        Color.Cyan,
                        Color.Gray,
                        Color.Green,
                        Color.Lime,
                        Color.Magenta,
                        Color.Maroon,
                        Color.Navy,
                        Color.Olive,
                        Color.Purple,
                        Color.Red,
                        Color.Silver,
                        Color.Teal,
                        Color.White,
                        Color.Yellow }));
            }
        }

        public static ReadOnlyCollection<Color> SystemColors
        {
            get
            {
                return _systemColors
                    ?? (_systemColors = typeof(SystemColors).GetProperties(
                        BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                        .Select(f => f.GetValue(null, null))
                        .Cast<Color>()
                        .ToReadOnly());
            }
        }

        public static ReadOnlyCollection<Color> ProfessionalColors
        {
            get
            {
                return _professionalColors
                    ?? (_professionalColors = typeof(ProfessionalColors).GetProperties(
                        BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                        .Select(f => f.GetValue(null, null))
                        .Cast<Color>()
                        .ToReadOnly());
            }
        }

        private static ReadOnlyCollection<Color> _colors;
        private static ReadOnlyCollection<Color> _basicColors;
        private static ReadOnlyCollection<Color> _systemColors;
        private static ReadOnlyCollection<Color> _professionalColors;
    }
}
