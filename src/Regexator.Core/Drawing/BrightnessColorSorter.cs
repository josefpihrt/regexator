// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Regexator.Drawing
{
    public sealed class BrightnessColorSorter : IComparer<Color>
    {
        public int Compare(Color x, Color y)
        {
            if (x.IsNamedColor && y.IsNamedColor)
            {
                int xi = Array.IndexOf(_colors, x.Name);
                if (xi != -1)
                {
                    int yi = Array.IndexOf(_colors, y.Name);

                    if (yi != -1)
                        return xi.CompareTo(yi);
                }
            }

            int i = x.GetBrightness().CompareTo(y.GetBrightness());
            if (i == 0)
            {
                i = x.GetHue().CompareTo(y.GetHue());

                if (i == 0)
                    i = x.GetSaturation().CompareTo(y.GetSaturation());
            }

            return -i;
        }

        private static readonly string[] _colors = new[]
        {
            "White",
            "WhiteSmoke",
            "Snow",
            "MistyRose",
            "SeaShell",
            "Linen",
            "AntiqueWhite",
            "BlanchedAlmond",
            "PapayaWhip",
            "OldLace",
            "FloralWhite",
            "Cornsilk",
            "LemonChiffon",
            "Beige",
            "LightGoldenrodYellow",
            "Ivory",
            "LightYellow",
            "Honeydew",
            "MintCream",
            "Azure",
            "LightCyan",
            "AliceBlue",
            "Lavender",
            "GhostWhite",
            "LavenderBlush",
            "Gainsboro",
            "LightGray",
            "PeachPuff",
            "Bisque",
            "NavajoWhite",
            "Moccasin",
            "Wheat",
            "PaleGoldenrod",
            "PaleTurquoise",
            "Pink",
            "LightPink",
            "Silver",
            "LightCoral",
            "Salmon",
            "LightSalmon",
            "BurlyWood",
            "Khaki",
            "LightGreen",
            "PaleGreen",
            "Aquamarine",
            "PowderBlue",
            "LightBlue",
            "SkyBlue",
            "LightSkyBlue",
            "LightSteelBlue",
            "Thistle",
            "Plum",
            "Violet",
            "HotPink",
            "DarkGray",
            "RosyBrown",
            "Tomato",
            "DarkSalmon",
            "Coral",
            "SandyBrown",
            "Tan",
            "DarkSeaGreen",
            "MediumAquamarine",
            "CornflowerBlue",
            "MediumSlateBlue",
            "MediumPurple",
            "Orchid",
            "PaleVioletRed",
            "Gray",
            "IndianRed",
            "Red",
            "OrangeRed",
            "Peru",
            "DarkOrange",
            "Orange",
            "Gold",
            "DarkKhaki",
            "Yellow",
            "YellowGreen",
            "GreenYellow",
            "Chartreuse",
            "LimeGreen",
            "Lime",
            "SpringGreen",
            "Turquoise",
            "MediumTurquoise",
            "Cyan",
            "Aqua",
            "CadetBlue",
            "DeepSkyBlue",
            "DodgerBlue",
            "LightSlateGray",
            "SlateGray",
            "RoyalBlue",
            "Blue",
            "SlateBlue",
            "BlueViolet",
            "MediumOrchid",
            "Magenta",
            "Fuchsia",
            "DeepPink",
            "DimGray",
            "Brown",
            "Firebrick",
            "Sienna",
            "Chocolate",
            "Goldenrod",
            "LawnGreen",
            "MediumSeaGreen",
            "MediumSpringGreen",
            "LightSeaGreen",
            "DarkTurquoise",
            "SteelBlue",
            "MediumBlue",
            "DarkOrchid",
            "DarkViolet",
            "MediumVioletRed",
            "Crimson",
            "SaddleBrown",
            "DarkGoldenrod",
            "OliveDrab",
            "DarkOliveGreen",
            "ForestGreen",
            "SeaGreen",
            "DarkSlateBlue",
            "DarkRed",
            "Maroon",
            "Olive",
            "Green",
            "DarkSlateGray",
            "DarkCyan",
            "Teal",
            "MidnightBlue",
            "DarkBlue",
            "Navy",
            "Indigo",
            "DarkMagenta",
            "Purple",
            "DarkGreen",
            "Black"
        };
    }
}
