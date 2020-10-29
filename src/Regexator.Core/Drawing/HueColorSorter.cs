// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Regexator.Drawing
{
    public sealed class HueColorSorter : IComparer<Color>
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

            int i = x.GetHue().CompareTo(y.GetHue());
            if (i == 0)
            {
                i = x.GetSaturation().CompareTo(y.GetSaturation());

                if (i == 0)
                    i = x.GetBrightness().CompareTo(y.GetBrightness());
            }

            return -i;
        }

        private static readonly string[] _colors = new[]
        {
            "LightPink",
            "Pink",
            "Crimson",
            "LavenderBlush",
            "PaleVioletRed",
            "HotPink",
            "DeepPink",
            "MediumVioletRed",
            "Orchid",
            "Thistle",
            "Plum",
            "Violet",
            "Magenta",
            "Fuchsia",
            "DarkMagenta",
            "Purple",
            "MediumOrchid",
            "DarkViolet",
            "DarkOrchid",
            "Indigo",
            "BlueViolet",
            "MediumPurple",
            "MediumSlateBlue",
            "SlateBlue",
            "DarkSlateBlue",
            "Lavender",
            "GhostWhite",
            "Blue",
            "MediumBlue",
            "MidnightBlue",
            "DarkBlue",
            "Navy",
            "RoyalBlue",
            "CornflowerBlue",
            "LightSteelBlue",
            "LightSlateGray",
            "SlateGray",
            "DodgerBlue",
            "AliceBlue",
            "SteelBlue",
            "LightSkyBlue",
            "SkyBlue",
            "DeepSkyBlue",
            "LightBlue",
            "PowderBlue",
            "CadetBlue",
            "Azure",
            "LightCyan",
            "PaleTurquoise",
            "Cyan",
            "Aqua",
            "DarkTurquoise",
            "DarkSlateGray",
            "DarkCyan",
            "Teal",
            "MediumTurquoise",
            "LightSeaGreen",
            "Turquoise",
            "Aquamarine",
            "MediumAquamarine",
            "MediumSpringGreen",
            "MintCream",
            "SpringGreen",
            "MediumSeaGreen",
            "SeaGreen",
            "Honeydew",
            "LightGreen",
            "PaleGreen",
            "DarkSeaGreen",
            "LimeGreen",
            "Lime",
            "ForestGreen",
            "Green",
            "DarkGreen",
            "Chartreuse",
            "LawnGreen",
            "GreenYellow",
            "DarkOliveGreen",
            "YellowGreen",
            "OliveDrab",
            "Beige",
            "LightGoldenrodYellow",
            "Ivory",
            "LightYellow",
            "Yellow",
            "Olive",
            "DarkKhaki",
            "LemonChiffon",
            "PaleGoldenrod",
            "Khaki",
            "Gold",
            "Cornsilk",
            "Goldenrod",
            "DarkGoldenrod",
            "FloralWhite",
            "OldLace",
            "Wheat",
            "Moccasin",
            "Orange",
            "PapayaWhip",
            "BlanchedAlmond",
            "NavajoWhite",
            "AntiqueWhite",
            "Tan",
            "BurlyWood",
            "Bisque",
            "DarkOrange",
            "Linen",
            "Peru",
            "PeachPuff",
            "SandyBrown",
            "Chocolate",
            "SaddleBrown",
            "SeaShell",
            "Sienna",
            "LightSalmon",
            "Coral",
            "OrangeRed",
            "DarkSalmon",
            "Tomato",
            "MistyRose",
            "Salmon",
            "Snow",
            "LightCoral",
            "RosyBrown",
            "IndianRed",
            "Red",
            "Brown",
            "Firebrick",
            "DarkRed",
            "Maroon",
            "White",
            "WhiteSmoke",
            "Gainsboro",
            "LightGray",
            "Silver",
            "DarkGray",
            "Gray",
            "DimGray",
            "Black"
        };
    }
}
