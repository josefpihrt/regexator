// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Regexator.Collections.Generic;
using Regexator.UI;
using Regexator.Text;

namespace Regexator.FileSystem
{
    internal sealed class SaveExecutor
    {
        private SaveExecutor(FileSystemManager manager, SaveEventArgs e)
        {
            _e = e;
            ProjectNode = manager.CurrentProjectNode;
            FileInputNode = manager.CurrentInputNode as FileInputNode;
            InputNode = manager.CurrentInputNode as InputNode;
            _isIncluded = InputNode != null;
        }

        public static bool Save(FileSystemManager manager, SaveEventArgs e)
        {
            var executor = new SaveExecutor(manager, e);
            return executor.Save();
        }

        private bool Save()
        {
            if (PatternConfirmRequired || InputConfirmRequired)
            {
                using (var frm = new SaveForm(this))
                {
                    switch (frm.ShowDialog())
                    {
                        case DialogResult.Cancel:
                            return false;
                        case DialogResult.No:
                            {
                                _autoSaveOnly = true;
                                break;
                            }
                    }
                }
            }

            return SaveProject() && SaveInputText();
        }

        private bool SaveInputText()
        {
            if (!_isIncluded
                && !_autoSaveOnly
                && InputSaveRequired
                && InputProps.ContainsAny(InputProps.Text | InputProps.Encoding))
            {
                return Executor.Execute(() => FileInputNode.SaveText(NewInput.Text));
            }

            return true;
        }

        private bool SaveProject()
        {
            SetInput();
            SetPattern();
            SetInputPath();
            if (_fInput || _fPattern || _fInputPath)
            {
                try
                {
                    ProjectNode.SaveProject();
                }
                catch (Exception ex)
                {
                    RestoreInputValues();
                    RestorePatternValues();
                    RestoreInputPathValue();
                    if (ex is InvalidOperationException)
                    {
                        if (InputRequiresMessage || PatternRequiresMessage)
                        {
                            if (CheckInvalidXmlChar() == -1)
                            {
                                MessageDialog.Warning(
                                    Resources.ProjectCannotBeSavedMsg + "\n\n" + ex.GetBaseException().Message);
                                Debug.Fail("");
                            }
                        }
                        else
                        {
                            Debug.Fail("");
                        }
                    }
                    else
                    {
                        Debug.Fail("");
                        if (!Executor.ProcessException(ex, InputRequiresMessage || PatternRequiresMessage))
                            throw;
                    }

                    return _autoSaveOnly || (!_fInput && !_fPattern && _fInputPath);
                }
            }

            return true;
        }

        private int CheckInvalidXmlChar()
        {
            string source = Resources.Pattern;
            int ch = -1;
            if (_fPattern)
            {
                ch = TextUtility.GetFirstInvalidXmlChar(NewContainer.Pattern.Text);
                if (ch == -1)
                {
                    ch = TextUtility.GetFirstInvalidXmlChar(NewContainer.Replacement.Text);
                    source = Resources.Replacement;
                }
            }

            if (ch == -1 && _fInput)
            {
                ch = TextUtility.GetFirstInvalidXmlChar(NewInput.Text);
                source = Resources.Input;
            }

            if (ch != -1)
            {
                string s = Resources.ProjectCannotBeSavedMsg + "\n\n" + Resources.ProjectContainsInvalidCharMsg;
                MessageDialog.Warning(string.Format(
                    CultureInfo.CurrentCulture,
                    s,
                    source,
                    (char)ch,
                    ch,
                    ch.ToString("X2", CultureInfo.InvariantCulture)));
            }

            return ch;
        }

        private bool SetInput()
        {
            if (InputSaveRequired)
            {
                InputProps props = (_isIncluded) ? InputProps : InputProps.Except(InputProps.Text);
                InputProps props2 = (_autoSaveOnly) ? Input.AutoSaveProps : Input.AllProps;
                if (props.Intersect(props2).Any())
                {
                    _fInput = true;
                    _inputClone = (Input)OldInput.Clone();
                    OldInput.CurrentLine = NewInput.CurrentLine;
                    if (!_autoSaveOnly)
                    {
                        if (Mode != SaveMode.InputText)
                        {
                            OldInput.Options = NewInput.Options;
                            OldInput.NewLine = NewInput.NewLine;
                            OldInput.Encoding = NewInput.Encoding;
                            OldInput.Attributes = NewInput.Attributes;
                        }

                        if (_isIncluded)
                            OldInput.Text = NewInput.Text;
                    }
                }
            }

            return _fInput;
        }

