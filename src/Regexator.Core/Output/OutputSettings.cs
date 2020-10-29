// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Regexator.Text;

namespace Regexator.Output
{
    public class OutputSettings : ICloneable
    {
        public OutputSettings()
        {
            Captions = new Captions();
            Symbols = new Symbols();
            PrefixSeparator = ":";
            NumberAlignment = TextAlignment.Right;
            AddInfo = true;
        }

        public object Clone()
        {
            return new OutputSettings()
            {
                Captions = (Captions)Captions.Clone(),
                Symbols = (Symbols)Symbols.Clone(),
                NumberAlignment = NumberAlignment,
                PrefixSeparator = PrefixSeparator,
                OmitRepeatedInfo = OmitRepeatedInfo,
                UseCarriageReturnSymbol = UseCarriageReturnSymbol,
                UseLinefeedSymbol = UseLinefeedSymbol,
                UseSpaceSymbol = UseSpaceSymbol,
                UseTabSymbol = UseTabSymbol,
                AddInfo = AddInfo
            };
        }

        public string PrefixSeparator
        {
            get { return _prefixSeparator; }
            set { _prefixSeparator = value ?? throw new ArgumentNullException("value"); }
        }

        public string NewLine
        {
            get { return _newLine ?? Environment.NewLine; }
            set { _newLine = value; }
        }

        public Symbols Symbols { get; private set; }
        public Captions Captions { get; private set; }
        public bool AddInfo { get; set; }
        public bool UseLinefeedSymbol { get; set; }
        public bool UseCarriageReturnSymbol { get; set; }
        public bool UseTabSymbol { get; set; }
        public bool UseSpaceSymbol { get; set; }
        public TextAlignment NumberAlignment { get; set; }
        public bool OmitRepeatedInfo { get; set; }

        private string _prefixSeparator;
        private string _newLine;
    }
}
