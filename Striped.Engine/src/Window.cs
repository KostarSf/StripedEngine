using Striped.Drawing;
using Striped.Utils;

namespace Striped.Engine
{
    public class Window : IDrawable
    {
        public Coords Position { get; set; } = Coords.Zero;
        public int Width { get; set; } = 30;
        public int Height { get; set; } = 10;
        public WindowBorder Border { get; set; } = WindowBorder.Default;
        public WindowControl Control { get; set; } = WindowControl.Close;
        public WindowHeader Header { get; set; } = new WindowHeader();
        public WindowBody Body { get; set; } = new WindowBody();
        public bool ShowHeader { get; set; } = true;
        public bool CanResize { get; set; } = true;
        public bool CanDrag { get; set; } = true;
        public Colors BorderColor { get; set; } = Colors.Default;
        public bool Active { get; set; } = true;
        public bool Hide { get; set; } = false;

        public Window() { }

        public void OnDraw()
        {
            Graphic.AddRectangleBorder(Border, BorderColor, Position, new Coords(Position.X + Width, Position.Y + Height));

            switch (Control)
            {
                case WindowControl.HideMaximizeClose:
                    {
                        Graphic.Add("-", new Coords(Position.X + Width - 3, Position.Y));
                        Graphic.Add("+", new Coords(Position.X + Width - 2, Position.Y));
                        Graphic.Add("x", new Coords(Position.X + Width - 1, Position.Y));
                    }
                    break;
                case WindowControl.HideClose:
                    {
                        Graphic.Add("-", new Coords(Position.X + Width - 2, Position.Y));
                        Graphic.Add("x", new Coords(Position.X + Width - 1, Position.Y));
                    }
                    break;
                case WindowControl.Close:
                    {
                        Graphic.Add("x", new Coords(Position.X + Width - 1, Position.Y));
                    }
                    break;
            }
        }
    }

    public enum WindowControl
    {
        Close,
        HideClose,
        HideMaximizeClose,
        None,
    }
}