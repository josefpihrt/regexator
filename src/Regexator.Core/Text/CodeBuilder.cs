// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public abstract class CodeBuilder
    {
        protected CodeBuilder()
            : this(new CodeBuilderSettings())
        {
        }

        protected CodeBuilder(CodeBuilderSettings settings)
        {
            _sb = new StringBuilder();
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        protected abstract string ProcessChar(int charCode);
        protected abstract string GetVariableDeclaration(string name, string typeName);
        protected abstract string GetArrayDeclaration(string name, string typeName);

        protected abstract string NewLineString { get; }
        protected abstract string NewKeyword { get; }
        protected abstract string OrOperator { get; }
        protected abstract string StringTypeName { get; }
        protected abstract string BooleanTypeName { get; }

        protected virtual string GetReplacementText(string replacement, ReplacementMode mode)
        {
            string s = GetText(replacement);

            if (mode != ReplacementMode.None)
                return s.AddIndent(new string(' ', IndentSize), false);

            return s;
        }

        protected string GetVariableDeclaration(string name)
        {
            return GetVariableDeclaration(name, null);
        }

        public string GetText(string code)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var fNewLine = false;
            var sb = new StringBuilder();
            using (var sr = new StringReader(code))
            {
                sb.Append(LineStart);

                while (true)
                {
                    int ch = sr.Read();
                    if (ch == -1)
                    {
                        sb.Append(LineEnd);
                        return sb.ToString();
                    }

                    if (ch == 10)
                    {
                        fNewLine = true;
                    }
                    else
                    {
                        if (fNewLine)
                        {
                            if (Settings.Multiline)
                            {
                                sb.Append("\n");
                            }
                            else
                            {
                                sb.Append(LineEnd);
                                sb.Append(NewLineReplacement);
                                sb.Append(LineStart);
                            }

                            fNewLine = false;
                        }

                        sb.Append(ProcessChar(ch));
                        sb.Append((char)ch);
                    }
                }
            }
        }

        public string GetRegexOptions(RegexOptions options)
        {
            if (options != RegexOptions.None || !Settings.OmitNoneOptions)
            {
                string prefix = RegexOptionsEnum + Dot;

                if (Settings.AddNamespace)
                    prefix = RegexNamespace + Dot + prefix;

                return string.Join(
                    Space + OrOperator + Space,
                    options.ToValues().Select(f => prefix + f.ToString()));
            }

            return "";
        }

        public void AppendRegexConstructor(string pattern, RegexOptions options)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            Append(NewKeyword);
            AppendSpace();
            AppendRegexClass();
            AppendParameters(GetText(pattern), GetRegexOptions(options));
        }

        public void AppendRegexInstance(string pattern, RegexOptions options)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            Append(GetVariableDeclaration("regex"));
            AppendAssignment();
            AppendRegexConstructor(pattern, options);
            AppendTerminationString();
        }

        public void AppendStaticMatch(string input, string pattern, RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            Append(GetVariableDeclaration("match", "Match"));
            AppendAssignment();
            AppendRegexClass();
            AppendDot();
            Append(MatchMethod);
            AppendParameters(GetText(input), GetText(pattern), GetRegexOptions(options));
            AppendTerminationString();
        }

        public void AppendStaticIsMatch(string input, string pattern, RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            Append(GetVariableDeclaration("success", BooleanTypeName));
            AppendAssignment();
            AppendRegexClass();
            AppendDot();
            Append(IsMatchMethod);
            AppendParameters(GetText(input), GetText(pattern), GetRegexOptions(options));
            AppendTerminationString();
        }

        public void AppendInstanceMatch(string input, string pattern, RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            AppendRegexInstance(pattern, options);
            AppendLine();
            Append(GetVariableDeclaration("match", "Match"));
            AppendAssignment();
            Append("regex");
            AppendDot();
            Append(MatchMethod);
            AppendParameters(GetText(input));
            AppendTerminationString();
        }

        public void AppendInstanceIsMatch(string input, string pattern, RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            AppendRegexInstance(pattern, options);
            AppendLine();
            Append(GetVariableDeclaration("success", BooleanTypeName));
            AppendAssignment();
            Append("regex");
            AppendDot();
            Append(IsMatchMethod);
            AppendParameters(GetText(input));
            AppendTerminationString();
        }

        public void AppendStaticReplace(
            string input,
            string pattern,
            string replacement,
            ReplacementMode mode,
            RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            Append(GetVariableDeclaration("result", StringTypeName));
            AppendAssignment();
            AppendRegexClass();
            AppendDot();
            Append(ReplaceMethod);
            AppendParameters(
                GetText(input),
                GetText(pattern),
                GetReplacementText(replacement, mode),
                GetRegexOptions(options));
            AppendTerminationString();
        }

        public void AppendInstanceReplace(
            string input,
            string pattern,
            string replacement,
            ReplacementMode mode,
            RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            AppendRegexInstance(pattern, options);
            AppendLine();
            Append(GetVariableDeclaration("result", StringTypeName));
            AppendAssignment();
            Append("regex");
            AppendDot();
            Append(ReplaceMethod);
            AppendParameters(GetText(input), GetReplacementText(replacement, mode));
            AppendTerminationString();
        }

        public void AppendStaticSplit(string input, string pattern, RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            Append(GetArrayDeclaration("splits", StringTypeName));
            AppendAssignment();
            AppendRegexClass();
            AppendDot();
            Append(SplitMethod);
            AppendParameters(GetText(input), GetText(pattern), GetRegexOptions(options));
            AppendTerminationString();
        }

        public void AppendInstanceSplit(string input, string pattern, RegexOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            AppendRegexInstance(pattern, options);
            AppendLine();
            Append(GetArrayDeclaration("splits", StringTypeName));
            AppendAssignment();
            Append("regex");
            AppendDot();
            Append(SplitMethod);
            AppendParameters(GetText(input));
            AppendTerminationString();
        }

        protected void Append(string value)
        {
            _sb.Append(value);
        }

        protected void Append(char value)
        {
            _sb.Append(value);
        }

        protected void AppendLine()
        {
            _sb.AppendLine();
        }

        protected void AppendTerminationString()
        {
            Append(TerminationString);
        }

        protected void AppendText(string text)
        {
            Append(GetText(text));
        }

        protected void AppendAssignment()
        {
            AppendSpace();
            Append(EqualSign);
            AppendSpace();
        }

        private void AppendParameters(params string[] values)
        {
            AppendOpeningRoundBracket();
            var isFirst = true;
            bool indent = Settings.NewLineOnParameters || values.Any(f => f.IsMultiline());

            if (indent)
                Indent();

            foreach (string value in values.Where(f => !string.IsNullOrEmpty(f)))
            {
                if (!isFirst)
                {
                    AppendComma();
                    if (!indent)
                        AppendSpace();
                }

                if (indent)
                    AppendLine();

                _sb.Append(Regex.Replace(value, "^", IndentString, RegexOptions.Multiline));
                isFirst = false;
            }

            if (indent)
                Unindent();

            AppendClosingRoundBracket();
        }

        protected void AppendNamespacePrefix()
        {
            if (Settings.AddNamespace)
            {
                Append(RegexNamespace);
                AppendDot();
            }
        }

        protected void AppendRegexClass()
        {
            AppendNamespacePrefix();
            Append(RegexClass);
        }

        private void AppendOpeningRoundBracket()
        {
            Append(OpeningRoundBracket);
        }

        private void AppendClosingRoundBracket()
        {
            Append(ClosingRoundBracket);
        }

        protected void AppendSpace()
        {
            Append(Space);
        }

        protected void AppendComma()
        {
            Append(Comma);
        }

        protected void AppendDot()
        {
            Append(Dot);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        public string IndentString
        {
            get { return new string(' ', IndentSize * IndentLevel); }
        }

        public void Indent()
        {
            IndentLevel++;
        }

        public void Unindent()
        {
            if (IndentLevel >= 1)
                IndentLevel--;
        }

        public string NewLineReplacement
        {
            get
            {
                switch (Settings.ConcatOperatorPosition)
                {
                    case ConcatOperatorPosition.End:
                        return "\n" + ConcatOperator + " " + NewLineString + " " + ConcatOperator + " ";
                    case ConcatOperatorPosition.Start:
                        return " "
                            + LineBreakString
                            + "\n"
                            + ConcatOperator
                            + " "
                            + NewLineString
                            + " "
                            + ConcatOperator
                            + " ";
                    default:
                        return null;
                }
            }
        }

        protected virtual string LineStart
        {
            get { return "\""; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        protected string LineEnd
        {
            get { return "\""; }
        }

        protected virtual string LineBreakString
        {
            get { return ""; }
        }

        public virtual string ConcatOperator
        {
            get { return Settings.ConcatOperator; }
        }

        protected virtual string TerminationString
        {
            get { return ""; }
        }

        public CodeBuilderSettings Settings
        {
            get { return _settings; }
            set { _settings = value ?? throw new ArgumentNullException("value"); }
        }

        public int IndentSize
        {
            get { return _indentSize; }
            set { _indentSize = Math.Max(value, 0); }
        }

        public int IndentLevel
        {
            get { return _indentLevel; }
            set { _indentLevel = Math.Max(value, 0); }
        }

        private readonly StringBuilder _sb;
        private CodeBuilderSettings _settings;
        private int _indentSize = 4;
        private int _indentLevel;

        public const string RegexClass = "Regex";
        public const string MatchMethod = "Match";
        public const string IsMatchMethod = "IsMatch";
        public const string SplitMethod = "Split";
        public const string ReplaceMethod = "Replace";
        public const string RegexOptionsEnum = "RegexOptions";
        public const string RegexNamespace = "System.Text.RegularExpressions";
        protected const string Dot = ".";
        protected const string Space = " ";
        protected const string Comma = ",";
        protected const string EqualSign = "=";
        protected const string OpeningRoundBracket = "(";
        protected const string ClosingRoundBracket = ")";
    }
}
