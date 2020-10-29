// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text;

namespace Regexator
{
    public static class ExceptionExtensions
    {
        public static string CreateLog(this Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            var sb = new StringBuilder();
            CreateLog(ex, 0, sb);
            sb.AppendLine("***  END OF EXCEPTION LOG ***");
            return sb.ToString();
        }

        private static void CreateLog(Exception ex, int indentLevel, StringBuilder sb)
        {
            var indent = "";

            if (indentLevel > 0)
                indent = new string(' ', indentLevel * 2);

            sb.Append(indent);
            sb.Append("type: ");
            sb.AppendLine(ex.GetType().ToString());

            if (!string.IsNullOrEmpty(ex.Message))
            {
                sb.Append(indent);
                sb.Append("message:");
                if (ex.Message.Contains('\n'))
                {
                    sb.AppendLine();
                    sb.AppendLine(ex.Message.SetLineIndent(indent + " "));
                }
                else
                {
                    sb.Append(' ').AppendLine(ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                sb.Append(indent);
                sb.AppendLine("stack trace:");
                sb.AppendLine(ex.StackTrace.SetLineIndent(indent + " "));
            }

            if (ex is AggregateException agg)
            {
                indentLevel++;

                foreach (Exception innerException in agg.InnerExceptions)
                    CreateLog(innerException, indentLevel, sb);

                indentLevel--;
            }
            else if (ex.InnerException != null)
            {
                CreateLog(ex.InnerException, indentLevel++, sb);
            }
        }
    }
}
