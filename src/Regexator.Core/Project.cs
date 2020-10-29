// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Regexator.Collections.Generic;
using Regexator.FileSystem;

namespace Regexator
{
    [Serializable]
    public class Project : ICloneable
    {
        [DebuggerStepThrough]
        public Project()
            : this(new ProjectContainer())
        {
        }

        [DebuggerStepThrough]
        public Project(ProjectContainer container)
            : this(container, null)
        {
        }

        [DebuggerStepThrough]
        public Project(ProjectContainer container, string filePath)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            FilePath = filePath;
            FileInputs = new InputCollection();
            Inputs = new InputCollection();
        }

        public void Save(FileStream stream)
        {
            Xml.Serialization.Projects.ProjectSerializer.Default.Serialize(stream, this);
        }

        public void Save(string filePath)
        {
            Xml.Serialization.Projects.ProjectSerializer.Default.Serialize(filePath, this);
        }

        public object Clone()
        {
            var project = new Project((ProjectContainer)Container.Clone(), FilePath)
            {
                InputName = InputName,
                InputNameKind = InputNameKind
            };
            project.FileInputs.AddRange(FileInputs.ToClones());
            project.Inputs.AddRange(Inputs.ToClones());
            return project;
        }

        public string GetNewInputName()
        {
            return GetNewInputName(Resources.Input);
        }

        public string GetNewInputName(string initialName)
        {
            string name = (!string.IsNullOrWhiteSpace(initialName)) ? initialName : Resources.Input;
            string newName = name;
            int i = 1;
            while (ContainsInput(newName))
            {
                i++;
                newName = name + i.ToString(CultureInfo.CurrentCulture);
            }

            return newName;
        }

        public string GetInputNameCopy(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var generator = new CopySuffixGenerator();
            while (true)
            {
                string newName = name + generator.Suffix;
                if (!ContainsInput(newName))
                    return newName;

                generator.Increment();
            }
        }

        public Input FindInput(string name)
        {
            return Inputs.FirstOrDefault(f => Input.NameEquals(f.Name, name));
        }

        public bool ContainsInput(string name)
        {
            return FindInput(name) != null;
        }

        public Input[] GetAllInputs()
        {
            return EnumerateAllInputs.ToArray();
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value ?? ""; }
        }

        public string InputName
        {
            get { return _inputName; }
            set { _inputName = value ?? ""; }
        }

        public IEnumerable<Input> EnumerateAllInputs
        {
            get { return Inputs.Concat(FileInputs); }
        }

        public bool HasInputOrFileInput
        {
            get { return Inputs.Count > 0 || FileInputs.Count > 0; }
        }

        public ProjectContainer Container
        {
            get { return _container; }
            set { _container = value ?? throw new ArgumentNullException("value"); }
        }

        public InputCollection FileInputs { get; }
        public InputCollection Inputs { get; }
        public InputKind InputNameKind { get; set; }

        private ProjectContainer _container;
        private string _filePath;
        private string _inputName = "";
    }
}
