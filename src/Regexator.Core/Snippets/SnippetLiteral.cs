// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regexator.Snippets
{
    [DebuggerDisplay("Id: {Id}")]
    public class SnippetLiteral : ICloneable
    {
        private SnippetLiteral(string id)
        {
            Id = id;
            Token = CreateToken(id);
            DefaultValue = "";
            Description = "";
        }

        public SnippetLiteral(string id, string defaultValue)
            : this(id, defaultValue, "")
        {
        }

        public SnippetLiteral(string id, string defaultValue, string description)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (!_idValidatorRegex.IsMatch(id))
                throw new ArgumentException("Snippet literal id is not valid.", nameof(id));

            if (_reservedIds.Contains(id))
                throw new ArgumentException("Snippet literal id cannot be equal to \"selected\" or \"end\"", nameof(id));

            Id = id;
            Token = CreateToken(id);
            DefaultValue = defaultValue ?? "";
            Description = description ?? "";
        }

        private static string CreateToken(string id)
        {
            return TokenDelimiter + id + TokenDelimiter;
        }

        public static string RemoveReservedLiterals(string input)
        {
            return RemoveLiterals(input, ReservedLiterals);
        }

        public static string RemoveLiterals(string input, IEnumerable<SnippetLiteral> literals)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (literals == null)
                throw new ArgumentNullException(nameof(literals));

            foreach (SnippetLiteral item in literals)
                input = item.RemoveToken(input);

            return input;
        }

        public string RemoveToken(string input)
        {
            return ReplaceToken(input, "");
        }

        public string ReplaceToken(string input, string replacement)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            return input.Replace(Token, replacement);
        }

        public int FindToken(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.IndexOf(Token, StringComparison.CurrentCulture);
        }

        public object Clone()
        {
            return new SnippetLiteral(Id, DefaultValue, Description);
        }

        public string Id { get; }
        public string Token { get; }
        public string Description { get; }
        public string DefaultValue { get; }

        public static readonly SnippetLiteral SelectedLiteral = new SnippetLiteral(SelectedId);
        public static readonly SnippetLiteral EndLiteral = new SnippetLiteral(EndId);

        public static readonly ReadOnlyCollection<SnippetLiteral> ReservedLiterals = Array.AsReadOnly(new SnippetLiteral[] {
            EndLiteral,
            SelectedLiteral });

        private static readonly Regex _idValidatorRegex = new Regex("^[a-z][a-z_0-9]+$", RegexOptions.IgnoreCase);
        private static readonly HashSet<string> _reservedIds = new HashSet<string>(ReservedLiterals.Select(f => f.Id));

        public const string TokenDelimiter = "%";
        public const string SelectedId = "selected";
        public const string EndId = "end";
    }
}
