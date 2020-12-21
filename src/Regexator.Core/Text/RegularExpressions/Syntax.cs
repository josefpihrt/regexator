// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text.RegularExpressions
{
    internal static class Syntax
    {
        public const string InlineCommentStart = "(?#";
        public const string Or = "|";
        public const string BeginningOfInput = @"\A";
        public const string BeginningOfInputOrLine = "^";
        public const string EndOfInput = @"\z";
        public const string EndOfInputOrLine = "$";
        public const string EndOfInputOrBeforeEndingLinefeed = @"\Z";
        public const string WordBoundary = @"\b";
        public const string NegativeWordBoundary = @"\B";
        public const string PreviousMatchEnd = @"\G";
        public const string AssertionStart = "(?=";
        public const string NegativeAssertionStart = "(?!";
        public const string BackAssertionStart = "(?<=";
        public const string NegativeBackAssertionStart = "(?<!";
        public const string NoncapturingGroupStart = "(?:";
        public const string NonbacktrackingGroupStart = "(?>";
        public const string GroupEnd = ")";
        public const string AnyChar = ".";
        public const string Digit = @"\d";
        public const string NotDigit = @"\D";
        public const string WhiteSpace = @"\s";
        public const string NotWhiteSpace = @"\S";
        public const string WordChar = @"\w";
        public const string NotWordChar = @"\W";
        public const string Bell = @"\a";
        public const string Tab = @"\t";
        public const string Linefeed = @"\n";
        public const string VerticalTab = @"\v";
        public const string FormFeed = @"\f";
        public const string CarriageReturn = @"\r";
        public const string Escape = @"\e";
        public const string CharGroupNegation = "^";
        public const string CharGroupStart = "[";
        public const string CharGroupEnd = "]";
        public const string AsciiHexadecimalStart = @"\x";
        public const string AsciiControlStart = @"\c";
        public const string UnicodeHexadecimalStart = @"\u";
        public const string UnicodeStart = @"\p{";
        public const string NotUnicodeStart = @"\P{";
        public const string UnicodeEnd = "}";
        public const string Maybe = "?";
        public const string MaybeMany = "*";
        public const string OneMany = "+";
        public const string Lazy = "?";
        public const char IgnoreCaseChar = 'i';
        public const char MultilineChar = 'm';
        public const char ExplicitCaptureChar = 'n';
        public const char SinglelineChar = 's';
        public const char IgnorePatternWhiteSpaceChar = 'x';
        public const string SubstituteLastCapturedGroup = "$+";
        public const string SubstituteEntireInput = "$_";
        public const string SubstituteEntireMatch = "$&";
        public const string SubstituteAfterMatch = "$'";
        public const string SubstituteBeforeMatch = "$`";
        public const string SubstituteNamedGroupStart = "${";
        public const string SubstituteNamedGroupEnd = "}";
    }
}
