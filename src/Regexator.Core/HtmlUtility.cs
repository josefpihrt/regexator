// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;

namespace Regexator
{
    public static class HtmlUtility
    {
        public static string DownloadDocument(Uri url)
        {
            return DownloadDocument(url, Encoding.UTF8);
        }

        public static string DownloadDocument(Uri url, Encoding encoding)
        {
            return DownloadDocument(url, encoding, 10000);
        }

        public static string DownloadDocument(Uri url, Encoding encoding, int timeout)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (timeout < 0 && timeout != Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(timeout));

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream(), encoding))
                    return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (ex is WebException
                    || ex is IOException
                    || ex is InvalidCastException
                    || ex is InvalidOperationException
                    || ex is ProtocolViolationException
                    || ex is SecurityException
                    || ex is NotSupportedException)
                {
                    throw new InvalidOperationException(null, ex);
                }

                throw;
            }
        }
    }
}
