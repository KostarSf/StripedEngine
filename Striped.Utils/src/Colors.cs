using System;

namespace Striped.Utils
{
    public class Colors
    {
        private Colors brushColor;

        public static Colors Default { get; set; } = new Colors(Color.White, Color.Black);

        public enum Color
        {
            Black = 0,
            DarkBlue = 1,
            DarkGreen = 2,
            DarkCyan = 3,
            DarkRed = 4,
            DarkMagenta = 5,
            DarkYellow = 6,
            Gray = 7,
            DarkGray = 8,
            Blue = 9,
            Green = 10,
            Cyan = 11,
            Red = 12,
            Magenta = 13,
            Yellow = 14,
            White = 15,
            Default = 16,
        }

        public ConsoleColor FontColor { get; set; }
        public ConsoleColor BackColor { get; set; }

        public Colors() { }

        public Colors(ConsoleColor fontColor, ConsoleColor backColor)
        {
            FontColor = fontColor;
            BackColor = backColor;
        }

        public Colors(Color fontColor, Color backColor)
        {
            Set(fontColor, backColor);
        }

        public Colors(Colors colors)
        {
            FontColor = colors.FontColor;
            BackColor = colors.BackColor;
        }

        public void Set(Color fontColor, Color backColor)
        {
            SetFont(fontColor);
            SetBack(backColor);
        }

        public void SetFont(Color fontColor)
        {
            switch (fontColor)
            {
                case Color.Default:
                    FontColor = Default.FontColor;
                    break;
                default:
                    FontColor = (ConsoleColor)fontColor;
                    break;
            }
        }

        public void SetBack(Color backColor)
        {
            switch (backColor)
            {
                case Color.Default:
                    BackColor = Default.BackColor;
                    break;
                default:
                    BackColor = (ConsoleColor)backColor;
                    break;
            }
        }

        public bool SameAs(Colors colors)
        {
            return FontColor == colors.FontColor && BackColor == colors.BackColor;
        }
    }
}
