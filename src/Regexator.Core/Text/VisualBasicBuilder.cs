// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public class VisualBasicBuilder : CodeBuilder
    {
        public VisualBasicBuilder()
        {
        }

        public VisualBasicBuilder(CodeBuilderSettings settings)
            : base(settings)
        {
        }

        public static string GetText(string code, CodeBuilderSettings settings)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var exporter = new VisualBasicBuilder(settings);
            return exporter.GetText(code);
        }

        protected override string ProcessChar(int charCode)
        {
            return (charCode == 34) ? "\"" : "";
        }

        protected override string GetVariableDeclaration(string name, string typeName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return "Dim " + name + ((typeName != null) ? " As " + typeName : "");
        }

        protected override string GetArrayDeclaration(string name, string typeName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            return "Dim " + name + " As " + typeName + "()";
        }

        protected override string GetReplacementText(string replacement, ReplacementMode mode)
        {
            string s = base.GetReplacementText(replacement, mode);
            switch (mode)
            {
                case ReplacementMode.ToUpper:
                    return "Function(m) m.Result(" + s + ").ToUpper(CultureInfo.CurrentCulture)";
                case ReplacementMode.ToLower:
                    return "Function(m) m.Result(" + s + ").ToLower(CultureInfo.CurrentCulture)";
            }

            return s;
        }

        protected override string LineBreakString
        {
            get { return (Settings.ConcatOperatorPosition == ConcatOperatorPosition.Start) ? " _" : base.LineBreakString; }
        }

        protected override string NewLineString
        {
            get
            {
                switch (Settings.NewLineLiteral)
                {
                    case NewLineLiteral.Lf:
                        return "vbLf";
                    case NewLineLiteral.CrLf:
                        return "vbCrLf";
                    case NewLineLiteral.Environment:
                        return "Environment.NewLine";
                    default:
                        return null;
                }
            }
        }

        protected override string NewKeyword
        {
            get { return "New"; }
        }

        protected override string OrOperator
        {
            get { return "Or"; }
        }

        protected override string StringTypeName
        {
            get { return "String"; }
        }

        protected override string BooleanTypeName
        {
            get { return "Boolean"; }
        }
    }
}
