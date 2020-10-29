// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Regexator.Text;
using Regexator.Xml.Serialization;

namespace Regexator
{
    public class RegexOptionsCollection : ReadOnlyCollection<RegexOptionsItem>
    {
        internal RegexOptionsCollection()
            : base(RegexOptionsUtility.Values.Select(f => new RegexOptionsItem(f)).ToArray())
        {
            _dic = Items.ToDictionary(f => f.Value, f => f);
            Array.ForEach(_items, (f) => _dic[f.Value].Description = f.Description);
        }

        public void ForEach(Action<RegexOptionsItem> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (RegexOptionsItem item in Items)
                action(item);
        }

        public RegexOptionsItem this[RegexOptions value]
        {
            get { return _dic[value]; }
        }

        private static IEnumerable<RegexOptionsItem> CreateItems()
        {
            foreach (Xml.Serialization.RegexOptionsItem item in XmlSerializationManager
                .DeserializeText<Xml.Serialization.RegexOptionsItem[]>(Resources.XmlRegexOptionsItems))
            {
                yield return Xml.Serialization.RegexOptionsItem.FromSerializable(item);
            }
        }

        private readonly Dictionary<RegexOptions, RegexOptionsItem> _dic;

        private static readonly RegexOptionsItem[] _items = CreateItems().ToArray();
    }
}
