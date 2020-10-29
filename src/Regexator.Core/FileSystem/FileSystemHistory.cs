// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Regexator.IO;

namespace Regexator.FileSystem
{
    public class FileSystemHistory : CommandHistory
    {
        protected override void OnUndoing(UndoEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Command is FileSystemCommand command)
            {
                FileSystemInfo info = command.FileSystemInfo;
                ProcessUndoEventArgs(info, e);
            }

            base.OnUndoing(e);
        }

        protected override void OnRedoing(UndoEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Command is FileSystemCommand command)
            {
                FileSystemInfo info = command.FileSystemInfo;
                ProcessUndoEventArgs(info, e);
            }

            base.OnRedoing(e);
        }

        private static void ProcessUndoEventArgs(FileSystemInfo info, UndoEventArgs e)
        {
            if (!info.ExistsNow())
            {
                e.Cancel = true;
                if (MessageDialog.Question(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ItemNotFoundRemoveItemMsg,
                        info.FullName)) == DialogResult.Yes)
                {
                    e.Remove = true;
                }
            }
        }

        public IEnumerable<ToolStripMenuItem> CreateUndoToolStripMenuItems(int count)
        {
            return CreateRedoToolStripMenuItems(UndoItems, count, f => Undo(f));
        }

        public IEnumerable<ToolStripMenuItem> CreateRedoToolStripMenuItems(int count)
        {
            return CreateRedoToolStripMenuItems(RedoItems, count, f => Redo(f));
        }

        private static IEnumerable<ToolStripMenuItem> CreateRedoToolStripMenuItems(
            IEnumerable<ICommand> commands,
            int count,
            Action<int> action)
        {
            int i = 1;
            foreach (FileSystemCommand command in commands.Take(count).Cast<FileSystemCommand>())
            {
                int index = i;
                var tsi = new ToolStripMenuItem(command.Name, command.Image, (object sender, EventArgs e) => action(index));

                if (command.IsToolTipTextNeeded)
                    tsi.ToolTipText = command.FullName;

                yield return tsi;
                i++;
            }
        }
    }
}
