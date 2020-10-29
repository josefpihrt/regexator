// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Text
{
    public sealed class Indenter
    {
        private int _spaceCount;

        public const int DefaultSpaceCount = 4;

        public Indenter()
            : this(DefaultSpaceCount)
        {
        }

        public Indenter(int spaceCount)
        {
            SpaceCount = spaceCount;
        }

        public string RemoveLineIndent(string line)
        {
            int index = line.GetIndentLength();
            int index2 = GetLastIndent(line, index);

            return line.Remove(index2, index - index2);
        }

        public int GetLastIndent(string input, int index)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (index < 0 || index > input.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int k = 0;
            for (int i = 0, d = 0; i < index && i < input.Length; i++)
            {
                if (input[i] != ' ' && input[i] != '\t')
                {
                    k = i + 1;
                }
                else if (d % SpaceCount == 0)
                {
                    k = i;
                }

                if (input[i] == '\t')
                {
                    d = (d / SpaceCount * SpaceCount) + SpaceCount;
                }
                else
                {
                    d++;
                }
            }

            return k;
        }

        public int SpaceCount
        {
            get { return _spaceCount; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");

                _spaceCount = value;
            }
        }
    }
}
