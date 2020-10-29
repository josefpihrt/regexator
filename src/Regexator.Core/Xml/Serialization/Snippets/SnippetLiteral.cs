// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Snippets
{
    public class SnippetLiteral
    {
        public static SnippetLiteral ToSerializable(Regexator.Snippets.SnippetLiteral item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new SnippetLiteral()
            {
                Id = item.Id,
                DefaultValue = item.DefaultValue,
                Description = item.Description
            };
        }

        public static Regexator.Snippets.SnippetLiteral FromSerializable(SnippetLiteral item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.Snippets.SnippetLiteral(item.Id, item.DefaultValue, item.Description);
        }

        public string Id { get; set; }

        [XmlElement("Default")]
        public string DefaultValue { get; set; }

        public string Description { get; set; }
    }
}
