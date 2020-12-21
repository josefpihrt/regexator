// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public static class TextUtility
    {
        private static readonly Regex _whiteSpacesRegex = new Regex(@"\s+");
        private static readonly Regex _lineStartRegex = new Regex(@"[^\n]*", RegexOptions.RightToLeft);
        private static readonly Regex _lineEndRegex = new Regex(@"[^\r\n]*");

        public static string RemoveWhiteSpace(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return _whiteSpacesRegex.Replace(input, "");
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Cr")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Lf")]
        public static string EnsureCarriageReturnLinefeed(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return RegexLibrary.LinefeedWithoutCarriageReturn.Replace(input, "\r\n");
        }

        public static int GetFirstInvalidXmlChar(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            for (int i = 0; i < input.Length; i++)
            {
                if (!IsValidXmlChar(input[i]))
                    return input[i];
            }

            return -1;
        }

        public static bool IsValidXmlChar(int value)
        {
            return
                value == 0x9
                    || value == 0xA
                    || value == 0xD
                    || (value >= 0x20 && value <= 0xD7FF)
                    || (value >= 0xE000 && value <= 0xFFFD)
                    || (value >= 0x10000 && value <= 0x10FFFF);
        }

        public static string SplitCamelCase(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return SplitCamelCase(value.ToString());
        }

        public static string SplitCamelCase(object value, string separator)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return SplitCamelCase(value.ToString(), separator);
        }

        public static string SplitCamelCase(string value)
        {
            return SplitCamelCase(value, " ");
        }

        public static string SplitCamelCase(string value, string separator)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            int len = value.Length;
            if (len > 0)
            {
                var sb = new StringBuilder(len);
                for (int i = 0; i < len; i++)
                {
                    char ch = value[i];
                    int j = i;
                    Func<char, bool> predicate;
                    var fCaps = false;
                    var fAppend = true;
                    if (char.IsLetter(ch))
                    {
                        if (char.IsUpper(ch) && (i + 1) < len && char.IsUpper(value[i + 1]))
                        {
                            predicate = f => char.IsUpper(f);
                            fCaps = true;
                            j++;
                        }
                        else
                        {
                            predicate = f => char.IsLower(f);
                        }

                        j++;
                    }
                    else if (char.IsDigit(ch))
                    {
                        predicate = f => char.IsDigit(f);
                        j++;
                    }
                    else
                    {
                        predicate = f => !char.IsLetterOrDigit(f);
                        fAppend = false;
                    }

                    while (j < len && predicate(value[j]))
                        j++;

                    if (fAppend)
                    {
                        if (fCaps && j > (i + 1) && j < len && char.IsLower(value[j]))
                            j--;

                        if (sb.Length > 0)
                            sb.Append(separator);

                        j -= i;
                        sb.Append(value, i, j);
                        i += j - 1;
                    }
                }

                return sb.ToString();
            }

            return "";
        }

        public static string RemoveDiacritics(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return new string(input.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        public static string Untabify(string input, int spaceCount)
        {
            return Untabify(input, spaceCount, 0);
        }

        public static string Untabify(string input, int spaceCount, int initialPosition)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (spaceCount < 1)
                throw new ArgumentOutOfRangeException(nameof(spaceCount));

            if (initialPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(initialPosition));

            int index = initialPosition;
            var sb = new StringBuilder();

            using (var sr = new StringReader(input))
            {
                while (true)
                {
                    int num = sr.Read();
                    switch (num)
                    {
                        case 9:
                            {
                                int cnt = spaceCount - (index % spaceCount);
                                sb.Append(new string(' ', cnt));
                                index += cnt;
                                break;
                            }
                        case 10:
                            {
                                sb.Append("\n");
                                index = 0;
                                break;
                            }
                        case -1:
                            return sb.ToString();
                        default:
                            {
                                sb.Append((char)num);
                                index++;
                                break;
                            }
                    }
                }
            }
        }

        public static string AddIndent(string input, string indent)
        {
            return AddIndent(input, indent, true);
        }

        public static string AddIndent(string input, int count)
        {
            return AddIndent(input, count, true);
        }

        public static string AddIndent(string input, int count, bool indentFirstLine)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return AddIndent(input, new string(' ', count), indentFirstLine);
        }

        public static string AddIndent(this string input, string indent, bool indentFirstLine)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (indent == null)
                throw new ArgumentNullException(nameof(indent));

            string s = (indentFirstLine) ? indent : "";

            return s + input.Replace("\n", "\n" + indent);
        }

        public static string RemoveIndent(string input)
        {
            return RemoveIndent(input, Indenter.DefaultSpaceCount);
        }

        public static string RemoveIndent(string input, int spaceCount)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (spaceCount < 1)
                throw new ArgumentOutOfRangeException(nameof(spaceCount));

            string newLine = Environment.NewLine;
            var sb = new StringBuilder();
            var processor = new Indenter(spaceCount);

            using (var sr = new StringReader(input))
            {
                var isFirst = true;
                string line = null;

                while (true)
                {
                    line = sr.ReadLine();
                    if (line != null)
                    {
                        if (!isFirst)
                        {
                            sb.Append(newLine);
                        }
                        else
                        {
                            isFirst = false;
                        }

                        sb.Append(processor.RemoveLineIndent(line));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return sb.ToString();
        }

        public static bool IsValidNewLine(string newLine)
        {
            return newLine != null && (newLine == "\n" || newLine == "\r\n");
        }

        public static TextSpan ExtendToEntireLine(string text, int index)
        {
            return ExtendToEntireLine(text, index, 0);
        }

        public static TextSpan ExtendToEntireLine(string text, int index, int length)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (index < 0 || index > text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (length < 0 || (index + length) > text.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            int startIndex = _lineStartRegex.Match(text, index).Index;
            int endIndex = _lineEndRegex.Match(text, index + length).Index;

            return new TextSpan(startIndex, endIndex - startIndex);
        }

        public static IEnumerable<int> IndexesOf(string input, string value)
        {
            return IndexesOf(input, value, SearchOptions.None);
        }

        public static IEnumerable<int> IndexesOf(string input, string value, SearchOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return IndexesOf();

            IEnumerable<int> IndexesOf()
            {
                string pattern = RegexUtility.Escape(value);

                if ((options & SearchOptions.MatchWholeWord) != 0)
                    pattern = @"\b" + pattern + @"\b";

                var regexOptions = RegexOptions.None;

                if ((options & SearchOptions.MatchCase) == 0)
                    regexOptions = RegexOptions.IgnoreCase;

                if ((options & SearchOptions.CultureInvariant) != 0)
                    regexOptions = RegexOptions.CultureInvariant;

                Match match = Regex.Match(input, pattern, regexOptions);
                while (match.Success)
                {
                    yield return match.Index;
                    match = match.NextMatch();
                }
            }
        }

        public static string Multiply(string value, int multiplier)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (multiplier < 1)
                throw new ArgumentOutOfRangeException(nameof(multiplier));

            switch (multiplier)
            {
                case 1:
                    return value;
                case 2:
                    return value + value;
            }

            var sb = new StringBuilder(multiplier * value.Length);
            for (int i = 0; i < multiplier; i++)
                sb.Append(value);

            return sb.ToString();
        }

        public static int GetLineFromCharIndex(string[] lines, int index)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            if (lines.Length == 0)
                return 0;

            if (index <= 0)
                return 0;

            int textLength = 0;
            for (int i = 0; i < lines.Length; i++)
                textLength += lines[i].Length;

            if (index >= textLength)
                return lines.Length - 1;

            if ((index / textLength) < 0.5)
            {
                int len = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    len += lines[i].Length;
                    if (len >= index)
                        return i;

                    len++;
                }
            }
            else
            {
                int len = textLength;
                for (int i = (lines.Length - 1); i >= 0; i--)
                {
                    len -= lines[i].Length;
                    if (index >= len)
                        return i;

                    len--;
                }
            }

            return 0;
        }

        public static int GetFirstCharIndexFromLine(string[] lines, int lineIndex)
        {
            return GetFirstCharIndexFromLine(lines, lineIndex, NewLineMode.Lf);
        }

        public static int GetFirstCharIndexFromLine(string[] lines, int lineIndex, NewLineMode newLineMode)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            if (lineIndex < 0 || lineIndex >= lines.Length)
                throw new ArgumentOutOfRangeException(nameof(lineIndex));

            if (lineIndex == 0)
                return 0;

            int value = 0;

            switch (newLineMode)
            {
                case NewLineMode.CrLf:
                    {
                        value += lineIndex * 2;
                        break;
                    }
                case NewLineMode.Lf:
                    {
                        value += lineIndex;
                        break;
                    }
                default:
                    {
                        Debug.Fail("");
                        break;
                    }
            }

            for (int i = 0; i < lines.Length; i++)
            {
                value += lines[i].Length;

                lineIndex--;
                if (lineIndex == 0)
                    break;
            }

            return value;
        }
    }
}
