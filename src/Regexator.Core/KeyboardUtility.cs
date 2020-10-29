// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;

namespace Regexator
{
    public static class KeyboardUtility
    {
        public const int KeyToggled = 0x1;
        public const int KeyPressed = 0x8000;

        public static bool IsKeyDown(Keys key)
        {
            return (GetKeyState(key) & KeyStates.Down) == KeyStates.Down;
        }

        public static bool IsKeyToggled(Keys key)
        {
            return (GetKeyState(key) & KeyStates.Toggled) == KeyStates.Toggled;
        }

        private static KeyStates GetKeyState(Keys key)
        {
            var states = KeyStates.None;

            short value = NativeMethods.GetKeyState((int)key);

            if ((value & KeyToggled) == KeyToggled)
                states |= KeyStates.Toggled;

            if ((value & KeyPressed) == KeyPressed)
                states |= KeyStates.Down;

            return states;
        }
    }
}
