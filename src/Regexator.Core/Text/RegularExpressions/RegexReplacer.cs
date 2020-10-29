// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public sealed class RegexReplacer
    {
        public string Replacement { get; }
        public ReplacementMode Mode { get; }

        private RegexReplacer(string replacement, ReplacementMode mode)
        {
            Replacement = replacement;
            Mode = mode;
        }

        public static string Replace(Regex regex, string input, string replacement, ReplacementMode mode)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            if (mode == ReplacementMode.None)
                return regex.Replace(input, replacement);

            var replacer = new RegexReplacer(replacement, mode);
            return replacer.Replace(regex, input);
        }

        private string Replace(Regex regex, string input)
        {
            return regex.Replace(input, f => Evaluator(f));
        }

        private string Evaluator(Match match)
        {
            switch (Mode)
            {
                case ReplacementMode.ToUpper:
                    return match.Result(Replacement).ToUpper(CultureInfo.CurrentCulture);
                case ReplacementMode.ToLower:
                    return match.Result(Replacement).ToLower(CultureInfo.CurrentCulture);
                case ReplacementMode.Char:
                    {
                        switch (Replacement.Length)
                        {
                            case 0:
                                return "";
                            case 1:
                                return new string(Replacement[0], match.Length);
                            default:
                                return Multiply(Replacement, match.Length);
                        }
                    }
                default:
                    return match.Result(Replacement);
            }
        }

        private static string Multiply(string value, int multiplier)
        {
            var sb = new StringBuilder(multiplier * value.Length);
            for (int i = 0; i < multiplier; i++)
                sb.Append(value);

            return sb.ToString();
        }
    }
}
