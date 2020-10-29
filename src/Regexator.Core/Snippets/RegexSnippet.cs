// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.Text;

namespace Regexator.Snippets
{
    [DebuggerDisplay("{FullName}")]
    public class RegexSnippet : Snippet, INotifyPropertyChanged
    {
        public RegexSnippet(string category, string name, string code, IList<SnippetLiteral> literals)
            : base(code, literals)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (!IsValidName(category))
                throw new ArgumentException("Category has an invalid format.", nameof(category));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!IsValidName(name))
                throw new ArgumentException("Name has an invalid format.", nameof(name));

            Category = category;
            Name = name;
            FullName = category + "." + name;

            if (!Enum.TryParse(Category, out RegexCategory regexCategory))
                regexCategory = RegexCategory.Custom;

            CategoryText = TextUtility.SplitCamelCase(category);
            RegexCategory = regexCategory;
            NewLineSymbol = DefaultNewLineSymbol;
            Engines = new Collection<string>();
            Visible = true;
        }

        private RegexSnippet(RegexSnippet snippet, ExtendedSnippetInfo info)
            : base(info.Code, snippet.Literals)
        {
            Category = snippet.Category;
            Name = snippet.Name;
            FullName = snippet.FullName;
            Title = snippet.Title + " " + EnumHelper.GetDescription(info.Kind).AddBrackets(BracketKind.Square);
            CategoryText = snippet.CategoryText;
            RegexCategory = snippet.RegexCategory;
            NewLineSymbol = snippet.NewLineSymbol;
            Engines = new Collection<string>();

            foreach (string item in snippet.Engines)
                Engines.Add(item);

            Visible = snippet.Visible;
            Options = info.Options;
            CodeKind = info.Kind;
        }

        public static void Insert(RichTextBox rtb, string text)
        {
            SnippetInsertProcessor.InsertText(rtb, text);
        }

        private static bool IsValidName(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return _nameOrCategoryValidatorRegex.IsMatch(input);
        }

        public string NewLineSymbol
        {
            get { return _newLineSymbol; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_newLineSymbol != value)
                {
                    _newLineSymbol = value;
                    CodeSingleline = Code.ToSingleline(NewLineSymbol);
                    CleanCodeSingleline = SnippetLiteral.RemoveLiterals(CodeSingleline, SnippetLiteral.ReservedLiterals);
                }
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public bool Favorite
        {
            get { return _favorite; }
            set
            {
                if (_favorite != value)
                {
                    _favorite = value;
                    NotifyPropertyChanged("Favorite");
                }
            }
        }

        public string ExtendedCode
        {
            get { return ExtendedInfo?.Code; }
        }

        public string ExtendedCleanCode
        {
            get { return ExtendedSnippet?.CleanCode; }
        }

        public SnippetCodeKind ExtendedKind
        {
            get { return (ExtendedInfo != null) ? ExtendedInfo.Kind : SnippetCodeKind.None; }
        }

        public bool IsExtensible
        {
            get { return ExtendedInfo != null; }
        }

        public ExtendedSnippetInfo ExtendedInfo
        {
            get { return _extendedInfo; }
            set
            {
                _extendedInfo = value;
                _extendedSnippet = null;
            }
        }

        public RegexSnippet ExtendedSnippet
        {
            get
            {
                if (_extendedSnippet == null && ExtendedInfo != null)
                    _extendedSnippet = new RegexSnippet(this, ExtendedInfo);

                return _extendedSnippet;
            }
        }

        public string FullName { get; }
        public string Name { get; }
        public string Category { get; }
        public string CategoryText { get; }
        public RegexCategory RegexCategory { get; }
        public Collection<string> Engines { get; }
        public bool Visible { get; set; }
        public string CodeSingleline { get; private set; }
        public string CleanCodeSingleline { get; private set; }
        public SnippetCodeKind CodeKind { get; }
        public RegexOptions Options { get; set; }
        public SnippetOrigin Origin { get; set; }

        private string _newLineSymbol;
        private bool _favorite;
        private ExtendedSnippetInfo _extendedInfo;
        private RegexSnippet _extendedSnippet;

        public static readonly string DefaultNewLineSymbol = " " + ((char)0XB6).ToString() + " ";

        private static readonly Regex _nameOrCategoryValidatorRegex = new Regex(
            "^[a-z][a-z_0-9]+$",
            RegexOptions.IgnoreCase);

        public const string FileExtension = "rgxs";
        public const string SnippetSearchPattern = "*." + FileExtension;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}