        private bool SetPattern()
        {
            if (PatternSaveRequired && (!_autoSaveOnly || ContainerProps.Intersect(ProjectContainer.AutoSaveProps).Any()))
            {
                _fPattern = true;
                _projectClone = (Project)Project.Clone();
                if (_autoSaveOnly)
                {
                    Project.Container.Pattern.CurrentLine = NewContainer.Pattern.CurrentLine;
                    Project.Container.Replacement.CurrentLine = NewContainer.Replacement.CurrentLine;
                }
                else
                {
                    Collection<string> unknownPatternOptions = Project.Container.Pattern.UnknownPatternOptions;
                    Collection<string> unknownReplacementOptions = Project.Container.Replacement.UnknownOptions;
                    Collection<string> unknownOutputOptions = Project.Container.OutputInfo.UnknownOptions;
                    switch (Mode)
                    {
                        case SaveMode.Pattern:
                            {
                                Project.Container.Pattern = NewContainer.Pattern;
                                Project.Container.Pattern.UnknownPatternOptions.AddItems(unknownPatternOptions);
                                break;
                            }
                        case SaveMode.Replacement:
                            {
                                Project.Container.Replacement = NewContainer.Replacement;
                                Project.Container.Replacement.UnknownOptions.AddItems(unknownReplacementOptions);
                                break;
                            }
                        case SaveMode.ProjectInfo:
                            {
                                Project.Container.ProjectInfo = NewContainer.ProjectInfo;
                                break;
                            }
                        default:
                            {
                                Project.Container = NewContainer;
                                Project.Container.Pattern.UnknownPatternOptions.AddItems(unknownPatternOptions);
                                Project.Container.Replacement.UnknownOptions.AddItems(unknownReplacementOptions);
                                Project.Container.OutputInfo.UnknownOptions.AddItems(unknownOutputOptions);
                                break;
                            }
                    }
                }
            }

            return _fPattern;
        }

        private bool SetInputPath()
        {
            if (Project != null && Mode != SaveMode.Input && Mode != SaveMode.InputText)
            {
                var newPath = "";
                var flg = false;
                var kind = InputKind.None;
                if (FileInputNode != null)
                {
                    newPath = FileInputNode.FullName;
                    kind = InputKind.File;
                    flg = kind != Project.InputNameKind || !FileSystemUtility.PathEquals(Project.InputName, newPath);
                }
                else if (InputNode != null)
                {
                    newPath = InputNode.Input.Name;
                    kind = InputKind.Included;
                    flg = (kind != Project.InputNameKind || !Input.NameEquals(Project.InputName, newPath));
                }

                if (flg)
                {
                    if (_projectClone == null)
                        _projectClone = (Project)Project.Clone();

                    _fInputPath = true;
                    Project.InputName = newPath;
                    Project.InputNameKind = kind;
                }
            }

            return _fInputPath;
        }

        private bool InputRequiresMessage
        {
            get
            {
                InputProps except = Input.AutoSaveProps;

                if (!_isIncluded)
                    except = except.Union(InputProps.Text);

                return _fInput && !_autoSaveOnly && InputProps.Except(except).Any();
            }
        }

        private bool PatternRequiresMessage
        {
            get { return _fPattern && !_autoSaveOnly && ContainerProps.Except(ProjectContainer.AutoSaveProps).Any(); }
        }

        private void RestoreInputValues()
        {
            if (_fInput)
            {
                OldInput.CurrentLine = _inputClone.CurrentLine;
                if (!_autoSaveOnly)
                {
                    OldInput.Options = _inputClone.Options;
                    OldInput.NewLine = _inputClone.NewLine;
                    OldInput.Encoding = _inputClone.Encoding;
                    OldInput.Attributes = _inputClone.Attributes;

                    if (_isIncluded)
                        OldInput.Text = _inputClone.Text;
                }
            }
        }

        private void RestorePatternValues()
        {
            if (_fPattern)
            {
                ProjectContainer container = Project.Container;
                if (_autoSaveOnly)
                {
                    container.Pattern.CurrentLine = _projectClone.Container.Pattern.CurrentLine;
                    container.Replacement.CurrentLine = _projectClone.Container.Replacement.CurrentLine;
                }
                else
                {
                    switch (Mode)
                    {
                        case SaveMode.Pattern:
                            {
                                Project.Container.Pattern = _projectClone.Container.Pattern;
                                break;
                            }
                        case SaveMode.Replacement:
                            {
                                Project.Container.Replacement = _projectClone.Container.Replacement;
                                break;
                            }
                        case SaveMode.ProjectInfo:
                            {
                                Project.Container.ProjectInfo = _projectClone.Container.ProjectInfo;
                                break;
                            }
                        default:
                            {
                                Project.Container = _projectClone.Container;
                                break;
                            }
                    }
                }
            }
        }

