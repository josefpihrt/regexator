// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Regexator
{
    public class ErrorInfo
    {
        public ErrorInfo(string name, Exception exception)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        [LocalizedDisplayName("Item")]
        public virtual string Name
        {
            get { return _name; }
        }

        [LocalizedDisplayName("Message")]
        public string Message
        {
            get { return Exception.GetBaseException().Message; }
        }

        [LocalizedDisplayName("Path")]
        public virtual string FullName
        {
            get { return ""; }
        }

        [LocalizedDisplayName("Comment")]
        public virtual string Comment
        {
            get { return ""; }
        }

        [Browsable(false)]
        public Exception Exception { get; }

        private readonly string _name;
    }
}
