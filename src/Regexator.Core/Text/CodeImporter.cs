// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Text;

namespace Regexator.Text
{
    public static class CodeImporter
    {
        public static string Import(string input, ExportMode mode)
        {
            if (mode == ExportMode.CSharp || mode == ExportMode.CSharpVerbatim)
                return ImportCSharp(input);

            if (mode == ExportMode.VisualBasic)
                return ImportVisualBasic(input);

            return null;
        }

        public static string ImportCSharp(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            string s = input;
            var sb = new StringBuilder();
            var state = State.Start;
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i]))
                    continue;

                if (s.Substring(i).StartsWith("/*", StringComparison.Ordinal))
                {
                    i = s.IndexOf("*/", i, StringComparison.Ordinal) + 1;
                }
                else if (s.Substring(i).StartsWith("//", StringComparison.Ordinal))
                {
                    i = s.IndexOf('\n', i + 2);
                    if (i >= 0)
                    {
                        sb.Append(s[i]);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (state <= State.AfterPlus)
                {
                    if (s[i] == '"' || s[i] == '\'')
                    {
                        Literal(sb, s, ref i);
                        state = State.BeforePlus;
                    }
                    else if (s.Substring(i).StartsWith("@\"", StringComparison.Ordinal))
                    {
                        i++;
                        VerbatimLiteral(sb, s, ref i);
                        state = State.BeforePlus;
                    }
                    else if (s.Substring(i).StartsWith("Environment.NewLine", StringComparison.Ordinal))
                    {
                        sb.Append(Environment.NewLine);
                        i += "Environment.NewLine".Length - 1;
                        state = State.BeforePlus;
                    }
                    else if (state == State.AfterPlus)
                    {
                        sb.AppendLine();
                        sb.Append(Resources.ParsingErrorAtIndex).Append(' ').Append(i.ToString(CultureInfo.CurrentCulture));
                        //sb.AppendFormat("ERROR index[{0}] \\x{1:X} {2}", i, (int)s[i], s[i]);
                        break;
                    }
                }
                else if (s[i] == '+')
                {
                    state = State.AfterPlus;
                }
                else
                {
                    break;
                }
            }

            return sb.ToString();
        }

        private static void Literal(StringBuilder sb, string s, ref int i)
        {
            char c = s[i];
            for (i++; i < s.Length; i++)
            {
                if (s[i] == c)
                    return;

                if (s[i] == '\\')
                {
                    i++;
                    int x;
                    int n = 4;
                    const NumberStyles hex = NumberStyles.AllowHexSpecifier;
                    switch (s[i])
                    {
                        case '0':
                            {
                                sb.Append('\0');
                                break;
                            }
                        case 'a':
                            {
                                sb.Append('\a');
                                break;
                            }
                        case 'b':
                            {
                                sb.Append('\b');
                                break;
                            }
                        case 'f':
                            {
                                sb.Append('\f');
                                break;
                            }
                        case 'n':
                            {
                                sb.Append('\n');
                                break;
                            }
                        case 'r':
                            {
                                sb.Append('\r');
                                break;
                            }
                        case 't':
                            {
                                sb.Append('\t');
                                break;
                            }
                        case 'v':
                            {
                                sb.Append('\v');
                                break;
                            }
                        case 'x':
                            {
                                for (n = 0;
                                    n < 4 && int.TryParse(s.Substring(i + 1, n + 1), hex, null, out _);
                                    n++) ;

                                goto case 'u';
                            }
                        case 'U':
                            {
                                n = 8;
                                goto case 'u';
                            }
                        case 'u':
                            {
                                x = int.Parse(s.Substring(i + 1, n), hex, CultureInfo.InvariantCulture);
                                if (x >= 0x10000)
                                {
                                    x -= 0x10000;
                                    sb.Append((char)(
                                        0xD800 | x >> 10));
                                    x = 0xDC00 | (x & 0x3FF);
                                }

                                sb.Append((char)x);
                                i += n;
                                break;
                            }
                        default:
                            {
                                sb.Append(s[i]);
                                break;
                            }
                    }
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
        }

        public static string ImportVisualBasic(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            string s = input;
            var sb = new StringBuilder();
            var state = State.Start;
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i]) || s[i] == '_')
                    continue;

                if (state <= State.AfterPlus)
                {
                    if (s[i] == '"')
                    {
                        VerbatimLiteral(sb, s, ref i);
                        state = State.BeforePlus;
                    }
                    else if (s.Substring(i).StartsWith("Chr(", StringComparison.Ordinal)
                        || s.Substring(i).StartsWith("ChrW(", StringComparison.Ordinal))
                    {
                        int k = s.IndexOf('(', i) + 1;
                        i = s.IndexOf(')', k);

                        if (s[k] == '&')
                        {
                            sb.Append((char)Convert.ToInt32(s.Substring(k + 2, i - k - 2), (s[k + 1] == 'H') ? 16 : 8));
                        }
                        else
                        {
                            sb.Append((char)Convert.ToInt32(s.Substring(k, i - k), CultureInfo.InvariantCulture));
                        }

                        state = State.BeforePlus;
                    }
                    else
                    {
                        for (int k = 0; k < _controlChars.GetLength(0); k++)
                        {
                            if (s.Substring(i).StartsWith(_controlChars[k, 0], StringComparison.Ordinal))
                            {
                                if (_controlChars[k, 1] == null)
                                {
                                    sb.Append(Environment.NewLine);
                                }
                                else
                                {
                                    sb.Append(_controlChars[k, 1]);
                                }

                                i += _controlChars[k, 0].Length - 1;
                                state = State.BeforePlus;
                                break;
                            }
                        }

                        if (state == State.AfterPlus)
                        {
                            sb.AppendLine();
                            sb.AppendFormat("ERROR index[{0}] \\x{1:X} {2}", i, (int)s[i], s[i]);
                            break;
                        }
                    }
                }
                else if (s[i] == '+' || s[i] == '&')
                {
                    state = State.AfterPlus;
                }
                else if (!(s[i] == 'c' && s[i - 1] == '"'))
                {
                    break;
                }
            }

            return sb.ToString();
        }

        private static void VerbatimLiteral(StringBuilder sb, string s, ref int i)
        {
            while (++i < s.Length)
            {
                if (s[i] == '"')
                {
                    if (!(i + 1 < s.Length && s[i + 1] == '"'))
                        return;

                    i++;
                }

                sb.Append(s[i]);
            }
        }

        private static readonly string[,] _controlChars = {
            {"Environment.NewLine"     , null  },
            {"ControlChars.NewLine"    , "\r\n"},
            {"vbNewLine"    , "\r\n"},
            {"ControlChars.CrLf"       , "\r\n"},
            {"vbCrLf"       , "\r\n"},
            {"ControlChars.Lf"         , "\n"  },
            {"vbLf"         , "\n"  },
            {"ControlChars.Cr"         , "\r"  },
            {"vbCr"         , "\r"  },
            {"ControlChars.NullChar"   , "\0"  },
            {"vbNullChar"   , "\0"  },
            {"ControlChars.Back"       , "\b"  },
            {"vbBack"       , "\b"  },
            {"ControlChars.FormFeed"   , "\f"  },
            {"vbFormFeed"   , "\f"  },
            {"ControlChars.Tab"        , "\t"  },
            {"vbTab"        , "\t"  },
            {"ControlChars.VerticalTab", "\v"  },
            {"vbVerticalTab", "\v"  },
            {"ControlChars.Quote"      , "\""  }
        };

        private enum State
        {
            Start,
            AfterPlus,
            BeforePlus
        }
    }
}
