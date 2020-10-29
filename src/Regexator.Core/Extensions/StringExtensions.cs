// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Regexator.Text;

namespace Regexator
{
    public static class StringExtensions
    {
        public static IEnumerable<string> EnumerateLines(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EnumerateLines2();

            IEnumerable<string> EnumerateLines2()
            {
                using (var sr = new StringReader(value))
                {
                    string line = null;
                    while (true)
                    {
                        line = sr.ReadLine();

                        if (line != null)
                        {
                            yield return line;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static string AddParentheses(this string value)
        {
            return AddBrackets(value, BracketKind.Round);
        }

        public static string AddBrackets(this string value, BracketKind kind)
        {
            switch (kind)
            {
                case BracketKind.Angle:
                    return string.Concat("<", value, ">");
                case BracketKind.Curly:
                    return string.Concat("{", value, "}");
                case BracketKind.Round:
                    return string.Concat("(", value, ")");
                case BracketKind.Square:
                    return string.Concat("[", value, "]");
                default:
                    return value;
            }
        }

        public static StringCollection ToStringCollection(this string value)
        {
            return new StringCollection() { value };
        }

        public static int GetIndentLength(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Regex.Match(value, @"^[\s-[\r\n]]+").Length;
        }

        public static string TrimMultiline(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return PatternLibrary.LineLeadingTrailingWhiteSpace.Replace(value, "");
        }

        public static string TrimStartMultiline(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return PatternLibrary.LineLeadingWhiteSpace.Replace(value, "");
        }

        public static string TrimEndMultiline(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return PatternLibrary.LineTrailingWhiteSpace.Replace(value, "");
        }

        public static string SetLineIndent(this string value, string indent)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (indent == null)
                throw new ArgumentNullException(nameof(indent));

            return PatternLibrary.LineLeadingWhiteSpace.Replace(value, indent);
        }

        [DebuggerStepThrough]
        public static bool ContainsAny(this string value, IEnumerable<string> values)
        {
            return ContainsAny(value, values, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool ContainsAny(this string value, IEnumerable<string> values, StringComparison comparison)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return values.Any(f => value.IndexOf(f, comparison) != -1);
        }

        [DebuggerStepThrough]
        public static bool StartsWithAny(this string value, IEnumerable<string> values)
        {
            return StartsWithAny(value, values, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool StartsWithAny(this string value, IEnumerable<string> values, StringComparison comparison)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return values.Any(f => value.StartsWith(f, comparison));
        }

        [DebuggerStepThrough]
        public static bool EndsWithAny(this string value, IEnumerable<string> values)
        {
            return EndsWithAny(value, values, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool EndsWithAny(this string value, IEnumerable<string> values, StringComparison comparison)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return values.Any(f => value.EndsWith(f, comparison));
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Cr")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Lf")]
        public static string EnsureCarriageReturnLinefeed(this string input)
        {
            return TextUtility.EnsureCarriageReturnLinefeed(input);
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Lf")]
        public static string RemoveLinefeed(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Replace("\n", "");
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Cr")]
        public static string RemoveCarriageReturn(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Replace("\r", "");
        }

        public static bool IsMultiline(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Contains("\n");
        }

        public static string ToUpperOrLower(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            string upper = input.ToUpper(CultureInfo.CurrentCulture);
            return (!string.Equals(input, upper, StringComparison.CurrentCulture))
                ? upper
                : input.ToLower(CultureInfo.CurrentCulture);
        }

        public static string ToSingleline(this string input)
        {
            return ToSingleline(input, "");
        }

        public static string ToSingleline(this string input, string newLineReplacement)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (newLineReplacement == null)
                throw new ArgumentNullException(nameof(newLineReplacement));

            return PatternLibrary.NewLine.Replace(input, newLineReplacement);
        }

        public static string Enclose(this string input, string[] values)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (string item in values)
                input = Enclose(input, item);

            return input;
        }

        public static string Enclose(this string input, char value)
        {
            return Enclose(input, value.ToString());
        }

        public static string Enclose(this string input, string value)
        {
            return Enclose(input, value, value);
        }

        public static string Enclose(this string input, char left, char right)
        {
            return Enclose(input, left.ToString(), right.ToString());
        }

        public static string Enclose(this string input, string left, string right)
        {
            return string.Concat(left, input, right);
        }

        public static bool IsUpper(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input == input.ToUpper(CultureInfo.CurrentCulture);
        }

        public static bool IsLower(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input == input.ToLower(CultureInfo.CurrentCulture);
        }

        public static bool IsCapitalized(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length > 1 && char.IsUpper(input[0]))
            {
                var s = new string(input.Skip(1).ToArray());
                return s == s.ToLower(CultureInfo.CurrentCulture);
            }

            return false;
        }

        public static bool IsMixedCase(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length > 2)
            {
                var s = new string(input.Skip(1).ToArray());
                return !(s == s.ToLower(CultureInfo.CurrentCulture) || s == s.ToUpper(CultureInfo.CurrentCulture));
            }

            return false;
        }

        public static string GetFirstLine(this string input)
        {
            return GetFirstLine(input, "");
        }

        public static string GetFirstLine(this string input, string suffix)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (suffix == null)
                throw new ArgumentNullException(nameof(suffix));

            Match match = PatternLibrary.FirstLine.Match(input);
            return (match.Success) ? match.Value + suffix : input;
        }

        public static string CutExcess(this string input, int maxLength, string suffix)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength));

            if (suffix == null)
                throw new ArgumentNullException(nameof(suffix));

            if (maxLength >= input.Length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, maxLength) + suffix;
            }
        }
    }
}
