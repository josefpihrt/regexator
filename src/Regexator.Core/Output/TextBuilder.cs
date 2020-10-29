// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public static class TextBuilder
    {
        public static string Match(MatchItemCollection items)
        {
            return Match(items, new OutputSettings());
        }

        public static string Match(MatchItemCollection items, OutputSettings settings)
        {
            return Match(items, settings, new GroupSettings());
        }

        public static string Match(MatchItemCollection items, GroupSettings groupSettings)
        {
            return Match(items, new OutputSettings(), groupSettings);
        }

        public static string Match(MatchItemCollection items, OutputSettings settings, GroupSettings groupSettings)
        {
            var builder = new MatchTextBuilder(items, settings, groupSettings);
            return string.Join(settings.NewLine, new MatchTextIterator(builder));
        }
    }
}
