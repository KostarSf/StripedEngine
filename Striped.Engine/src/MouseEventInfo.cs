using System;
using System.Collections.Generic;
using System.Text;
using Striped.Utils;

namespace Striped.Engine
{
    public class MouseEventInfo
    {
        public Coords Position { get; set; } = new Coords(0, 0);

        public MouseButtons ButtonPressed { get; set; }
        public MouseEvent EventFlag { get; set; }

        internal static MouseButtons GetPressedButton(uint dwButtonState)
        {
            return (MouseButtons)dwButtonState;
        }

        internal static MouseEvent GetEventFlag(uint dwEventFlags)
        {
            return (MouseEvent)dwEventFlags;
        }
    }

    public enum MouseButtons
    {
        None = 0,
        LeftMouseButton = 1,
        RightMouseButton = 2,
        LeftAndRightMouseButton = 3,
        MiddleMouseButton = 4,
        ScrollUp = 7864320,
        ScrollDown = -7864320,
    }

    public enum MouseEvent
    {
        None = 1,
        Pressed = 0,
        PressedTwice = 2,
    }
}
