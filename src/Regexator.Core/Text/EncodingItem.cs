// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regexator.Text
{
    public class EncodingItem
    {
        private readonly EncodingInfo _info;
        private Encoding _encoding;

        public EncodingItem(Encoding encoding)
        {
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public EncodingItem(EncodingInfo info)
        {
            _info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public override string ToString()
        {
            return EncodingName;
        }

        public override bool Equals(object obj)
        {
            if (obj is EncodingItem other)
                return CodePage == other.CodePage;

            return false;
        }

        public override int GetHashCode()
        {
            return CodePage.GetHashCode();
        }

        public Encoding Encoding
        {
            get { return _encoding ?? (_encoding = _info.GetEncoding()); }
        }

        public string EncodingName
        {
            get { return (_encoding != null) ? _encoding.EncodingName : _info.DisplayName; }
        }

        public int CodePage
        {
            get { return (_encoding != null) ? _encoding.CodePage : _info.CodePage; }
        }

        public static IEnumerable<EncodingItem> EnumerateItems()
        {
            return Encoding.GetEncodings()
                .OrderBy(f => f.DisplayName)
                .Select(f => new EncodingItem(f));
        }
    }
}
