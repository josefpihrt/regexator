// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public abstract class RegexBuilder
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract string GetText();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract DataTable GetTable();

        public abstract OutputSettings Settings { get; }
        public abstract int Count { get; }
        public abstract LimitState LimitState { get; }
        public abstract ReadOnlyCollection<RegexBlock> Blocks { get; }
        public abstract EvaluationMode Mode { get; }
        public abstract string Input { get; }

        public virtual int CaptureCount
        {
            get { return Count; }
        }

        public virtual int InfoLength
        {
            get { return 0; }
        }
    }
}
