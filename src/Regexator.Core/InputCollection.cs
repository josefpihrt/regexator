// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regexator
{
    [Serializable]
    public class InputCollection : Collection<Input>
    {
        public InputCollection()
        {
        }

        public InputCollection(IList<Input> list)
            : base(list)
        {
        }

        public void AddRange(IEnumerable<Input> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (Input item in items)
                Add(item);
        }
    }
}
