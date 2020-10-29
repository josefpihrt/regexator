// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Windows.Forms;

namespace Regexator.Text
{
    public sealed class OutputSearchDefinition : SearchDefinition
    {
        public OutputSearchDefinition(TextBoxBase box, int length)
            : base(box)
        {
            Length = length;
        }

        public override IEnumerable<SearchMatch> FindAll()
        {
            foreach (SearchMatch result in base.FindAll())
            {
                if (!result.IsContained(Length))
                    yield return result;
            }
        }

        public int Length { get; }
    }
}
