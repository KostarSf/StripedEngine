using System;

namespace Striped.Engine
{
    public class KeyEventInfo
    {
        public uint dwControlKeyState;

        public bool Pressed { get; set; }
        public char KeyChar { get; set; }
        public ConsoleKey Key { get; set; }
        public KeyModifiers Modifiers { get; set; }
        public ConsoleModifiers ConsoleModifiers { get; set; }

        internal static ConsoleKey GetConsoleKey(ushort wVirtualKeyCode)
        {
            return (ConsoleKey)wVirtualKeyCode;
        }

        internal static KeyModifiers GetModifiers(uint dwControlKeyState)
        {
            return (KeyModifiers)dwControlKeyState;
        }

        internal static KeyModifiers GetModifiers(ConsoleModifiers modifiers)
        {
            switch (modifiers)
            {
                case ConsoleModifiers.Alt:
                    return KeyModifiers.Alt;
                case ConsoleModifiers.Shift:
                    return KeyModifiers.Shift;
                case ConsoleModifiers.Control:
                    return KeyModifiers.Control;
                default:
                    return KeyModifiers.None;
            }
        }
    }

    public enum KeyModifiers
    {
        None = 32,
        Alt = 34,
        Shift = 48,
        ShiftAlt = 50,
        Control = 40,
        ControlAlt = 42,
        ControlShift = 56,
        ControlShiftAlt = 58,
    }
}