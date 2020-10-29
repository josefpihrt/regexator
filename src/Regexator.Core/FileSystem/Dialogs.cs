// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Regexator.UI;
using Regexator;

namespace Regexator.FileSystem
{
    public static class Dialogs
    {
        internal static void AddNewItem(IEnumerable<NewItemCommand> commands)
        {
            using (var frm = new NewItemForm(commands.ToArray()))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    NewItemCommand command = frm.SelectedCommand;
                    command?.Execute();
                }
            }
        }

        public static string GetNewProjectPath(string initialDirectory)
        {
            return GetNewProjectPath(initialDirectory, DefaultProjectFileName);
        }

        public static string GetNewProjectPath(string initialDirectory, string fileName)
        {
            while (true)
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.DefaultExt = FileSystemManager.ProjectExtension;
                    dlg.FileName = fileName ?? DefaultProjectFileName;
                    dlg.Filter = ProjectDialogFilter;
                    dlg.OverwritePrompt = false;
                    dlg.Title = Resources.NewProject;
                    dlg.InitialDirectory = initialDirectory;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(dlg.FileName))
                        {
                            MessageDialog.Warning(string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.ProjectFileAlreadyExistsMsg,
                                dlg.FileName));
                        }
                        else
                        {
                            return dlg.FileName;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static string GetExistingProjectPath(string initialDirectory)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.DefaultExt = FileSystemManager.ProjectExtension;
                dlg.Filter = ProjectDialogFilter;
                dlg.Title = Resources.OpenProject;
                dlg.InitialDirectory = initialDirectory;

                if (dlg.ShowDialog() == DialogResult.OK)
                    return dlg.FileName;
            }

            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static string GetNewInputPath(ProjectNode projectNode)
        {
            if (projectNode == null)
                throw new ArgumentNullException(nameof(projectNode));

            return GetNewInputPath(projectNode.DirectoryName, projectNode.FileNameWithoutExtension);
        }

        public static string GetNewInputPath(string initialDirectory, string fileName)
        {
            while (true)
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.DefaultExt = FileSystemManager.DefaultInputExtension;
                    dlg.InitialDirectory = initialDirectory;
                    dlg.FileName = fileName;
                    dlg.Filter = InputDialogFilter;
                    dlg.OverwritePrompt = false;
                    dlg.Title = Resources.NewFileInput;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(dlg.FileName))
                        {
                            MessageDialog.Warning(
                                string.Format(CultureInfo.CurrentCulture, Resources.FileAlreadyExistsMsg, dlg.FileName));
                        }
                        else
                        {
                            return dlg.FileName;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static string[] GetExistingInputPaths(ProjectNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            StringComparer comparer = FileSystemUtility.ComparerIgnoreCase;
            while (true)
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.DefaultExt = FileSystemManager.DefaultInputExtension;
                    dlg.InitialDirectory = node.DirectoryName;
                    dlg.FileName = node.FileNameWithoutExtension;
                    dlg.Filter = InputDialogFilter;
                    dlg.Title = Resources.OpenInput;
                    dlg.Multiselect = true;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        if (dlg.FileNames.Contains(node.FullName, comparer))
                        {
                            MessageDialog.Warning(Resources.ProjectCannotContainItselfAsInputMsg);
                            continue;
                        }

                        string[] intersect = dlg
                            .FileNames
                            .Intersect(node.Project.FileInputs.Select(f => f.Name), comparer)
                            .ToArray();

                        if (intersect.Length > 0)
                        {
                            MessageDialog.Warning(string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.FileIsAlreadyPartOfProjectMsg,
                                intersect[0]));
                            continue;
                        }

                        return dlg.FileNames;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public const string ProjectDialogFilter = "Project Files (*."
            + FileSystemManager
                .ProjectExtension
            + ")|*."
            + FileSystemManager.ProjectExtension;

        public const string InputDialogFilter = "All files (*.*)|*.*";
        public const string DefaultProjectFileName = "Project_01";
    }
}
