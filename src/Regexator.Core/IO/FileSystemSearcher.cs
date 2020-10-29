// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using Regexator.Text.RegularExpressions;

namespace Regexator.IO
{
    public class FileSystemSearcher
    {
        private CancellationToken _cancellationToken;
        private SearchMode _searchMode;

        public FileSystemSearcher()
            : this(CancellationToken.None)
        {
        }

        public FileSystemSearcher(CancellationToken cancellationToken)
        {
            if (cancellationToken == default)
                throw new ArgumentNullException(nameof(cancellationToken));

            _cancellationToken = cancellationToken;

            SearchMode = SearchMode.FileName;
            FilePattern = "*";
            DirectoryPattern = "*";
        }

        public IEnumerable<SearchResult> Find(IEnumerable<string> directoryPaths)
        {
            if (directoryPaths == null)
                throw new ArgumentNullException(nameof(directoryPaths));

            return Find2();

            IEnumerable<SearchResult> Find2()
            {
                foreach (string directoryPath in directoryPaths)
                {
                    foreach (SearchResult result in Find(directoryPath))
                        yield return result;
                }
            }
        }

        public IEnumerable<SearchResult> Find(string directoryPath)
        {
            var stack = new Stack<string>();
            stack.Push(directoryPath);

            while (stack.Count > 0)
            {
                string dirPath = stack.Pop();

                if (!Directory.Exists(dirPath))
                    continue;

                var success = false;

                if (SearchMode == SearchMode.FileName)
                {
                    IEnumerator<string> fi = GetFilesEnumerator(dirPath);
                    if (fi != null)
                    {
                        using (fi)
                        {
                            while (fi.MoveNext())
                            {
                                if (IsMatch(fi.Current))
                                {
                                    success = true;
                                    yield return CreateResult(fi.Current, FileSystemEntry.File);
                                }

                                _cancellationToken.ThrowIfCancellationRequested();
                            }
                        }
                    }
                }

                IEnumerator<string> di = GetDirectoriesEnumerator(dirPath);
                if (di != null)
                {
                    using (di)
                    {
                        while (di.MoveNext())
                        {
                            if (SearchMode == SearchMode.DirectoryName && IsMatch(Path.GetFileName(di.Current)))
                                yield return CreateResult(di.Current, FileSystemEntry.Directory);

                            if (SearchOption == SearchOption.AllDirectories && (!RequireMatchForDeeperSearch || success))
                                stack.Push(di.Current);

                            _cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
        }

        private IEnumerator<string> GetFilesEnumerator(string directoryPath)
        {
            try
            {
                return Directory.EnumerateFiles(directoryPath, FilePattern).GetEnumerator();
            }
            catch (Exception ex) when (IsAllowedException(ex))
            {
            }

            return null;
        }

        private IEnumerator<string> GetDirectoriesEnumerator(string directoryPath)
        {
            try
            {
                return Directory.EnumerateDirectories(directoryPath, DirectoryPattern).GetEnumerator();
            }
            catch (Exception ex) when (IsAllowedException(ex))
            {
            }

            return null;
        }

        public bool IsMatch(string filePath)
        {
            if (Regex == null)
                return true;

            switch (FileNamePart)
            {
                case FileNamePart.NameAndExtension:
                    {
                        return Regex.IsMatch(Path.GetFileName(filePath));
                    }
                case FileNamePart.NameWithoutExtension:
                    {
                        return Regex.IsMatch(Path.GetFileNameWithoutExtension(filePath));
                    }
                case FileNamePart.Extension:
                    {
                        string extension = Path.GetExtension(filePath);

                        if (extension.Length > 0 && extension[0] == '.')
                            extension = extension.Substring(1);

                        return Regex.IsMatch(extension);
                    }
                default:
                    {
                        Debug.Fail(string.Format("Missing case for {0}", FileNamePart));
                        return false;
                    }
            }
        }

        internal SearchResult CreateResult(string fullName, FileSystemEntry entry)
        {
            var result = new SearchResult(fullName, entry);

            if (Replacement == null)
                return result;

            switch (FileNamePart)
            {
                case FileNamePart.NameAndExtension:
                    {
                        result.NewName = Replace(result.Name);
                        return result;
                    }
                case FileNamePart.NameWithoutExtension:
                    {
                        string extension = result.Extension;
                        result.NewName = Replace(result.NameWithoutExtension)
                            + ((extension.Length > 0)
                                ? "."
                                : "")
                            + extension;
                        return result;
                    }
                case FileNamePart.Extension:
                    {
                        string extension = Replace(result.Extension);
                        result.NewName = result.NameWithoutExtension + ((extension.Length > 0) ? "." : "") + extension;
                        return result;
                    }
            }

            return result;
        }

        private string Replace(string input)
        {
            return RegexReplacer.Replace(Regex, input, Replacement, ReplacementMode);
        }

        protected virtual bool IsAllowedException(Exception ex)
        {
            return ex is IOException
                || ex is ArgumentException
                || ex is UnauthorizedAccessException
                || ex is SecurityException
                || ex is NotSupportedException;
        }

        public Regex Regex { get; set; }

        public SearchOption SearchOption { get; set; }
        public ReplacementMode ReplacementMode { get; set; }
        public FileNamePart FileNamePart { get; set; }
        public string Replacement { get; set; }

        public SearchMode SearchMode
        {
            get { return _searchMode; }
            set
            {
                if (value == SearchMode.FileContent)
                    throw new NotSupportedException();

                _searchMode = value;
            }
        }

        public string FilePattern { get; set; }
        public string DirectoryPattern { get; set; }
        public bool RequireMatchForDeeperSearch { get; set; }
    }
}
