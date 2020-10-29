// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Regexator.UI;
using Regexator.Windows.Forms;
using Regexator;

namespace Regexator.FileSystem
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class FileSystemManager : IDisposable
    {
        public FileSystemManager()
        {
            TreeView = new FileSystemTreeView(this);
            UseRecycleBin = true;
            ConfirmFileInputRemoval = true;
            KeyProcessor = new KeyProcessor(this);
            ClipboardManager = new ClipboardManager(this);
#if DEBUG
            ClipboardManager.Enabled = true;
#else
            this.ClipboardManager.Enabled = false;
#endif
            History = new FileSystemHistory();
            RecentManager = new RecentManager(this);
            SaveManager = new SaveManager(this);
        }

        public void CloseProject()
        {
            if (Save())
            {
                CurrentInputNode = null;
                CurrentProjectNode = null;
            }
        }

        public void CloseInput()
        {
            if (SaveManager.SaveInput())
                CurrentInputNode = null;
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ProjectOr")]
        public void AddNewProjectOrFolder()
        {
            Dialogs.AddNewItem(CreateNewProjectOrFolderCommands());
        }

        public void AddNewInput()
        {
            Dialogs.AddNewItem(CreateNewInputCommands());
        }

        private IEnumerable<NewItemCommand> CreateNewProjectOrFolderCommands()
        {
            yield return new NewItemCommand(Resources.Project, () => LoadNewProject(), Resources.IcoCodeNew);
            yield return new NewItemCommand(
                Resources.ProjectWithInput,
                () => LoadNewProject(InputKind.Included),
                Resources.IcoNewProjectWithInput);
            yield return new NewItemCommand(
                Resources.ProjectWithFileInput,
                () => LoadNewProject(InputKind.File),
                Resources.IcoNewProjectWithFileInput);

            if (CanAddNewSubdirectory)
                yield return new NewItemCommand(Resources.Folder, () => AddNewSubdirectory(), Resources.IcoFolder);
        }

        private IEnumerable<NewItemCommand> CreateNewInputCommands()
        {
            if (TreeView.SelectedNode is ProjectNode node)
            {
                yield return new NewItemCommand(
                    Resources.Input,
                    () => LoadNewInput(node, InputKind.Included),
                    Resources.IcoInputNew);
                yield return new NewItemCommand(
                    Resources.FileInput,
                    () => LoadNewInput(node, InputKind.File),
                    Resources.IcoFileNew);
            }
        }

        public RootNode FindRootNode(string dirPath)
        {
            return EnumerateRootNodes().FirstOrDefault(f => FileSystemUtility.PathEquals(f.FullName, dirPath));
        }

        public ProjectNode FindProjectNode(string projectPath)
        {
            return EnumerateProjectNodes().FirstOrDefault(f => FileSystemUtility.PathEquals(f.FullName, projectPath));
        }

        public FileInputNode FindFileInputNode(string projectPath, string name)
        {
            ProjectNode node = FindProjectNode(projectPath);

            if (node != null)
                return node.FindFileInputNode(name);

            return null;
        }

        public InputNode FindInputNode(string projectPath, string name)
        {
            ProjectNode node = FindProjectNode(projectPath);

            if (node != null)
                return node.FindInputNode(name);

            return null;
        }

        public IEnumerable<RootNode> EnumerateRootNodes()
        {
            return TreeView.Nodes.OfType<RootNode>();
        }

        public IEnumerable<DirectoryNode> EnumerateDirectoryNodes()
        {
            return EnumerateRootNodes().SelectMany(f => f.Nodes.OfType<DirectoryNode>());
        }

        public IEnumerable<ProjectNode> EnumerateProjectNodes()
        {
            return EnumerateRootNodes().SelectMany(f => f.EnumerateProjectNodes());
        }

        public IEnumerable<FileInputNode> EnumerateFileInputNodes()
        {
            return EnumerateProjectNodes().SelectMany(f => f.EnumerateFileInputNodes());
        }

        public void AddNewSubdirectory()
        {
            if (CanAddNewSubdirectory)
            {
                RootNode rootNode = SelectedRootNode;
                string dirName = FileSystemUtility.GetNewDirectoryName(rootNode.FullName);
                DirectoryNode directoryNode = rootNode.AddDirectoryNode(dirName);
                if (directoryNode != null)
                {
                    TreeView.SelectedNode = directoryNode;
                    directoryNode.BeginEdit();
                }
            }
        }

        public void ReloadDirectory()
        {
            RootNode node = SelectedRootNode;
            if (node != null)
            {
                bool focused = TreeView.Focused;
                ReloadDirectory(node);

                if (focused)
                    TreeView.Select();
            }
        }

        private void ReloadDirectory(RootNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (Save())
            {
                ProjectNode projectNode = CurrentProjectNode;
                RootNode rootNode = LoadDirectoryInternal(node.DirectoryInfo);
                if (rootNode != null && projectNode != null)
                {
                    projectNode = rootNode.FindProjectNode(projectNode.FullName);

                    if (projectNode != null)
                        LoadProject(projectNode);
                }
            }
        }

        private RootNode FindOrLoadDirectory(DirectoryInfo info)
        {
            return FindRootNode(info.FullName) ?? LoadDirectoryInternal(info);
        }

        public void LoadDirectory(string dirPath)
        {
            if (dirPath == null)
                throw new ArgumentNullException(nameof(dirPath));

            DirectoryInfo info = Executor.CreateDirectoryInfo(dirPath, true);

            if (info != null)
                LoadDirectory(info);
        }

        private RootNode LoadDirectoryInternal(DirectoryInfo info)
        {
            var rootNode = new RootNode(info) { Text = Resources.ParentDirectoryStr };

            if (Executor.Execute(() => rootNode.LoadNodes(f => IsMatch(f), f => IsMatch(f))))
            {
                TreeView.BeginUpdate();
                TreeView.Nodes.Clear();
                TreeView.Nodes.Add(rootNode);
                rootNode.Expand();
                TreeView.SelectedNode = rootNode;
                SetRootNodeImage(rootNode);
                TreeView.EndUpdate();
                OnRootNodeChanged(new TreeViewEventArgs(rootNode));
            }
            else
            {
                return null;
            }

            return rootNode;
        }

        public void LoadDirectory(DirectoryInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (FindRootNode(info.FullName) != null || (Save() && LoadDirectoryInternal(info) != null))
            {
                if (!History.IsExecuting)
                    AddComand(new DirectoryCommand(info, this));
            }
        }

        public void LoadParentDirectory()
        {
            RootNode node = SelectedRootNode;

            if (node != null)
                LoadParentDirectory(node);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void LoadParentDirectory(RootNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            DirectoryInfo info = node.ParentDirectory;

            if (info != null)
                LoadDirectory(info);
        }

        private void DeleteDirectory(string dirPath)
        {
            FileSystemUtility.DeleteDirectory(dirPath, UseRecycleBin);
        }

        public void DeleteDirectory(DirectoryNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (!Directory.Exists(node.FullName))
            {
                if (MessageDialog.Question(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FolderNotFoundRemoveItemMsg,
                        node.FullName)) == DialogResult.Yes)
                {
                    RemoveNode(node);
                }
            }
            else if (MessageDialog.Confirm(string.Format(
                CultureInfo.CurrentCulture,
                Resources.Item_AndAllItsContentWillBeDeletedPermanentlyMsg,
                node.FileName.Enclose("'"))))
            {
                if (Executor.Delete(() => DeleteDirectory(node.FullName)))
                    RemoveNode(node);
            }
        }

        public void DeleteFile(string filePath)
        {
            FileSystemUtility.DeleteFile(filePath, UseRecycleBin);
        }

        public void LoadNewProject()
        {
            LoadNewProject(InputKind.None);
        }

        public void LoadNewProject(InputKind inputKind)
        {
            if (Save())
            {
                string projectPath = Dialogs.GetNewProjectPath(SelectedRootPath);
                if (projectPath != null)
                {
                    Executor.Execute(() =>
                    {
                        var info = new FileInfo(projectPath);
                        ProjectNode projectNode = null;
                        TreeNode inputNode = null;
                        Project project = CreateDefaultProject(projectPath);
                        using (new FileConnection(project))
                        {
                            projectNode = AddProjectNode(info, project);
                            inputNode = AddNewInputOrFileInputNode(projectNode, inputKind);
                        }

                        if (projectNode != null)
                            LoadProject(projectNode, inputNode);
                    });
                }
            }
        }

        public void LoadNewProject(ProjectContainer container, Input input)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            string fileName = (CurrentProjectNode != null)
                ? CurrentProjectNode.FileNameWithoutExtension
                : Dialogs.DefaultProjectFileName;
            string projectPath = Dialogs.GetNewProjectPath(SelectedRootPath, fileName);
            if (projectPath != null)
            {
                Executor.Execute(() =>
                {
                    var info = new FileInfo(projectPath);
                    ProjectNode projectNode = null;
                    TreeNode inputNode = null;
                    var project = new Project(container, projectPath);
                    using (new FileConnection(project))
                    {
                        projectNode = AddProjectNode(info, project);

                        if (input != null)
                            inputNode = AddNewInputOrFileInputNode(projectNode, input);
                    }

                    if (projectNode != null)
                        LoadProject(projectNode, inputNode);
                });
            }
        }

        private TreeNode AddNewInputOrFileInputNode(ProjectNode projectNode, InputKind kind)
        {
            return AddNewInputOrFileInputNode(projectNode, null, kind);
        }

        private TreeNode AddNewInputOrFileInputNode(ProjectNode projectNode, Input input)
        {
            switch (input.Kind)
            {
                case InputKind.File:
                    {
                        string inputPath = Dialogs.GetNewInputPath(
                            projectNode.DirectoryName,
                            projectNode.FileNameWithoutExtension);
                        if (inputPath != null)
                        {
                            FileInfo info = Executor.CreateFileInfo(inputPath);
                            input.Name = inputPath;
                            return FindOrAddFileInputNode(info, input, projectNode);
                        }

                        return null;
                    }
                case InputKind.Included:
                    return projectNode.AddInputNode(input);
            }

            return null;
        }

        private TreeNode AddNewInputOrFileInputNode(ProjectNode projectNode, Input input, InputKind kind)
        {
            switch (kind)
            {
                case InputKind.File:
                    return AddNewFileInputNode(input, projectNode);
                case InputKind.Included:
                    {
                        if (input != null)
                        {
                            input.Name = projectNode.Project.GetNewInputName(input.Name);
                            return projectNode.AddInputNode(input);
                        }

                        return AddNewInputNode(projectNode);
                    }
            }

            return null;
        }

        public void LoadProject()
        {
            if (Save())
            {
                string projectPath = Dialogs.GetExistingProjectPath(SelectedRootPath);

                if (projectPath != null)
                    LoadProject(projectPath, false);
            }
        }

        public void LoadProject(string projectPath)
        {
            LoadProject(projectPath, true);
        }

        private void LoadProject(string projectPath, bool save)
        {
            FileInfo info = Executor.CreateFileInfo(projectPath, true);

            if (info != null)
                LoadProject(info, save);
        }

        internal void LoadProject(FileInfo projectInfo)
        {
            LoadProject(projectInfo, true);
        }

        private void LoadProject(FileInfo projectInfo, bool save)
        {
            if (!save || Save())
            {
                ProjectNode projectNode = FindOrAddProjectNode(projectInfo);
                if (projectNode != null)
                    LoadProject(projectNode);
            }
        }

        internal void LoadProject(ProjectNode projectNode)
        {
            if (projectNode == null)
                throw new ArgumentNullException(nameof(projectNode));

            if (projectNode.Project.InputNameKind == InputKind.Included)
            {
                LoadProject(projectNode, projectNode.FindInputNodeFromInputPath(), true);
            }
            else
            {
                LoadProject(projectNode, projectNode.FindFileInputNodeFromInputPath(), true);
            }
        }

        public void LoadOrReloadProject(ProjectNode projectNode)
        {
            if (projectNode == null)
                throw new ArgumentNullException(nameof(projectNode));

            bool fCurrent = IsCurrentProjectNode(projectNode);

            bool load;
            if (fCurrent)
            {
                load = (MessageDialog.Question(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.DoYouWantToReopenMsg,
                    projectNode.FileName.Enclose("'"))) == DialogResult.Yes);
            }
            else
            {
                load = Save();
            }

            if (load)
            {
                InputNode inputNode = (fCurrent) ? CurrentInputNode as InputNode : null;
                if (inputNode != null || projectNode.Project.InputNameKind == InputKind.Included)
                {
                    LoadProject(projectNode, inputNode ?? projectNode.FindInputNodeFromInputPath(), !fCurrent);
                }
                else
                {
                    FileInputNode fileNode = (fCurrent) ? CurrentInputNode as FileInputNode : null;
                    LoadProject(projectNode, fileNode ?? projectNode.FindFileInputNodeFromInputPath(), !fCurrent);
                }
            }
        }

        internal void LoadProject(FileInfo projectInfo, FileInfo inputInfo)
        {
            if (Save())
            {
                ProjectNode projectNode = FindOrAddProjectNode(projectInfo);
                if (projectNode != null)
                {
                    FileInputNode inputNode = (inputInfo != null)
                        ? projectNode.FindFileInputNode(inputInfo.FullName)
                        : projectNode.FindFileInputNodeFromInputPath();
                    LoadProject(projectNode, inputNode);
                }
            }
        }

        private void LoadProject(ProjectNode projectNode, TreeNode node)
        {
            if (node is InputNode inputNode)
            {
                LoadProject(projectNode, inputNode);
            }
            else
            {
                LoadProject(projectNode, node as FileInputNode);
            }
        }

        private void LoadProject(ProjectNode projectNode, FileInputNode inputNode)
        {
            LoadProject(projectNode, inputNode, true);
        }

        private void LoadProject(ProjectNode projectNode, FileInputNode inputNode, bool checkCurrent)
        {
            if (checkCurrent && IsCurrentProjectNode(projectNode))
            {
                if (inputNode != null)
                {
                    Debug.Assert(!IsCurrentInputNode(inputNode));
                    Executor.Execute(() => LoadFileInput(inputNode));
                }
            }
            else
            {
                if (inputNode != null
                    && !Executor.Execute(() => inputNode.LoadText(), false))
                {
                    inputNode = null;
                }

                Open(projectNode, inputNode, OpenMode.None);
            }
        }

        internal void LoadProject(FileInfo projectInfo, string inputName)
        {
            ProjectNode projectNode = FindOrAddProjectNode(projectInfo);
            if (projectNode != null)
            {
                InputNode node = (inputName != null)
                    ? projectNode.FindInputNode(inputName)
                    : projectNode.FindInputNodeFromInputPath();
                LoadProject(projectNode, node);
            }
        }

        internal void LoadProject(ProjectNode projectNode, InputNode inputNode)
        {
            LoadProject(projectNode, inputNode, true);
        }

        internal void LoadProject(ProjectNode projectNode, InputNode inputNode, bool checkCurrent)
        {
            if (checkCurrent && IsCurrentProjectNode(projectNode))
            {
                if (inputNode != null)
                {
                    Debug.Assert(!IsCurrentInputNode(inputNode));
                    Executor.Execute(() => LoadInput(inputNode));
                }
            }
            else
            {
                Open(projectNode, inputNode, OpenMode.None);
            }
        }

        private FileInputNode AddNewFileInputNode(Input input, ProjectNode projectNode)
        {
            FileInputNode inputNode = null;
            string inputPath = Dialogs.GetNewInputPath(projectNode);
            if (inputPath != null)
            {
                if (input == null)
                    input = CreateDefaultFileInput(inputPath);

                input.Name = inputPath;
                Executor.Execute(() => inputNode = FindOrAddFileInputNode(new FileInfo(inputPath), input, projectNode));
            }

            return inputNode;
        }

        internal InputNode AddNewInputNode(ProjectNode projectNode)
        {
            if (projectNode == null)
                throw new ArgumentNullException(nameof(projectNode));

            string name = projectNode.Project.GetNewInputName();
            Input input = CreateDefaultInput(name);
            return projectNode.AddInputNode(input);
        }

        private ProjectNode FindOrAddProjectNode(FileInfo info)
        {
            ProjectNode projectNode = null;
            RootNode rootNode = FindRootNode(info.DirectoryName);
            bool flg = (rootNode != null);

            if (rootNode == null)
                rootNode = LoadDirectoryInternal(info.Directory);

            if (rootNode != null)
            {
                projectNode = rootNode.FindProjectNode(info.FullName);
                if (projectNode == null && flg)
                    projectNode = rootNode.AddProjectNode(info);
            }

            return projectNode;
        }

        private ProjectNode AddProjectNode(FileInfo info, Project project)
        {
            ProjectNode projectNode = null;
            RootNode rootNode = FindOrLoadDirectory(info.Directory);
            if (rootNode != null)
            {
                projectNode = rootNode.FindProjectNode(info.FullName);
                if (projectNode != null)
                    RemoveNode(projectNode);

                projectNode = rootNode.AddProjectNode(info, project);
            }

            return projectNode;
        }

        public void DeleteProject(ProjectNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (!File.Exists(node.FullName))
            {
                if (MessageDialog.Question(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FileNotFoundRemoveItemMsg,
                        node.FullName)) == DialogResult.Yes)
                {
                    RemoveNode(node);
                }
            }
            else
            {
                FileInputNode[] inputNodes = node.GetFileInputNodes();
                bool fInput = inputNodes.Length > 0;
                using (var frm = new ProjectDeleteForm()
                {
                    Message = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Item_WillBeDeletedPermanentlyMsg,
                        node.FileName.Enclose("'")),
                    DeleteAllFileInputs = false,
                    DeleteAllFileInputsEnabled = fInput
                })
                {
                    if (frm.ShowDialog() == DialogResult.OK
                        && Executor.Delete(() => DeleteFile(node.FullName)))
                    {
                        if (fInput && frm.DeleteAllFileInputs)
                        {
                            FileErrorInfo[] errors = DeleteFileInputs(inputNodes, false);
                            ErrorInfoForm.ShowErrors(errors, Resources.FollowingInputsCouldNotBeDeletedMsg);
                        }

                        RemoveNode(node);
                    }
                }
            }
        }

        internal void DeleteAllFileInputs(ProjectNode node)
        {
            if (node.FileInputs.Count > 0
                && MessageDialog.Confirm(
                    string.Format(CultureInfo.CurrentCulture, Resources.DeleteAllInputFilesMsg, node.FileName.Enclose("'"))))
            {
                FileErrorInfo[] errors = null;
                using (node.OpenFile())
                {
                    errors = DeleteFileInputs(node.GetFileInputNodes(), true);

                    foreach (Input input in node.FileInputs.Except(node.EnumerateFileInputNodes().Select(f => f.Input))
                        .ToArray())
                    {
                        node.FileInputs.Remove(input);
                    }
                }

                if (errors != null)
                    ErrorInfoForm.ShowErrors(errors, Resources.FollowingFilesCouldNotBeDeletedMsg);

                Debug.Assert(!node
                    .GetFileInputNodes()
                    .Select(f => f.Input)
                    .Except(node.FileInputs)
                    .Any());
            }
        }

        private FileErrorInfo[] DeleteFileInputs(FileInputNode[] nodes, bool removeNode)
        {
            var errors = new List<FileErrorInfo>();
            foreach (FileInputNode node in nodes)
            {
                try
                {
                    DeleteFile(node.FullName);
                    if (removeNode)
                        RemoveNode(node);
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                    }
                    else if (Executor.IsFileSystemException(ex))
                    {
                        errors.Add(new FileErrorInfo(node.FullName, ex));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return errors.ToArray();
        }

        public void LoadOrReloadFileInput(FileInputNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (LoadFileInputCheck(node))
            {
                try
                {
                    LoadFileInput(node);
                }
                catch (Exception ex)
                {
                    if (ex is FileNotFoundException)
                    {
                        if (MessageDialog.Question(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.FileNotFoundRemoveItemMsg,
                                node.FullName)) == DialogResult.Yes)
                        {
                            RemoveFileInput(node);
                        }
                    }
                    else if (!Executor.ProcessException(ex))
                    {
                        throw;
                    }
                }
            }
        }

        private bool LoadFileInputCheck(FileInputNode node)
        {
            if (IsCurrentInputNode(node))
            {
                string text = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.DoYouWantToReopenMsg,
                    node.FileName.Enclose("'"));
                return MessageDialog.Question(text) == DialogResult.Yes;
            }
            else
            {
                return (IsCurrentProjectNode(node.ProjectNode)) ? SaveManager.SaveInput() : Save();
            }
        }

        public void LoadFileInput(FileInputNode inputNode)
        {
            if (inputNode == null)
                throw new ArgumentNullException(nameof(inputNode));

            inputNode.LoadText();

            if (IsCurrentProjectNode(inputNode.ProjectNode))
            {
                Open(null, inputNode, OpenMode.InputOnly);
            }
            else
            {
                Open(inputNode.ProjectNode, inputNode, OpenMode.None);
            }
        }

        public void LoadFileInputs(ProjectNode projectNode, string[] inputPaths)
        {
            if (LoadFileInputsInternal(projectNode, inputPaths))
            {
                if (inputPaths.Length == 1)
                {
                    Executor.Execute(() => LoadFileInput(projectNode.FindFileInputNode(inputPaths[0])));
                }
                else
                {
                    projectNode.Expand();
                }
            }
        }

        internal bool LoadFileInputsInternal(ProjectNode projectNode, string[] inputPaths)
        {
            if (projectNode == null)
                throw new ArgumentNullException(nameof(projectNode));

            if (inputPaths == null)
                throw new ArgumentNullException(nameof(inputPaths));

            string[] paths = inputPaths.Except(projectNode.EnumerateFileInputPaths(), FileSystemUtility.ComparerIgnoreCase)
                .ToArray();
            if (paths.Length > 0)
            {
                return projectNode.OpenFile(() =>
                {
                    foreach (string path in paths)
                        projectNode.AddFileInputNode(path, f => CreateDefaultFileInput(f));
                });
            }

            return false;
        }

        internal void LoadNewInput(InputKind kind)
        {
            if (TreeView.SelectedNode is ProjectNode node)
                LoadNewInput(node, kind);
        }

        public void LoadNewInput(ProjectNode node, InputKind kind)
        {
            if ((IsCurrentProjectNode(node)) ? SaveManager.SaveInput() : Save())
            {
                switch (kind)
                {
                    case InputKind.File:
                        {
                            LoadNewInput(node, CreateDefaultFileInput());
                            break;
                        }
                    case InputKind.Included:
                        {
                            LoadNewInput(node, CreateDefaultInput());
                            break;
                        }
                }
            }
        }

        public void LoadNewInput(ProjectNode projectNode, Input input)
        {
            LoadNewInput(projectNode, input, null);
        }

        public void LoadNewInput(ProjectNode projectNode, Input input, string initialFileName)
        {
            if (projectNode == null)
                throw new ArgumentNullException(nameof(projectNode));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            switch (input.Kind)
            {
                case InputKind.File:
                    {
                        string inputPath = Dialogs.GetNewInputPath(
                            projectNode.DirectoryName,
                            initialFileName ?? projectNode.FileNameWithoutExtension);
                        if (inputPath != null)
                        {
                            FileInfo info = Executor.CreateFileInfo(inputPath, true);
                            if (info != null)
                            {
                                FileInputNode fileNode = projectNode.OpenFile(delegate
                                {
                                    input.Name = inputPath;
                                    return FindOrAddFileInputNode(info, input, projectNode);
                                });

                                if (fileNode != null)
                                    LoadFileInput(fileNode);
                            }
                        }

                        break;
                    }
                case InputKind.Included:
                    {
                        string name = projectNode.Project.GetNewInputName(initialFileName);
                        InputNode inputNode = projectNode.OpenFile(delegate
                        {
                            input.Name = name;
                            return projectNode.AddInputNode(input);
                        });
                        if (inputNode != null)
                        {
                            LoadInput(inputNode);
                            inputNode.BeginEdit();
                        }

                        break;
                    }
            }
        }

        public void LoadNewFileInputs(ProjectNode projectNode)
        {
            if (Save())
            {
                string[] inputPaths = Dialogs.GetExistingInputPaths(projectNode);

                if (inputPaths != null)
                    LoadFileInputs(projectNode, inputPaths);
            }
        }

        private FileInputNode FindOrAddFileInputNode(FileInfo info, Input input, ProjectNode projectNode)
        {
            input.SaveText();
            FileInputNode node = projectNode.FindFileInputNode(info.FullName);

            if (node != null)
                RemoveNode(node);

            return projectNode.AddFileInputNode(info, input);
        }

        public void DeleteFileInput(FileInputNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (!File.Exists(node.FullName))
            {
                if (MessageDialog.Question(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FileNotFoundRemoveItemMsg,
                        node.FullName)) == DialogResult.Yes)
                {
                    RemoveFileInput(node);
                }
            }
            else if (MessageDialog.Confirm(string.Format(
                CultureInfo.CurrentCulture,
                Resources.Item_WillBeDeletedPermanentlyMsg,
                node.FileName.Enclose("'"))))
            {
                if (Executor.Delete(() => DeleteFile(node.FullName)))
                    RemoveFileInput(node);
            }
        }

        public void DeleteInput(InputNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (MessageDialog.Confirm(string.Format(
                CultureInfo.CurrentCulture,
                Resources.Item_WillBeDeletedPermanentlyMsg,
                node.Input.Name.Enclose("'")))
                && node.ProjectNode.OpenFile(delegate { node.ProjectNode.Inputs.Remove(node.Input); }))
            {
                RemoveNode(node);
            }
        }

        public void RemoveFileInput(FileInputNode node)
        {
            RemoveFileInput(node, false);
        }

        public void RemoveFileInput(FileInputNode node, bool confirm)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (!confirm
                || MessageDialog.Confirm(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ReferenceToFileWillBeRemovedFromProjectMsg,
                    node.FileName.Enclose("'"))))
            {
                if (node.ProjectNode.OpenFile(delegate { node.Project.FileInputs.Remove(node.Input); }))
                    RemoveNode(node);
            }
        }

        public void RemoveInput(InvalidInputNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            ProjectNode projectNode = node.ProjectNode;
            if (projectNode.OpenFile(delegate { projectNode.FileInputs.Remove(node.Input); }))
                RemoveNode(node);
        }

        public void RemoveAllInputs(ProjectNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Project.HasInputOrFileInput
                && MessageDialog.Confirm(
                    string.Format(CultureInfo.CurrentCulture, Resources.RemoveAllInputsMsg, node.FileName.Enclose("'")))
                && node.OpenFile(() =>
                {
                    node.FileInputs.Clear();
                    node.Inputs.Clear();
                }))
            {
                RemoveNodes(node.GetFileInputNodes());
                RemoveNodes(node.GetInputNodes());
            }
        }

        internal void LoadOrReloadInput(InputNode node)
        {
            bool load;
            if (IsCurrentInputNode(node))
            {
                load = MessageDialog.Question(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.DoYouWantToReopenMsg,
                        node.FileName)) == DialogResult.Yes;
            }
            else
            {
                load = (IsCurrentProjectNode(node.ProjectNode)) ? SaveManager.SaveInput() : Save();
            }

            if (load)
                LoadInput(node);
        }

        internal void LoadInput(InputNode node)
        {
            if (IsCurrentProjectNode(node.ProjectNode))
            {
                Open(null, node, OpenMode.InputOnly);
            }
            else
            {
                Open(node.ProjectNode, node, OpenMode.None);
            }
        }

        private void Open(ProjectNode projectNode, TreeNode inputNode, OpenMode mode)
        {
            if (projectNode != null || inputNode != null)
                AddCommand(projectNode, inputNode);

            if (mode == OpenMode.None)
            {
                CurrentProjectNode = projectNode;
                CurrentInputNode = inputNode;
            }
            else if (mode == OpenMode.InputOnly)
            {
                CurrentInputNode = inputNode;
            }

            if (projectNode != null)
            {
                RecentManager.Add(new RecentProject(projectNode.FileInfo));
                TreeView.SelectedNode = projectNode;
            }
            else if (inputNode != null)
            {
                TreeView.SelectedNode = inputNode;
            }

            inputNode?.EnsureVisible();

            if (projectNode != null)
            {
                projectNode.Expand();
                projectNode.EnsureVisible();
            }

            ProjectContainer container = projectNode?.Project.Container;
            var inputContainer = inputNode as IInputContainer;
            Input input = inputContainer?.Input;
            Open(container, input, mode);
        }

        private void AddCommand(ProjectNode projectNode, TreeNode inputNode)
        {
            if (!History.IsExecuting)
            {
                FileSystemCommand command = GetCommand(projectNode, inputNode);
                if (command != null)
                    AddComand(command);
            }
        }

        private void AddComand(FileSystemCommand command)
        {
            var current = History.Current as FileSystemCommand;

            if (Equals(current, command))
                History.Remove(current);

            History.AddCommand(command);
        }

        private FileSystemCommand GetCommand(ProjectNode projectNode, TreeNode node)
        {
            if (node is FileInputNode fileNode)
            {
                FileInfo projectInfo = (projectNode != null) ? projectNode.FileInfo : fileNode.ProjectNode.FileInfo;
                return new FileInputCommand(projectInfo, fileNode.FileInfo, this);
            }
            else if (node is InputNode inputNode)
            {
                FileInfo projectInfo = (projectNode != null) ? projectNode.FileInfo : inputNode.ProjectNode.FileInfo;
                return new InputCommand(projectInfo, inputNode.Input, this);
            }
            else if (projectNode != null)
            {
                return new ProjectCommand(projectNode.FileInfo, this);
            }

            return null;
        }

        internal static void SetRootNodeImage(RootNode node)
        {
            node.SetImage((node.IsExpanded) ? NodeImageIndex.DirectoryOpen : NodeImageIndex.Directory);
        }

        public Input CreateDefaultFileInput()
        {
            return CreateDefaultFileInput(null);
        }

        public Input CreateDefaultFileInput(string filePath)
        {
            return CreateDefaultInput(filePath, InputKind.File);
        }

        public Input CreateDefaultInput()
        {
            return CreateDefaultInput(null);
        }

        public Input CreateDefaultInput(string name)
        {
            return CreateDefaultInput(name, InputKind.Included);
        }

        public Input CreateDefaultInput(string filePath, InputKind kind)
        {
            Input input = DefaultInput;
            input.Name = filePath;
            input.Kind = kind;
            return input;
        }

        public Project CreateDefaultProject(string filePath)
        {
            return new Project(DefaultProjectContainer ?? new ProjectContainer(), filePath);
        }

        public bool IsCurrentProjectNode(string fullName)
        {
            return CurrentProjectNode != null && FileSystemUtility.PathEquals(CurrentProjectNode.FullName, fullName);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public bool IsCurrentProjectNode(FileInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return CurrentProjectNode != null && FileSystemUtility.PathEquals(CurrentProjectNode.FullName, info.FullName);
        }

        public bool IsCurrentProjectNode(TreeNode node)
        {
            return node != null && ReferenceEquals(CurrentProjectNode, node);
        }

        public bool IsCurrentInputNode(TreeNode node)
        {
            return node != null && ReferenceEquals(CurrentInputNode, node);
        }

        internal void RemoveNodes(TreeNode[] nodes)
        {
            foreach (TreeNode node in nodes)
                RemoveNode(node);
        }

        internal void RemoveNode(TreeNode node)
        {
            TreeNode n = node.NextOrPreviousNode();
            node.Remove();
            if (IsCurrentProjectNode(node))
            {
                CurrentProjectNode = null;
                CurrentInputNode = null;
            }
            else if (IsCurrentInputNode(node))
            {
                CurrentInputNode = null;
            }

            if (n != null)
            {
                TreeView.SelectedNode = n;
                n.EnsureVisible();
            }
        }

        internal void LoadInvalid(InvalidNode invalidNode)
        {
            if (invalidNode is InvalidProjectNode projectNode)
            {
                LoadInvalidProject(projectNode);
            }
            else if (invalidNode is InvalidInputNode inputNode)
            {
                LoadInvalidInput(inputNode);
            }
        }

        private void LoadInvalidProject(InvalidProjectNode invalidNode)
        {
            if (invalidNode.Parent is RootNode rootNode)
            {
                ProjectNode projectNode = rootNode.FindProjectNode(invalidNode.Path);

                if (projectNode != null)
                {
                    RemoveNode(invalidNode);
                    TreeView.SelectedNode = projectNode;
                    LoadOrReloadProject(projectNode);
                }
                else if (Executor.Execute(
                    () => projectNode = invalidNode.CreateNode() as ProjectNode,
                    Executor.BasicExceptions | Exceptions.InvalidOperation | Exceptions.XmlException))
                {
                    RemoveNode(invalidNode);
                    rootNode.Nodes.Add(projectNode);
                    LoadProject(projectNode);
                }
            }
        }

        private void LoadInvalidInput(InvalidInputNode invalidNode)
        {
            if (invalidNode.Parent is ProjectNode projectNode)
            {
                FileInputNode inputNode = projectNode.FindFileInputNode(invalidNode.Path);
                if (inputNode != null)
                {
                    RemoveNode(invalidNode);
                    TreeView.SelectedNode = inputNode;
                    LoadOrReloadFileInput(inputNode);
                }
                else
                {
                    Executor.Execute(() =>
                    {
                        inputNode = invalidNode.CreateNode() as FileInputNode;
                        using (projectNode.OpenFile())
                        {
                            RemoveNode(invalidNode);
                            projectNode.Nodes.Add(inputNode);
                            projectNode.FileInputs.Add(inputNode.Input);
                        }

                        LoadFileInput(inputNode);
                    });
                }
            }
        }

        public bool Save()
        {
            return SaveManager.Save();
        }

        private bool IsMatch(FileSystemInfo info)
        {
            return ShowHidden || (info.Attributes & FileAttributes.Hidden) == 0;
        }

        private bool IsMatch(FileSystemNode node)
        {
            if (!string.IsNullOrEmpty(SearchPhrase)
                && node.Kind == NodeKind.Project)
            {
                var projectNode = node as ProjectNode;
                const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;

                return projectNode.FileNameWithoutExtension.IndexOf(SearchPhrase, comparison) != -1
                    || projectNode.Project.Container.Pattern.Text.IndexOf(SearchPhrase, comparison) != -1;
            }

            return true;
        }

        public void ClearSearchPhrase()
        {
            SearchPhrase = "";
        }

        internal bool CanCut()
        {
            var node = TreeView.SelectedNode as IClipboardNode;
            return node?.CanCut() ?? false;
        }

        internal bool CanCopy()
        {
            var node = TreeView.SelectedNode as IClipboardNode;
            return node?.CanCopy() ?? false;
        }

        internal bool CanBePasted()
        {
            TreeNode target = TreeView.SelectedNode;
            return (target != null) && ClipboardManager.CanBePasted(target);
        }

        protected virtual void Open(ProjectContainer container, Input input, OpenMode mode)
        {
        }

        protected virtual void OnRootNodeChanged(TreeViewEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            CurrentProjectNode = null;
            CurrentInputNode = null;

            if (e.Node is RootNode rootNode)
                RecentManager.Add(new RecentDirectory(rootNode.DirectoryInfo));

            RootNodeChanged?.Invoke(this, e);
        }

        protected virtual void OnCurrentProjectChanged(EventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            CurrentProjectChanged?.Invoke(this, e);
        }

        protected virtual void OnCurrentInputChanged(EventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            CurrentInputChanged?.Invoke(this, e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClipboardManager.Dispose();
                    ClipboardManager = null;
                    TreeView.Dispose();
                    TreeView = null;
                }

                _disposed = true;
            }
        }

        public string SearchPhrase
        {
            get { return _searchPhrase; }
            set { _searchPhrase = value ?? ""; }
        }

        public TreeNode SelectedNode
        {
            get { return TreeView.SelectedNode; }
            set { TreeView.SelectedNode = value; }
        }

        public string CurrentProjectPath
        {
            get { return CurrentProjectNode?.FullName; }
        }

        public string CurrentInputFullName
        {
            get
            {
                var inputContainer = CurrentInputNode as IInputContainer;
                return inputContainer?.FullName;
            }
        }

        public FileInfo CurrentProjectInfo
        {
            get { return CurrentProjectNode?.FileInfo; }
        }

        public FileInfo CurrentInputInfo
        {
            get
            {
                var inputNode = CurrentInputNode as FileInputNode;
                return inputNode?.FileInfo;
            }
        }

        public RootNode SelectedRootNode
        {
            get
            {
                if (SelectedNode != null)
                {
                    var node = SelectedNode as INode;
                    return node?.GetRootNode();
                }

                return EnumerateRootNodes().FirstOrDefault();
            }
        }

        public string SelectedRootPath
        {
            get
            {
                return (SelectedNode is INode node)
              ? node.RootPath
              : EnumerateRootNodes().Select(f => f.FullName).FirstOrDefault();
            }
        }

        public ProjectNode CurrentProjectNode
        {
            get { return _projectNode; }
            private set
            {
                if (!ReferenceEquals(_projectNode, value))
                {
                    _projectNode?.SetImage(NodeImageIndex.Project);

                    _projectNode = value;

                    _projectNode?.SetImage(NodeImageIndex.ProjectOpen);

                    OnCurrentProjectChanged(EventArgs.Empty);
                }
            }
        }

        public TreeNode CurrentInputNode
        {
            get { return _inputNode; }
            private set
            {
                if (!ReferenceEquals(_inputNode, value))
                {
                    if (_inputNode is IInputContainer inputContainer)
                        FileSystemUtility.SetImage(_inputNode, inputContainer.NodeImageIndex);

                    _inputNode = value;
                    inputContainer = _inputNode as IInputContainer;

                    if (inputContainer != null)
                        FileSystemUtility.SetImage(_inputNode, inputContainer.OpenNodeImageIndex);

                    OnCurrentInputChanged(EventArgs.Empty);
                }
            }
        }

        public virtual Input DefaultInput
        {
            get { return new Input(); }
        }

        public virtual ProjectContainer DefaultProjectContainer
        {
            get { return new ProjectContainer(); }
        }

        public FileSystemTreeView TreeView { get; private set; }

        internal bool CanAddNewSubdirectory
        {
            get { return SelectedRootNode != null; }
        }

        public static Encoding DefaultEncoding
        {
            get { return new UTF8Encoding(true); }
        }

        internal KeyProcessor KeyProcessor { get; }
        internal ClipboardManager ClipboardManager { get; private set; }
        public FileSystemHistory History { get; }
        public RecentManager RecentManager { get; }
        public SaveManager SaveManager { get; }
        public bool UseRecycleBin { get; set; }
        public bool ShowHidden { get; set; }
        public bool ConfirmFileInputRemoval { get; set; }

        private ProjectNode _projectNode;
        private TreeNode _inputNode;
        private string _searchPhrase = "";
        private bool _disposed;

        public event EventHandler<TreeViewEventArgs> RootNodeChanged;
        public event EventHandler CurrentProjectChanged;
        public event EventHandler CurrentInputChanged;

        public const string ProjectExtension = "rgx";
        public const string ProjectSearchPattern = "*" + ProjectExtension;
        public const string DefaultInputExtension = "txt";
    }
}