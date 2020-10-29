// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using Regexator.Text;

namespace Regexator.IO
{
    //TODO: přidat vlastnost OriginalFileName
    public sealed class SearchResult : INotifyPropertyChanged
    {
        private static readonly string _directorySeparator = new string(new char[] { Path.DirectorySeparatorChar });

        private string _message;
        private string _fullName;
        private string _newName;
        private SearchResultState _state;
        private static Regex _invalidFileNameCharsRegex;

        public SearchResult(string fullName)
            : this(fullName, FileSystemEntry.File)
        {
        }

        public SearchResult(string fullName, FileSystemEntry entry)
        {
            _fullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Name = Path.GetFileName(fullName);
            Entry = entry;
        }

        public bool Rename()
        {
            try
            {
                string directoryName = Path.GetDirectoryName(FullName);
                if (!string.IsNullOrEmpty(directoryName))
                {
                    Debug.WriteLine(string.Format("renaming: {0}", FullName));

                    if (IsDirectory)
                    {
                        Microsoft.VisualBasic.FileIO.FileSystem.RenameDirectory(FullName, NewName);
                    }
                    else
                    {
                        Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(FullName, NewName);
                    }

                    FullName = Path.Combine(directoryName, NewName);

                    Debug.WriteLine(string.Format("renamed: {0}", FullName));

                    State = SearchResultState.Renamed;
                    return true;
                }
            }
            catch (Exception ex) when (ProcessException(ex))
            {
            }

            return false;
        }

        public bool Delete()
        {
            return Delete(RecycleOption.SendToRecycleBin);
        }

        public bool Delete(RecycleOption recycleOption)
        {
            try
            {
                Debug.WriteLine(string.Format("deleting: {0}", FullName));

                if (IsDirectory)
                {
                    Microsoft.VisualBasic.FileIO.FileSystem
                        .DeleteDirectory(FullName, UIOption.OnlyErrorDialogs, recycleOption, UICancelOption.DoNothing);
                }
                else
                {
                    Microsoft.VisualBasic.FileIO.FileSystem
                        .DeleteFile(FullName, UIOption.OnlyErrorDialogs, recycleOption, UICancelOption.DoNothing);
                }

                Debug.WriteLine(string.Format("deleted: {0}", FullName));

                State = SearchResultState.Deleted;
                return true;
            }
            catch (Exception ex) when (ProcessException(ex))
            {
            }

            return false;
        }

        private bool ProcessException(Exception ex)
        {
            if (FileSystemUtility.IsFileSystemException(ex))
            {
                State = SearchResultState.Error;
                Message = ex.GetBaseException().Message;
                return true;
            }

            return false;
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnNotifyPropertyChanged(nameof(Message));
                }
            }
        }

        public string NameWithoutExtension
        {
            get
            {
                return (IsDirectory)
                    ? Path.GetFileName(Name)
                    : Path.GetFileNameWithoutExtension(Name);
            }
        }

        public string Extension
        {
            get
            {
                return (IsDirectory)
                    ? ""
                    : Path.GetExtension(Name).TrimStart('.');
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnNotifyPropertyChanged(nameof(FullName));
                }
            }
        }

        public string DirectoryName
        {
            get
            {
                string s = Path.GetDirectoryName(FullName);
                if (!string.IsNullOrEmpty(s) && s[s.Length - 1] != Path.DirectorySeparatorChar)
                    return s + _directorySeparator;

                return s;
            }
        }

        public string NewName
        {
            get { return _newName; }
            set
            {
                if (_newName != value)
                {
                    _newName = value;
                    OnNotifyPropertyChanged(nameof(NewName));
                }
            }
        }

        public bool IsValidNewName
        {
            get { return !string.IsNullOrWhiteSpace(NewName) && !InvalidFileNameCharsRegex.IsMatch(NewName); }
        }

        public SearchResultState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnNotifyPropertyChanged(nameof(State));
                }
            }
        }

        public string Name { get; }

        public bool IsFile
        {
            get { return Entry == FileSystemEntry.File; }
        }

        public bool IsDirectory
        {
            get { return Entry == FileSystemEntry.Directory; }
        }

        public FileSystemEntry Entry { get; }

        public bool CanRename
        {
            get
            {
                return !IsDeleted
                    && !IsRenamed
                    && IsValidNewName
                    && !IsNewNameUnchanged;
            }
        }

        public bool IsNewNameUnchanged
        {
            get { return string.Equals(Name, NewName, StringComparison.Ordinal); }
        }

        public bool CanDelete
        {
            get { return !IsDeleted; }
        }

        public bool IsRenamed
        {
            get { return State == SearchResultState.Renamed; }
        }

        public bool IsDeleted
        {
            get { return State == SearchResultState.Deleted; }
        }

        private void OnNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static Regex InvalidFileNameCharsRegex
        {
            get
            {
                return _invalidFileNameCharsRegex
                    ?? (_invalidFileNameCharsRegex = PatternLibrary.InvalidFileNameChars.ToRegex());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
