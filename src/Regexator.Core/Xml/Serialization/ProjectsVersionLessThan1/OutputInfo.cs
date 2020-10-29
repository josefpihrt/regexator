// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
{
    public class OutputInfo
    {
        public static OutputInfo ToSerializable(Regexator.OutputInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new OutputInfo() { Options = item.Options.ToString() };
        }

        public static Regexator.OutputInfo FromSerializable(OutputInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.OutputInfo(item.ParseOptions());
        }

        public OutputOptions ParseOptions()
        {
            if (Enum.TryParse(Options, out OutputOptions value))
                return value;

            return EnumHelper.ParseOutputOptions(Options);
        }

        public string Options { get; set; }
        public string SortDirection { get; set; }
    }
}