        private void RestoreInputPathValue()
        {
            if (_fInputPath)
            {
                Project.InputName = _projectClone.InputName;
                Project.InputNameKind = _projectClone.InputNameKind;
            }
        }

        public ContainerProps ContainerProps
        {
            get
            {
                if (!_fContainerProps)
                {
                    _containerProps = CreateContainerProps();
                    _fContainerProps = true;
                }

                return _containerProps;
            }
        }

        private ContainerProps CreateContainerProps()
        {
            var value = ContainerProps.None;
            if (OldContainer != null)
            {
                ContainerProps props = GetContainerPropsToCheck();
                value = (_e.Confirm) ? ProjectContainer.GetChangedProps(OldContainer, NewContainer, props) : props;
                if (OldContainer.Pattern.HasOptions(PatternOptions.CurrentLineOnly)
                    ^ NewContainer.Pattern.HasOptions(PatternOptions.CurrentLineOnly))
                {
                    value = value.Except(ContainerProps.PatternCurrentLine);
                }

                if (OldContainer.Replacement.HasOptions(ReplacementOptions.CurrentLineOnly)
                    ^ NewContainer.Replacement.HasOptions(ReplacementOptions.CurrentLineOnly))
                {
                    value = value.Except(ContainerProps.ReplacementCurrentLine);
                }
            }

            return value;
        }

        private ContainerProps GetContainerPropsToCheck()
        {
            switch (Mode)
            {
                case SaveMode.Pattern:
                    return Pattern.AllProps;
                case SaveMode.Replacement:
                    return Replacement.AllProps;
                case SaveMode.ProjectInfo:
                    return ProjectInfo.AllProps;
                default:
                    return ProjectContainer.AllProps;
            }
        }

        public InputProps InputProps
        {
            get
            {
                if (!_fInputProps)
                {
                    _inputProps = CreateInputProps();
                    _fInputProps = true;
                }

                return _inputProps;
            }
        }

        private InputProps CreateInputProps()
        {
            var value = InputProps.None;
            if (OldInput != null)
            {
                InputProps props = GetInputPropsToCheck();
                value = (_e.Confirm) ? Input.GetChangedProps(OldInput, NewInput, props) : props;
                if (OldInput.HasOptions(InputOptions.CurrentLineOnly)
                    ^ NewInput.HasOptions(InputOptions.CurrentLineOnly))
                {
                    value = value.Except(InputProps.CurrentLine);
                }
            }

            return value;
        }

        private InputProps GetInputPropsToCheck()
        {
            switch (Mode)
            {
                case SaveMode.InputText:
                    return InputProps.Text;
                default:
                    return Input.AllProps.Except(InputProps.Name);
            }
        }

        public Project Project
        {
            get { return ProjectNode?.Project; }
        }

        public string ProjectPath
        {
            get { return ProjectNode?.FullName; }
        }

        public string InputFullName
        {
            get
            {
                if (FileInputNode != null)
                {
                    return FileInputNode.FullName;
                }
                else if (InputNode != null)
                {
                    return InputNode.FullName;
                }

                return null;
            }
        }

        public ProjectContainer OldContainer
        {
            get { return Project?.Container; }
        }

        public Input OldInput
        {
            get
            {
                if (FileInputNode != null)
                {
                    return FileInputNode.Input;
                }
                else if (InputNode != null)
                {
                    return InputNode.Input;
                }

                return null;
            }
        }

        public bool PatternConfirmRequired
        {
            get { return _e.Confirm && NewContainer != null && ContainerProps.Except(ProjectContainer.AutoSaveProps).Any(); }
        }

        public bool InputConfirmRequired
        {
            get { return _e.Confirm && NewInput != null && InputProps.Except(Input.AutoSaveProps).Any(); }
        }

        public bool PatternSaveRequired
        {
            get { return NewContainer != null && ContainerProps.Any(); }
        }

        public bool InputSaveRequired
        {
            get { return NewInput != null && InputProps.Any(); }
        }

        public ProjectContainer NewContainer
        {
            get { return _e.Container; }
        }

        public Input NewInput
        {
            get { return _e.Input; }
        }

        public SaveMode Mode
        {
            get { return _e.Mode; }
        }

        public ProjectNode ProjectNode { get; }

        public FileInputNode FileInputNode { get; }

        public InputNode InputNode { get; }

        private ContainerProps _containerProps;
        private InputProps _inputProps;
        private bool _fContainerProps;
        private bool _fInputProps;
        private readonly SaveEventArgs _e;
        private bool _autoSaveOnly;
        private readonly bool _isIncluded;
        private bool _fPattern;
        private bool _fInput;
        private bool _fInputPath;
        private Input _inputClone;
        private Project _projectClone;
    }
}
