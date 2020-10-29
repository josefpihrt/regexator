// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Xml
{
    public static class XmlUtility
    {
        public const string CDataEnd = "]]>";
        public const string CDataEndEncoded = "&rsqb;&rsqb;&gt;";

        public static string EncodeCDataEnd(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Replace(CDataEnd, CDataEndEncoded);
        }

        public static string DecodeCDataEnd(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Replace(CDataEndEncoded, CDataEnd);
        }
    }
}