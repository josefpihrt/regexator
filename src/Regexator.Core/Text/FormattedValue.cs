// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Regexator.Output;

namespace Regexator.Text
{
    public class FormattedValue
    {
        public FormattedValue(string value, OutputSettings settings)
        {
            Value = value;
            _settings = settings;
        }

        public override string ToString()
        {
            return _formattedValue ?? (_formattedValue = TextProcessor.ProcessSymbols(Value, _settings));
        }

        public string Value { get; }

        private readonly OutputSettings _settings;
        private string _formattedValue;
    }
}
