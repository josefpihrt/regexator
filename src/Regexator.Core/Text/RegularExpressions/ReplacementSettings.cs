// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Text.RegularExpressions
{
    public sealed class ReplacementSettings
    {
        public ReplacementSettings()
            : this(ReplacementMode.None)
        {
        }

        public ReplacementSettings(ReplacementMode mode)
            : this(mode, MatchData.InfiniteLimit)
        {
        }

        public ReplacementSettings(int limit)
            : this(ReplacementMode.None, limit)
        {
        }

        public ReplacementSettings(ReplacementMode mode, int limit)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit));

            Mode = mode;
            Limit = limit;
        }

        public ReplacementMode Mode { get; }
        public int Limit { get; }

        public static ReplacementSettings Default { get; } = new ReplacementSettings();
    }
}
