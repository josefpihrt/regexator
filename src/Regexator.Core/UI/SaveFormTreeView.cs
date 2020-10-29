// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Regexator.FileSystem;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    internal class SaveFormTreeView : ExtendedTreeView
    {
        public SaveFormTreeView()
        {
            ShowPlusMinus = false;
            ShowLines = false;
            HideSelection = false;
            FullRowSelect = true;
            ShowNodeToolTips = true;
        }

        public void Load(SaveExecutor executor)
        {
            Nodes.AddRange(CreateNodes(executor).ToArray());
            ExpandAll();
        }

        private static IEnumerable<TreeNode> CreateNodes(SaveExecutor executor)
        {
            if (executor.PatternConfirmRequired)
            {
                yield return new TreeNode(
                    Resources.Project
                        + ((executor.ProjectPath != null)
                            ? " " + Resources.HyphenStr + " " + Path.GetFileName(executor.ProjectPath)
                            : ""),
                    GetProjectNodes(executor).ToArray())
                { ToolTipText = executor.ProjectNode.FullName };
            }

            if (executor.InputConfirmRequired)
            {
                yield return new TreeNode(
                    Resources.Input
                        + ((executor.InputFullName != null)
                            ? " " + Resources.HyphenStr + " " + Path.GetFileName(executor.InputFullName)
                            : ""),
                    GetInputNodes(executor).ToArray())
                { ToolTipText = executor.FileInputNode?.FullName };
            }
        }

        private static IEnumerable<TreeNode> GetProjectNodes(SaveExecutor executor)
        {
            return GetProjectNodes(executor.ContainerProps.Except(ProjectContainer.AutoSaveProps)).Where(f => f != null);
        }

        private static IEnumerable<TreeNode> GetProjectNodes(ContainerProps props)
        {
            yield return CreateProjectNode(Resources.Mode, ContainerProps.Mode, props, addBrackets: false);
            yield return CreateProjectNode(Resources.Attributes, ContainerProps.Attributes, props, addBrackets: false);
            yield return CreateProjectNode(Resources.Info, ProjectInfo.AllProps, props);
            yield return CreateProjectNode(Resources.Pattern, Pattern.AllProps, props);
            yield return CreateProjectNode(Resources.Replacement, Replacement.AllProps, props);
            yield return CreateProjectNode(Resources.Output, OutputInfo.AllProps, props);
            yield return CreateProjectNode(
                Resources.FileSystemSearch,
                FileSystemSearchInfo.AllProps,
                props,
                addBrackets: false);
        }

        private static TreeNode CreateProjectNode(string text, ContainerProps props, ContainerProps allProps)
        {
            return CreateProjectNode(text, props, allProps, true);
        }

        private static TreeNode CreateProjectNode(
            string text,
            ContainerProps props,
            ContainerProps allProps,
            bool addBrackets)
        {
            ContainerProps value = props.Intersect(allProps);
            if (value != ContainerProps.None)
            {
                string s = text;

                if (addBrackets)
                {
                    s += " "
                        + string.Join(", ", value.ToValues().Select(f => EnumHelper.GetDescription(f)))
                            .AddParentheses();
                }

                return new TreeNode(s);
            }

            return null;
        }

        private static IEnumerable<TreeNode> GetInputNodes(SaveExecutor executor)
        {
            return executor.InputProps
                .Except(Input.AutoSaveProps)
                .ToValues()
                .Select(f => new TreeNode(EnumHelper.GetDescription(f)))
                .OrderBy(f => f.Text);
        }

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.Cancel = true;
            base.OnBeforeCollapse(e);
        }
    }
}
