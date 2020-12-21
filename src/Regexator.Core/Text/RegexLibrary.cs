// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Regexator.Text
{
    public static class RegexLibrary
    {
        public static readonly Regex LineLeadingWhiteSpace = new Regex(
            @"
^
[\s-[\r\n]]+
",
            RegexOptions.Multiline);

        public static readonly Regex LineTrailingWhiteSpace = new Regex(
            @"
[\s-[\r\n]]+
(?=
    \r?
    $
)
",
            RegexOptions.Multiline);

        public static readonly Regex LineLeadingTrailingWhiteSpace = new Regex(
            @"
(
    ^
    [\s-[\r\n]]+
|
    [\s-[\r\n]]+
    (?=
        \r?
        $
    )
)
",
            RegexOptions.Multiline);

        public static readonly Regex NewLine = new Regex(@"
(?>
    \r?
    \n
)
");

        public static readonly Regex FirstLine = new Regex(@"
\A
[^\r\n]*
");

        public static readonly Regex LinefeedWithoutCarriageReturn = new Regex(@"
(?<!
    \r
)
\n
");

        public static readonly Regex EmptyOrWhiteSpaceLine = new Regex(
            @"
^
[\s-[\r\n]]*
(?:
    (?:
        \r?
        \n
    )
|
    \z
)
",
            RegexOptions.Multiline);

        public static readonly Regex EmptyLine = new Regex(
            @"
^
(?:
    \r?
    \n
)
",
            RegexOptions.Multiline);

        public static readonly Regex FirstLastEmptyLine = new Regex(@"
(?>
    \A
    (?:
        \r?
        \n
    )
|
    (?:
        \r?
        \n
    )
    \z
)
");

        public static readonly Regex InvalidPathChar = new Regex(@"
(?>
    \x00
|
    \x01
|
    \x02
|
    \x03
|
    \x04
|
    \x05
|
    \x06
|
    \a
|
    \x08
|
    \t
|
    \n
|
    \v
|
    \f
|
    \r
|
    \x0E
|
    \x0F
|
    \x10
|
    \x11
|
    \x12
|
    \x13
|
    \x14
|
    \x15
|
    \x16
|
    \x17
|
    \x18
|
    \x19
|
    \x1A
|
    \e
|
    \x1C
|
    \x1D
|
    \x1E
|
    \x1F
|
    ""
|
    <
|
    >
|
    \|
)
");

        public static readonly Regex InvalidFileNameChar = new Regex(@"
(?>
    \x00
|
    \x01
|
    \x02
|
    \x03
|
    \x04
|
    \x05
|
    \x06
|
    \a
|
    \x08
|
    \t
|
    \n
|
    \v
|
    \f
|
    \r
|
    \x0E
|
    \x0F
|
    \x10
|
    \x11
|
    \x12
|
    \x13
|
    \x14
|
    \x15
|
    \x16
|
    \x17
|
    \x18
|
    \x19
|
    \x1A
|
    \e
|
    \x1C
|
    \x1D
|
    \x1E
|
    \x1F
|
    ""
|
    \*
|
    /
|
    :
|
    <
|
    >
|
    \?
|
    \\
|
    \|
)
");
    }
}