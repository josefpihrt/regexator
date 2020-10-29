// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Pihrtsoft.Text.RegularExpressions.Linq;

namespace Regexator.Text
{
    public static class PatternLibrary
    {
        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern LineLeadingWhiteSpace = Patterns.Options(
            RegexOptions.Multiline,
            Patterns.BeginInputOrLine().WhiteSpaceExceptNewLine().OneMany());

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern LineTrailingWhiteSpace = Patterns.Options(
            RegexOptions.Multiline,
            Patterns.WhiteSpaceExceptNewLine().OneMany().EndInputOrLine(true));

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern LineLeadingTrailingWhiteSpace = Patterns
            .Options(
                RegexOptions.Multiline,
                Patterns.BeginInputOrLine().WhiteSpaceExceptNewLine().OneMany(),
                Patterns.WhiteSpaceExceptNewLine().OneMany().EndInputOrLine(true));

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern NewLine = Patterns.NonbacktrackingGroup(
            Patterns.NewLine());

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern FirstLine = Patterns.BeginInput()
            .WhileNotNewLineChar();

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern LinefeedWithoutCarriageReturn = Patterns
            .NotAssertBack(Patterns.CarriageReturn())
            .Linefeed();

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern EmptyOrWhiteSpaceLine = Patterns
            .BeginLine()
            .WhiteSpaceExceptNewLine()
            .MaybeMany()
            .Any(Patterns.NewLine(), Patterns.EndInput());

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern EmptyLine = Patterns.BeginLine().NewLine();

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern FirstLastEmptyLine = Patterns
            .NonbacktrackingGroup(
                Patterns.BeginInput().NewLine(),
                Patterns.NewLine().EndInput());

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern InvalidPathChars = Patterns
            .NonbacktrackingGroup(
                Path.GetInvalidPathChars().OrderBy(f => (int)f).Select(f => Patterns.Character(f)));

        public static readonly Pihrtsoft.Text.RegularExpressions.Linq.Pattern InvalidFileNameChars = Patterns
            .NonbacktrackingGroup(
                Path.GetInvalidFileNameChars().OrderBy(f => (int)f).Select(f => Patterns.Character(f)));
    }
}