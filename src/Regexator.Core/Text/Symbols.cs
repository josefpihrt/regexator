// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Text
{
    public sealed class Symbols : ICloneable
    {
        public Symbols()
        {
            CarriageReturn = DefaultCarriageReturn;
            Linefeed = DefaultLinefeed;
            NoCapture = DefaultNoCapture;
            Number = DefaultNumber;
            Tab = DefaultTab;
            Space = DefaultSpace;
        }

        public object Clone()
        {
            return new Symbols()
            {
                CarriageReturn = CarriageReturn,
                Linefeed = Linefeed,
                NoCapture = NoCapture,
                Number = Number,
                Tab = Tab,
                Space = Space
            };
        }

        public string CarriageReturn
        {
            get { return _cr; }
            set { _cr = value ?? DefaultCarriageReturn; }
        }

        public string Linefeed
        {
            get { return _lf; }
            set { _lf = value ?? DefaultLinefeed; }
        }

        public string NoCapture
        {
            get { return _noCapture; }
            set { _noCapture = value ?? DefaultNoCapture; }
        }

        public string Number
        {
            get { return _number; }
            set { _number = value ?? DefaultNumber; }
        }

        public string Tab
        {
            get { return _tab; }
            set { _tab = value ?? DefaultTab; }
        }

        public string Space
        {
            get { return _space; }
            set { _space = value ?? DefaultSpace; }
        }

        private string _cr;
        private string _lf;
        private string _noCapture;
        private string _number;
        private string _tab;
        private string _space;

        public static readonly string DefaultCarriageReturn = ((char)0x2190).ToString();
        public static readonly string DefaultLinefeed = ((char)0x2193).ToString();
        public static readonly string DefaultNoCapture = ((char)0x2205).ToString();
        public static readonly string DefaultTab = ((char)0x2192).ToString();
        public static readonly string DefaultSpace = ((char)0x02FD).ToString();

        public const string DefaultNumber = "#";
    }
}
