// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text
{
    public interface ITextContainer
    {
        string Text { get; set; }
        string SelectedText { get; set; }
        int SelectionStart { get; set; }
        int SelectionLength { get; set; }

        void SelectText(int start, int length);
    }
}
