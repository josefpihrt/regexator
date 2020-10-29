// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Regexator.Snippets;
using Regexator.Text;
using Regexator.Text.RegularExpressions;
using Regexator.Collections.Generic;

namespace Regexator
{
    public static class EnumHelper
    {
        private static IEnumerable<T> ParseValues<T>(string input) where T : struct
        {
            if (!string.IsNullOrEmpty(input))
            {
                foreach (string value in input
                    .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim()))
                {
                    if (Enum.TryParse(value, out T result))
                        yield return result;
                }
            }
        }

        public static InputOptions ParseInputOptions(string input)
        {
            var result = InputOptions.None;

            foreach (InputOptions value in ParseValues<InputOptions>(input))
                result |= value;

            return result;
        }

        public static InputOptions GetValue(this IEnumerable<InputOptions> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var result = InputOptions.None;

            foreach (InputOptions value in values)
                result |= value;

            return result;
        }

        public static PatternOptions ParsePatternOptions(string input)
        {
            var result = PatternOptions.None;

            foreach (PatternOptions value in ParseValues<PatternOptions>(input))
                result |= value;

            return result;
        }

        public static PatternOptions GetValue(this IEnumerable<PatternOptions> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var result = PatternOptions.None;

            foreach (PatternOptions value in values)
                result |= value;

            return result;
        }

        public static ReplacementOptions ParseReplacementOptions(string input)
        {
            var result = ReplacementOptions.None;

            foreach (ReplacementOptions value in ParseValues<ReplacementOptions>(input))
                result |= value;

            return result;
        }

        public static ReplacementOptions GetValue(this IEnumerable<ReplacementOptions> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var result = ReplacementOptions.None;

            foreach (ReplacementOptions value in values)
                result |= value;

            return result;
        }

        public static ItemAttributes ParseAttributes(string input)
        {
            var result = ItemAttributes.None;

            foreach (ItemAttributes value in ParseValues<ItemAttributes>(input))
                result |= value;

            return result;
        }

        public static OutputOptions ParseOutputOptions(string input)
        {
            var result = OutputOptions.None;

            foreach (OutputOptions value in ParseValues<OutputOptions>(input))
                result |= value;

            return result;
        }

        public static OutputOptions GetValue(this IEnumerable<OutputOptions> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var result = OutputOptions.None;

            foreach (OutputOptions value in values)
                result |= value;

            return result;
        }

