// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Regexator.Snippets
{
    internal class LiteralInfo
    {
        public LiteralInfo(SnippetLiteral literal)
            : this(literal, literal.DefaultValue)
        {
        }

        public LiteralInfo(SnippetLiteral literal, string value)
        {
            Literal = literal;
            Value = value;
            Indexes = new List<int>();
        }

        public string Value { get; set; }
        public List<int> Indexes { get; }
        public SnippetLiteral Literal { get; }
    }
}
