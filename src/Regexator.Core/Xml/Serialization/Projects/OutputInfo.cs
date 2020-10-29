// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using Regexator.Collections.Generic;

namespace Regexator.Xml.Serialization.Projects
{
    public class OutputInfo
    {
        public static OutputInfo ToSerializable(Regexator.OutputInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new OutputInfo()
            {
                Options = SerializeOptions(item),
                Groups = GroupSettings.ToSerializable(item.Groups)
            };
        }

        private static string SerializeOptions(Regexator.OutputInfo item)
        {
            if (item.UnknownOptions.Count > 0)
            {
                if (item.Options == OutputOptions.None)
                    return string.Join(", ", item.UnknownOptions);

                return item.Options.ToString() + ", " + string.Join(", ", item.UnknownOptions);
            }

            return item.Options.ToString();
        }

        public static Regexator.OutputInfo FromSerializable(OutputInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var info = new Regexator.OutputInfo()
            {
                Groups = (item.Groups != null)
                    ? GroupSettings.FromSerializable(item.Groups)
                    : new global::Regexator.Text.RegularExpressions.GroupSettings()
            };

            ParseOptions(item, info);
            return info;
        }

        private static void ParseOptions(OutputInfo item, Regexator.OutputInfo info)
        {
            if (Enum.TryParse(item.Options, out OutputOptions options))
            {
                info.Options = options;
            }
            else
            {
                var result = new EnumParseResult<OutputOptions>();
                info.Options = result.ParseValues(item.Options).GetValue();
                info.UnknownOptions.AddItems(result.UnknownValues);
            }
        }

        public string Options { get; set; }

        [XmlElement("Groups")]
        public GroupSettings Groups { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1041:ProvideObsoleteAttributeMessage")]
        [Obsolete]
        public string SortDirection { get; set; }
    }
}