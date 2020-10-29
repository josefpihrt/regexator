// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

namespace Regexator
{
    public static class MessageDialog
    {
        public const string DefaultConfirmCaption = "Confirm";
        public const string DefaultInfoCaption = "Info";
        public const string DefaultQuestionCaption = "Question";
        public const string DefaultWarningCaption = "Warning";
        public const string DefaultErrorCaption = "Error";
        public const string DefaultErrorMessage = "Ooops, an error occured.";

        private static string _confirmCaption = DefaultConfirmCaption;
        private static string _infoCaption = DefaultInfoCaption;
        private static string _questionCaption = DefaultQuestionCaption;
        private static string _warningCaption = DefaultWarningCaption;
        private static string _errorCaption = DefaultErrorCaption;
        private static string _errorMessage = DefaultErrorMessage;

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static DialogResult Question(string text)
        {
            return MessageBox.Show(text, QuestionCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static bool QuestionYesNo(string text)
        {
            return MessageBox.Show(
                text,
                QuestionCaption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static void Info(string text)
        {
            MessageBox.Show(text, InfoCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void InfoIf(bool condition, string text)
        {
            if (condition)
                Info(text);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static void Warning(string text)
        {
            MessageBox.Show(text, WarningCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void WarningIf(bool condition, string text)
        {
            if (condition)
                Warning(text);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static void Warning(string text, Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            MessageBox.Show(text + "\n\n" + ex.Message, WarningCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static bool ConfirmDelete(string item)
        {
            return ConfirmDelete(item, false);
        }

        public static bool ConfirmDelete(string item, bool hasContent)
        {
            if (hasContent)
            {
                return Confirm(
                    string.Format(CultureInfo.CurrentCulture, Resources._0_AndAllItsContentWillBeDeletedPermanently, item));
            }
            else
            {
                return Confirm(string.Format(CultureInfo.CurrentCulture, Resources._0_WillBeDeletedPermanently, item));
            }
        }

        public static bool Confirm(string text)
        {
            return Confirm(text, ConfirmCaption);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static bool Confirm(string text, string caption)
        {
            return MessageBox.Show(
                text,
                caption,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation) == DialogResult.OK;
        }

        public static void Err(Exception ex)
        {
            Err(ErrorMessage, ex);
        }

        public static void Err(string text, Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            Err(text + "\n\n" + ex.Message);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static void Err(string text)
        {
            MessageBox.Show(text, ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string MasterCaption { get; set; }

        public static string ConfirmCaption
        {
            get { return (string.IsNullOrEmpty(MasterCaption)) ? _confirmCaption : MasterCaption; }
            set { _confirmCaption = (string.IsNullOrEmpty(value)) ? DefaultConfirmCaption : value; }
        }

        public static string InfoCaption
        {
            get { return (string.IsNullOrEmpty(MasterCaption)) ? _infoCaption : MasterCaption; }
            set { _infoCaption = (string.IsNullOrEmpty(value)) ? DefaultInfoCaption : value; }
        }

        public static string QuestionCaption
        {
            get { return (string.IsNullOrEmpty(MasterCaption)) ? _questionCaption : MasterCaption; }
            set { _questionCaption = (string.IsNullOrEmpty(value)) ? DefaultQuestionCaption : value; }
        }

        public static string WarningCaption
        {
            get { return (string.IsNullOrEmpty(MasterCaption)) ? _warningCaption : MasterCaption; }
            set { _warningCaption = (string.IsNullOrEmpty(value)) ? DefaultWarningCaption : value; }
        }

        public static string ErrorCaption
        {
            get { return (string.IsNullOrEmpty(MasterCaption)) ? _errorCaption : MasterCaption; }
            set { _errorCaption = (string.IsNullOrEmpty(value)) ? DefaultErrorCaption : value; }
        }

        public static string ErrorMessage
        {
            get { return (string.IsNullOrEmpty(MasterCaption)) ? _errorMessage : MasterCaption; }
            set { _errorMessage = (string.IsNullOrEmpty(value)) ? DefaultErrorMessage : value; }
        }
    }
}
