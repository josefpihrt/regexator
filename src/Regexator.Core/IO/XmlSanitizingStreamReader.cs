// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

namespace Regexator.IO
{
    public class XmlSanitizingStreamReader : StreamReader
    {
        private const int EOF = -1;

        public XmlSanitizingStreamReader(Stream stream)
            : base(stream)
        {
        }

        public XmlSanitizingStreamReader(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        public XmlSanitizingStreamReader(string path)
            : base(path)
        {
        }

        public XmlSanitizingStreamReader(string path, Encoding encoding)
            : base(path, encoding)
        {
        }

        public static bool IsValidXmlChar(int character)
        {
            //version 1.0 http://www.w3.org/TR/REC-xml/#charsets
            return
                character == 0x9
                    || character == 0xA
                    || character == 0xD
                    || (character >= 0x20 && character <= 0xD7FF)
                    || (character >= 0xE000 && character <= 0xFFFD)
                    || (character >= 0x10000 && character <= 0x10FFFF);

            //version 1.1 http://www.w3.org/TR/xml11/#charsets
            //return
            //!(
            //    character <= 0x8 ||
            //    character == 0xB ||
            //    character == 0xC ||
            //    (character >= 0xE && character <= 0x1F) ||
            //    (character >= 0x7F && character <= 0x84) ||
            //    (character >= 0x86 && character <= 0x9F) ||
            //    character > 0x10FFFF
            //);
        }

        public override int Read()
        {
            int ch;
            do
            {
                if ((ch = base.Read()) == EOF)
                    break;

            } while (!IsValidXmlChar(ch));

            return ch;
        }

        public override int Peek()
        {
            int ch;
            do
            {
                ch = base.Peek();

            } while
            (
                !IsValidXmlChar(ch) && (ch = base.Read()) != EOF
            );
            return ch;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if ((buffer.Length - index) < count)
                throw new ArgumentException();

            int num = 0;
            do
            {
                int num2 = Read();

                if (num2 == -1)
                    return num;

                buffer[index + num++] = (char)num2;

            } while (num < count);
            return num;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            int num;
            int num2 = 0;
            do
            {
                num = Read(buffer, index + num2, count - num2);
                num2 += num;

            } while ((num > 0) && (num2 < count));
            return num2;
        }

        public override string ReadLine()
        {
            var builder = new StringBuilder();
            while (true)
            {
                int num = Read();
                switch (num)
                {
                    case -1:
                        {
                            if (builder.Length > 0)
                                return builder.ToString();

                            return null;
                        }
                    case 13:
                    case 10:
                        {
                            if ((num == 13) && (Peek() == 10))
                                Read();

                            return builder.ToString();
                        }
                }

                builder.Append((char)num);
            }
        }

        public override string ReadToEnd()
        {
            int num;
            var buffer = new char[0x1000];
            var builder = new StringBuilder(0x1000);

            while ((num = Read(buffer, 0, buffer.Length)) != 0)
                builder.Append(buffer, 0, num);

            return builder.ToString();
        }
    }
}