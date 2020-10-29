// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Regexator.Text
{
    [DebuggerDisplay("Index: {Index}, Length: {Length}")]
    public class TextSpan : IEquatable<TextSpan>
    {
        public static readonly TextSpan Empty = new TextSpan(0, 0);

        protected TextSpan()
        {
        }

        public TextSpan(int index)
            : this(index, 0)
        {
        }

        public TextSpan(int index, int length)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Index = index;
            Length = length;
        }

        public TextSpan(Capture capture)
        {
            if (capture == null)
                throw new ArgumentNullException(nameof(capture));

            Index = capture.Index;
            Length = capture.Length;
        }

        public virtual TextSpan Offset(int count)
        {
            return new TextSpan(Index + count, Length);
        }

        public virtual TextSpan Extend(int count)
        {
            return new TextSpan(Index, Length + count);
        }

        public virtual TextSpan Combine(TextSpan textSpan)
        {
            if (textSpan == null)
                throw new ArgumentNullException(nameof(textSpan));

            int index = Math.Min(Index, textSpan.Index);
            int endIndex = Math.Max(EndIndex, textSpan.EndIndex);
            return new TextSpan(index, endIndex - index);
        }

        public bool Contains(int index)
        {
            return (Length == 0)
                ? index == Index
                : index >= Index && index < EndIndex;
        }

        public TextSpan ExtendToEntireLine(string text)
        {
            return TextUtility.ExtendToEntireLine(text, Index, Length);
        }

        public bool Equals(TextSpan other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return (Index == other.Index) && (Length == other.Length);
        }

        public override bool Equals(object obj)
        {
            var other = obj as TextSpan;
            return Equals(other);
        }

        public static bool operator ==(TextSpan left, TextSpan right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(TextSpan left, TextSpan right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Index ^ Length;
        }

        public int EndIndex
        {
            get { return Index + Length; }
        }

        public int Index { get; }

        public int Length { get; }
    }
}
