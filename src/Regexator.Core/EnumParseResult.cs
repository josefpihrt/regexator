// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Regexator
{
    internal class EnumParseResult<T> where T : struct
    {
        public EnumParseResult()
        {
            _unknown = new List<string>();
        }

        public IEnumerable<T> ParseValues(string input)
        {
            _unknown.Clear();
            if (!string.IsNullOrEmpty(input))
            {
                foreach (string value in input
                    .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim()))
                {
                    if (Enum.TryParse(value, out T result))
                    {
                        yield return result;
                    }
                    else
                    {
                        _unknown.Add(value);
                    }
                }
            }
        }

        public IEnumerable<string> UnknownValues
        {
            get { return _unknown.Select(f => f); }
        }

        private readonly List<string> _unknown;
    }
}
