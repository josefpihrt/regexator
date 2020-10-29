// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;

namespace Regexator
{
    [Serializable]
    public class ProjectContainer : ICloneable
    {
        public ProjectContainer()
        {
            ProjectInfo = new ProjectInfo();
            Pattern = new Pattern();
            Replacement = new Replacement();
            OutputInfo = new OutputInfo();
            FileSystemSearchInfo = new FileSystemSearchInfo();
        }

        public static ContainerProps GetChangedProps(ProjectContainer first, ProjectContainer second, ContainerProps props)
        {
            if (first == null || second == null || ReferenceEquals(first, second))
                return ContainerProps.None;

            var value = ContainerProps.None;

            if (props.Contains(ContainerProps.Attributes) && first.Attributes != second.Attributes)
                value |= ContainerProps.Attributes;

            if (props.Contains(ContainerProps.Mode) && first.Mode != second.Mode)
                value |= ContainerProps.Mode;

            value |= ProjectInfo.GetChangedProps(first.ProjectInfo, second.ProjectInfo, props);
            value |= Pattern.GetChangedProps(first.Pattern, second.Pattern, props);
            value |= Replacement.GetChangedProps(first.Replacement, second.Replacement, props);
            value |= OutputInfo.GetChangedProps(first.OutputInfo, second.OutputInfo, props);
            value |= FileSystemSearchInfo.GetChangedProps(first.FileSystemSearchInfo, second.FileSystemSearchInfo, props);
            return value;
        }

        public object Clone()
        {
            return new ProjectContainer()
            {
                Mode = Mode,
                Attributes = Attributes,
                ProjectInfo = (ProjectInfo)ProjectInfo.Clone(),
                Pattern = (Pattern)Pattern.Clone(),
                Replacement = (Replacement)Replacement.Clone(),
                OutputInfo = (OutputInfo)OutputInfo.Clone(),
                FileSystemSearchInfo = (FileSystemSearchInfo)FileSystemSearchInfo.Clone()
            };
        }

        public ProjectInfo ProjectInfo
        {
            get { return _projectInfo; }
            set { _projectInfo = value ?? throw new ArgumentNullException("value"); }
        }

        public Pattern Pattern
        {
            get { return _pattern; }
            set { _pattern = value ?? throw new ArgumentNullException("value"); }
        }

        public Replacement Replacement
        {
            get { return _replacement; }
            set { _replacement = value ?? throw new ArgumentNullException("value"); }
        }

        public OutputInfo OutputInfo
        {
            get { return _outputInfo; }
            set { _outputInfo = value ?? throw new ArgumentNullException("value"); }
        }

        public FileSystemSearchInfo FileSystemSearchInfo
        {
            get { return _fileSystemSearchInfo; }
            set { _fileSystemSearchInfo = value ?? throw new ArgumentNullException("value"); }
        }

        private static ContainerProps CreateAllProps()
        {
            var value = ContainerProps.None;

            foreach (ContainerProps item in Enum.GetValues(typeof(ContainerProps)).Cast<ContainerProps>())
                value |= item;

            return value;
        }

        public static ContainerProps AllProps
        {
            get
            {
                if (_allProps == ContainerProps.None)
                    _allProps = CreateAllProps();

                return _allProps;
            }
        }

        public static ContainerProps AutoSaveProps
        {
            get { return ContainerProps.PatternCurrentLine | ContainerProps.ReplacementCurrentLine; }
        }

        public EvaluationMode Mode { get; set; }
        public ItemAttributes Attributes { get; set; }

        private ProjectInfo _projectInfo;
        private Pattern _pattern;
        private Replacement _replacement;
        private OutputInfo _outputInfo;
        private FileSystemSearchInfo _fileSystemSearchInfo;
        private static ContainerProps _allProps;
    }
}
