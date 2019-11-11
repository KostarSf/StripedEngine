using System;
using System.Collections.Generic;
using System.Text;

namespace Striped.Drawing
{
    public class WindowBorder
    {
        public static WindowBorder Default { get; } = new WindowBorder();

        public string LineTop { get; set; } = "═";
        public string LineLeft { get; set; } = "│";
        public string LineRight { get; set; } = "│";
        public string LineBottom { get; set; } = "─";
        public string CornerTopLeft { get; set; } = "╒";
        public string CornerTopRight { get; set; } = "╕";
        public string CornerBottomLeft { get; set; } = "└";
        public string CornerBottomRight { get; set; } = "┘";

        public WindowBorder() { }
    }
}
