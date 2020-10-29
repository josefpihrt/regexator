// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Pihrtsoft.Text.RegularExpressions.Linq;

namespace Regexator.Text.RegularExpressions
{
    public sealed class CharMatchInfo
    {
        private CharMatchInfo(string pattern)
            : this(pattern, "")
        {
        }

        private CharMatchInfo(string pattern, string comment)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }

        public static IEnumerable<CharMatchInfo> Create(char value)
        {
            return Create(value, false);
        }

        public static IEnumerable<CharMatchInfo> Create(char value, bool inCharGroup)
        {
            return Create(value, inCharGroup, RegexOptions.None);
        }

        public static IEnumerable<CharMatchInfo> Create(char value, RegexOptions options)
        {
            return Create(value, false, options);
        }

        public static IEnumerable<CharMatchInfo> Create(char value, bool inCharGroup, RegexOptions options)
        {
            return Create((int)value, inCharGroup, options);
        }

        internal static IEnumerable<CharMatchInfo> Create(int charCode, bool inCharGroup, RegexOptions options)
        {
            if (charCode < 0 || charCode > 0xFFFF)
                throw new ArgumentOutOfRangeException(nameof(charCode));

            return Create();

            IEnumerable<CharMatchInfo> Create()
            {
                string s = ((char)charCode).ToString();

                if (Regex.IsMatch(s, @"\d", options))
                {
                    yield return new CharMatchInfo(@"\d", "Digit character");
                }
                else
                {
                    yield return new CharMatchInfo(@"\D", "Non-digit character");
                }

                if (Regex.IsMatch(s, @"\s", options))
                {
                    yield return new CharMatchInfo(@"\s", "Whitespace character");
                }
                else
                {
                    yield return new CharMatchInfo(@"\S", "Non-whitespace character");
                }

                if (Regex.IsMatch(s, @"\w", options))
                {
                    yield return new CharMatchInfo(@"\w", "Word character");
                }
                else
                {
                    yield return new CharMatchInfo(@"\W", "Non-word character");
                }

                foreach (GeneralCategory category in Enum.GetValues(typeof(GeneralCategory)).Cast<GeneralCategory>())
                {
                    string pattern = @"\p{" + RegexUtility.GetCategoryDesignation(category) + "}";
                    if (Regex.IsMatch(s, pattern, options))
                    {
                        MemberInfo[] info = typeof(GeneralCategory).GetMember(category.ToString());
                        object[] attributes = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        yield return new CharMatchInfo(
                            pattern,
                            $"Unicode category: {((DescriptionAttribute)attributes[0]).Description}");
                    }
                }

                foreach (NamedBlock block in Enum.GetValues(typeof(NamedBlock)).Cast<NamedBlock>())
                {
                    string pattern = @"\p{" + RegexUtility.GetBlockDesignation(block) + "}";
                    if (Regex.IsMatch(s, pattern, options))
                    {
                        MemberInfo[] info = typeof(NamedBlock).GetMember(block.ToString());
                        object[] attributes = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        yield return new CharMatchInfo(
                            pattern,
                            $"Unicode block: {((DescriptionAttribute)attributes[0]).Description}");
                        break;
                    }
                }

                if (charCode <= 0xFF)
                {
                    switch (RegexUtility.GetEscapeMode((char)charCode, inCharGroup))
                    {
                        case CharEscapeMode.Backslash:
                            {
                                yield return new CharMatchInfo(@"\" + ((char)charCode).ToString(), "Escaped character");
                                break;
                            }
                        case CharEscapeMode.Bell:
                            {
                                yield return new CharMatchInfo(@"\a");
                                break;
                            }
                        case CharEscapeMode.CarriageReturn:
                            {
                                yield return new CharMatchInfo(@"\r");
                                break;
                            }
                        case CharEscapeMode.Escape:
                            {
                                yield return new CharMatchInfo(@"\e");
                                break;
                            }
                        case CharEscapeMode.FormFeed:
                            {
                                yield return new CharMatchInfo(@"\f");
                                break;
                            }
                        case CharEscapeMode.Linefeed:
                            {
                                yield return new CharMatchInfo(@"\n");
                                break;
                            }
                        case CharEscapeMode.Tab:
                            {
                                yield return new CharMatchInfo(@"\t");
                                break;
                            }
                        case CharEscapeMode.VerticalTab:
                            {
                                yield return new CharMatchInfo(@"\v");
                                break;
                            }
                        case CharEscapeMode.None:
                            {
                                yield return new CharMatchInfo(((char)charCode).ToString());
                                break;
                            }
                    }

                    if (inCharGroup && charCode == 8)
                        yield return new CharMatchInfo(@"\b", "Escaped character");

                    yield return new CharMatchInfo(
                        @"\u" + charCode.ToString("X4", CultureInfo.InvariantCulture),
                        "Unicode character (four hexadecimal digits)");

                    yield return new CharMatchInfo(
                        @"\x" + charCode.ToString("X2", CultureInfo.InvariantCulture),
                        "ASCII character (two hexadecimal digits)");

                    yield return new CharMatchInfo(
                        @"\" + Convert.ToString(charCode, 8).PadLeft(2, '0'),
                        "ASCII character (two or three octal digits)");

                    if (charCode > 0 && charCode <= 0x1A)
                    {
                        yield return new CharMatchInfo(
                            @"\c" + Convert.ToChar('a' + charCode - 1),
                            "ASCII control character");
                        yield return new CharMatchInfo(
                            @"\c" + Convert.ToChar('A' + charCode - 1),
                            "ASCII control character");
                    }
                }
            }
        }

        public string Pattern { get; }

        public string Comment { get; }
    }
}
