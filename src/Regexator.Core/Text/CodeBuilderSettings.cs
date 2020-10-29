// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text
{
    public class CodeBuilderSettings
    {
        public CodeBuilderSettings()
        {
            ConcatOperator = "+";
            OmitNoneOptions = true;
        }

        public NewLineLiteral NewLineLiteral { get; set; }
        public ConcatOperatorPosition ConcatOperatorPosition { get; set; }
        public bool Verbatim { get; set; }
        public bool AddNamespace { get; set; }
        public bool NewLineOnParameters { get; set; }
        public bool OmitNoneOptions { get; set; }
        public bool Multiline { get; set; }

        public string ConcatOperator
        {
            get { return _concatOperator; }
            set { _concatOperator = value ?? ""; }
        }

        private string _concatOperator;
    }
}
