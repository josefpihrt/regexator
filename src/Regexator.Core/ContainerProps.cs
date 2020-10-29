// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator
{
    [Flags]
    public enum ContainerProps
    {
        None = 0,
        Attributes = 1,
        Mode = 2,
        Title = 4,
        Author = 8,
        Description = 16,
        Version = 32,
        HelpUrl = 64,
        Keywords = 128,
        PatternText = 256,
        RegexOptions = 512,
        PatternOptions = 1024,
        PatternCurrentLine = 2048,
        ReplacementText = 4096,
        ReplacementOptions = 8192,
        ReplacementNewLine = 16384,
        ReplacementCurrentLine = 32768,
        IgnoredGroups = 65536,
        OutputOptions = 131072,
        FileSystemSearchMode = 262144,
        FileSystemSearchOption = 524288,
        FileSystemFileNamePart = 1048576
    }
}
