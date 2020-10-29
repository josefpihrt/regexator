// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text
{
    public class SummaryElement
    {
        internal SummaryElement(
            SummaryElements element,
            string heading,
            TextSpan headingSpan,
            string value,
            TextSpan valueSpan,
            bool valueIsEmptyCaption)
        {
            Element = element;
            Heading = heading;
            HeadingSpan = headingSpan;
            Value = value;
            ValueSpan = valueSpan;
            ValueIsEmptyCaption = valueIsEmptyCaption;
        }

        public SummaryElements Element { get; }

        public string Heading { get; }

        public TextSpan HeadingSpan { get; }

        public int HeadingIndex
        {
            get { return HeadingSpan.Index; }
        }

        public string Value { get; }

        public TextSpan ValueSpan { get; }

        public int ValueIndex
        {
            get { return ValueSpan.Index; }
        }

        public bool ValueIsEmptyCaption { get; }
    }
}
