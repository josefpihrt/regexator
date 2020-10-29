// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;

namespace Regexator.FileSystem
{
    public class SuffixGenerator
    {
        public SuffixGenerator()
        {
            _count = 1;
        }

        public void Increment()
        {
            _count++;
        }

        public virtual string Suffix
        {
            get
            {
                if (_count > 1)
                    return " (" + _count.ToString(CultureInfo.CurrentCulture) + ")";

                return "";
            }
        }

        private int _count;
    }
}
