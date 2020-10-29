// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.FileSystem
{
    public interface IInputContainer
    {
        Input Input { get; set; }
        ProjectNode ProjectNode { get; }
        string FullName { get; }
        string FileName { get; }
        NodeImageIndex NodeImageIndex { get; }
        NodeImageIndex OpenNodeImageIndex { get; }
    }
}
