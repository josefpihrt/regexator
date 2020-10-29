// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using Regexator.Output;
using Regexator.Text.RegularExpressions;

namespace Regexator
{
    [Serializable]
    public class OutputInfo : ICloneable
    {
        public OutputInfo()
            : this(OutputOptions.None)
        {
        }

        public OutputInfo(OutputOptions options)
            : this(options, new GroupSettings())
        {
        }

        public OutputInfo(OutputOptions options, GroupSettings groups)
        {
            Options = options;
            Groups = groups ?? throw new ArgumentNullException(nameof(groups));
            UnknownOptions = new Collection<string>();
        }

        public static ContainerProps GetChangedProps(OutputInfo first, OutputInfo second, ContainerProps props)
        {
            if (first == null || second == null || ReferenceEquals(first, second))
                return ContainerProps.None;

            var value = ContainerProps.None;

            if (props.Contains(ContainerProps.OutputOptions) && first.Options != second.Options)
                value |= ContainerProps.OutputOptions;

            value |= GroupSettingsHelper.GetChangedProps(first.Groups, second.Groups, props);
            return value;
        }

        public object Clone()
        {
            return new OutputInfo(Options, (GroupSettings)Groups.Clone());
        }

        public GroupSettings Groups
        {
            get { return _groups; }
            set { _groups = value ?? throw new ArgumentNullException("value"); }
        }

        public Collection<string> UnknownOptions { get; }
        public OutputOptions Options { get; set; }

        private GroupSettings _groups;

        public static readonly ContainerProps AllProps = ContainerProps.OutputOptions | ContainerProps.IgnoredGroups;
    }
}
