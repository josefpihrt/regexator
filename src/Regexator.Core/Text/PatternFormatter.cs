// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Regexator.Text
{
    public class PatternFormatter
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public string Format(string input, string tab)
        {
            _s = input ?? throw new ArgumentNullException(nameof(input));
            _tab = tab ?? throw new ArgumentNullException(nameof(tab));
            using (_sw = new StringWriter(CultureInfo.CurrentCulture))
            {
                _sw.NewLine = "\n";
                var flgOk = false;

                try
                {
                    flgOk = Execute("");
                }
                catch (Exception)
                {
                    flgOk = false;
                }

                if (!_zauZav)
                    _sw.WriteLine();

                if (!flgOk)
                    _sw.WriteLine("ERROR: see output");
            }

            return _sw.ToString();
        }

        private void NextIndex()
        {
            do
            {
                _i++;

            } while (_i < _s.Length && char.IsWhiteSpace(_s[_i]));
        }

        private bool Test(string a)
        {
            int k = 0;
            for (int j = _i + 1; j < _s.Length; j++)
            {
                if (!char.IsWhiteSpace(_s[j]))
                {
                    if (_s[j] != a[k])
                        return false;

                    k++;

                    if (k == a.Length)
                        return true;
                }
            }

            return false;
        }

        private bool Execute(string tab)
        {
            for (; _i < _s.Length; NextIndex())
            {
                if (_s[_i] == '(')
                {
                    if (_fifex)
                    {
                        _fifex = false;
                    }
                    else
                    {
                        if (_zauZav)
                        {
                            _zauZav = false;
                        }
                        else
                        {
                            _sw.WriteLine();
                        }

                        _sw.Write(tab);
                    }

                    _sw.Write(_s[_i]);

                    if (Test("?("))
                        _fifex = true;

                    _topZav = true;
                    NextIndex();
                    Execute(tab + _tab);

                    if (_zauZav)
                    {
                        _sw.Write(tab);
                    }
                    else if (!_topZav)
                    {
                        _sw.WriteLine();
                        _sw.Write(tab);
                    }

                    _sw.Write(_s[_i]);

                    if (Test("+") || Test("*") || Test("?") || Test("{"))
                    {
                        NextIndex();
                        if (_s[_i] == '{')
                        {
                            int j = _s.Substring(_i).IndexOf('}');
                            _sw.Write(_s.Substring(_i, j + 1));
                            _i += j;
                        }
                        else
                        {
                            _sw.Write(_s[_i]);
                        }

                        if (Test("?"))
                        {
                            NextIndex();
                            _sw.Write(_s[_i]);
                        }
                    }

                    _topZav = false;
                    _zauZav = true;
                    _sw.WriteLine();
                }
                else if (_s[_i] == ')')
                {
                    return false;
                }
                else
                {
                    if (_zauZav)
                    {
                        _zauZav = false;
                        _sw.Write(tab);
                    }

                    if (_s[_i] == '\\')
                    {
                        _sw.Write(_s[_i]);
                        _i++;
                        _sw.Write(_s[_i]);
                    }
                    else if (_s[_i] == '[')
                    {
                        for (int u = 0; _i < _s.Length; _i++)
                        {
                            _sw.Write(_s[_i]);
                            if (_s[_i] == '\\')
                            {
                                _i++;
                                _sw.Write(_s[_i]);
                            }
                            else if (_s[_i] == '[')
                            {
                                u++;
                            }
                            else if (_s[_i] == ']')
                            {
                                u--;
                                if (u == 0)
                                    break;
                            }
                        }
                    }
                    else
                    {
                        _sw.Write(_s[_i]);
                    }
                }
            }

            return true;
        }

        private bool _topZav = true;
        private bool _zauZav = true;
        private bool _fifex;
        private int _i;
        private string _s;
        private string _tab;
        private StringWriter _sw;
    }
}