        public static string GetDescription(SnippetCodeKind value)
        {
            switch (value)
            {
                case SnippetCodeKind.Escaped:
                    return Resources.Escaped;
                case SnippetCodeKind.Format:
                    return Resources.Format;
                case SnippetCodeKind.Lazy:
                    return Resources.Lazy;
                case SnippetCodeKind.Negative:
                    return Resources.Negative;
                case SnippetCodeKind.None:
                    return Resources.None;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(EvaluationMode value)
        {
            switch (value)
            {
                case EvaluationMode.Match:
                    return Resources.Match;
                case EvaluationMode.Split:
                    return Resources.Split;
                case EvaluationMode.Replace:
                    return Resources.Replace;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(ContainerProps value)
        {
            switch (value)
            {
                case ContainerProps.Attributes:
                    return Resources.Attributes;
                case ContainerProps.IgnoredGroups:
                    return Resources.IgnoredGroups;
                case ContainerProps.Mode:
                    return Resources.Mode;
                case ContainerProps.OutputOptions:
                case ContainerProps.PatternOptions:
                    return Resources.Options;
                case ContainerProps.RegexOptions:
                    return Resources.RegexOptions;
                case ContainerProps.ReplacementNewLine:
                    return Resources.NewLine;
                case ContainerProps.ReplacementOptions:
                    return Resources.Options;
                case ContainerProps.ReplacementText:
                    return Resources.Text;
                case ContainerProps.ReplacementCurrentLine:
                    return Resources.CurrentLine;
                case ContainerProps.PatternText:
                    return Resources.Text;
                case ContainerProps.PatternCurrentLine:
                    return Resources.CurrentLine;
                case ContainerProps.Author:
                    return Resources.Author;
                case ContainerProps.Description:
                    return Resources.Description;
                //case ContainerProps.HelpUrl:
                //    break;
                //case ContainerProps.Keywords:
                //    break;
                case ContainerProps.Title:
                    return Resources.Title;
                case ContainerProps.Version:
                    return Resources.Version;
                case ContainerProps.FileSystemSearchMode:
                    return Resources.SearchMode;
                case ContainerProps.FileSystemSearchOption:
                    return Resources.SearchOption;
                case ContainerProps.FileSystemFileNamePart:
                    return Resources.FileNamePart;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(InputProps value)
        {
            switch (value)
            {
                case InputProps.Name:
                    return Resources.Name;
                case InputProps.Options:
                    return Resources.Options;
                case InputProps.Text:
                    return Resources.Text;
                case InputProps.NewLine:
                    return Resources.NewLine;
                case InputProps.CurrentLine:
                    return Resources.CurrentLine;
                case InputProps.Encoding:
                    return Resources.Encoding;
                case InputProps.Attributes:
                    return Resources.Attributes;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(InputOptions value)
        {
            switch (value)
            {
                case InputOptions.CurrentLineOnly:
                    return Resources.UseCurrentLineOnly;
                case InputOptions.Highlight:
                    return Resources.HighlightText;
                case InputOptions.CurrentLineIncludesNewLine:
                    return Resources.CurrentLineIncludesNewLine;
                case InputOptions.ReplacementSync:
                    return Resources.ReplacementSync;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(PatternOptions value)
        {
            switch (value)
            {
                case PatternOptions.CurrentLineOnly:
                    return Resources.UseCurrentLineOnly;
                case PatternOptions.InputSync:
                    return Resources.InputSync;
                case PatternOptions.ReplacementSync:
                    return Resources.ReplacementSync;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(OutputOptions value)
        {
            switch (value)
            {
                case OutputOptions.Highlight:
                    return Resources.HighlightText;
                case OutputOptions.Info:
                    return Resources.ShowInfo;
                case OutputOptions.CarriageReturnSymbol:
                    return Resources.CarriageReturnSymbol;
                case OutputOptions.LinefeedSymbol:
                    return Resources.LineFeedSymbol;
                case OutputOptions.TabSymbol:
                    return Resources.TabSymbol;
                case OutputOptions.SpaceSymbol:
                    return Resources.SpaceSymbol;
                case OutputOptions.NoCaptureSymbol:
                    return Resources.NoCaptureSymbolTableOnly;
                case OutputOptions.WrapText:
                    return Resources.WrapTextTableOnly;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(ReplacementOptions value)
        {
            switch (value)
            {
                case ReplacementOptions.CurrentLineOnly:
                    return Resources.UseCurrentLineOnly;
                case ReplacementOptions.CurrentLineIncludesNewLine:
                    return Resources.CurrentLineIncludesNewLine;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static string GetDescription(ReplacementMode value)
        {
            switch (value)
            {
                case ReplacementMode.None:
                    return Resources.None;
                case ReplacementMode.ToUpper:
                    return Resources.ToUppercase;
                case ReplacementMode.ToLower:
                    return Resources.ToLowercase;
                default:
                    return TextUtility.SplitCamelCase(value);
            }
        }

        public static bool IsBrowsable<TEnum>(TEnum value)
        {
            return typeof(TEnum)
                .GetField(Enum.GetName(typeof(TEnum), value))
                .GetCustomAttributes(typeof(BrowsableAttribute), false)
                .OfType<BrowsableAttribute>()
                .All(f => f.Browsable);
        }

        public static ReadOnlyCollection<InputOptions> InputOptionsValues
        {
            get { return _inputOptionsValuesLazy.Value; }
        }

        public static ReadOnlyCollection<PatternOptions> PatternOptionsValues
        {
            get { return _patternOptionsValuesLazy.Value; }
        }

        public static ReadOnlyCollection<ReplacementOptions> ReplacementOptionsValues
        {
            get { return _replacementOptionsValuesLazy.Value; }
        }

        public static ReadOnlyCollection<OutputOptions> OutputOptionsValues
        {
            get { return _outputOptionsValuesLazy.Value; }
        }

        private static readonly Lazy<ReadOnlyCollection<PatternOptions>> _patternOptionsValuesLazy
            = new Lazy<ReadOnlyCollection<PatternOptions>>(() =>
            {
                return Enum.GetValues(typeof(PatternOptions))
                    .Cast<PatternOptions>()
                    .Where(f => f != PatternOptions.None)
                    .ToReadOnly();
            });

        private static readonly Lazy<ReadOnlyCollection<InputOptions>> _inputOptionsValuesLazy
            = new Lazy<ReadOnlyCollection<InputOptions>>(() =>
            {
                return Enum.GetValues(typeof(InputOptions))
                    .Cast<InputOptions>()
                    .Where(f => f != InputOptions.None)
                    .ToReadOnly();
            });

        private static readonly Lazy<ReadOnlyCollection<ReplacementOptions>> _replacementOptionsValuesLazy
            = new Lazy<ReadOnlyCollection<ReplacementOptions>>(() =>
            {
                return Enum.GetValues(typeof(ReplacementOptions))
                    .Cast<ReplacementOptions>()
                    .Where(f => f != ReplacementOptions.None)
                    .ToReadOnly();
            });

        private static readonly Lazy<ReadOnlyCollection<OutputOptions>> _outputOptionsValuesLazy
            = new Lazy<ReadOnlyCollection<OutputOptions>>(() =>
            {
                return Enum.GetValues(typeof(OutputOptions))
                    .Cast<OutputOptions>()
                    .Where(f => f != OutputOptions.None)
                    .ToReadOnly();
            });
    }
}

