// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public class CSharpBuilder : CodeBuilder
    {
        public CSharpBuilder()
        {
        }

        public CSharpBuilder(CodeBuilderSettings settings)
            : base(settings)
        {
        }

        public static string GetText(string code, CodeBuilderSettings settings)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var exporter = new CSharpBuilder(settings);
            return exporter.GetText(code);
        }

        protected override string ProcessChar(int charCode)
        {
            switch (charCode)
            {
                case 34:
                    return (Settings.Verbatim) ? "\"" : "\\";
                case 92:
                    return (Settings.Verbatim) ? "" : "\\";
            }

            return "";
        }

        protected override string GetVariableDeclaration(string name, string typeName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return (typeName ?? "var") + Space + name;
        }

        protected override string GetArrayDeclaration(string name, string typeName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            return typeName + "[]" + Space + name;
        }

        protected override string GetReplacementText(string replacement, ReplacementMode mode)
        {
            string s = base.GetReplacementText(replacement, mode);
            switch (mode)
            {
                case ReplacementMode.ToUpper:
                    return "m => m.Result(" + s + ").ToUpper(CultureInfo.CurrentCulture)";
                case ReplacementMode.ToLower:
                    return "m => m.Result(" + s + ").ToLower(CultureInfo.CurrentCulture)";
            }

            return s;
        }

        protected override string LineStart
        {
            get { return ((Settings.Verbatim) ? "@" : "") + base.LineStart; }
        }

        protected override string NewLineString
        {
            get
            {
                switch (Settings.NewLineLiteral)
                {
                    case NewLineLiteral.Lf:
                        return "'\\n'";
                    case NewLineLiteral.CrLf:
                        return "\"\\r\\n\"";
                    case NewLineLiteral.Environment:
                        return "Environment.NewLine";
                    default:
                        return "";
                }
            }
        }

        public override string ConcatOperator
        {
            get { return "+"; }
        }

        protected override string NewKeyword
        {
            get { return "new"; }
        }

        protected override string OrOperator
        {
            get { return "|"; }
        }

        protected override string TerminationString
        {
            get { return ";"; }
        }

        protected override string StringTypeName
        {
            get { return "string"; }
        }

        protected override string BooleanTypeName
        {
            get { return "bool"; }
        }
    }
}
