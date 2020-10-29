// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace Regexator.FileSystem
{
    public class CopySuffixGenerator
    {
        public CopySuffixGenerator()
        {
            CopyString = " - Copy";
        }

        public void Increment()
        {
            _count++;
        }

        public string Suffix
        {
            get
            {
                if (_count == 0)
                    return "";

                if (_count == 1)
                    return CopyString;

                return CopyString + " (" + _count.ToString(CultureInfo.CurrentCulture) + ")";
            }
        }

        public string CopyString
        {
            get { return _copyString; }
            set { _copyString = value ?? throw new ArgumentNullException("value"); }
        }

        private int _count;
        private string _copyString;
    }
}
