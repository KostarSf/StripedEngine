namespace Striped.Utils
{
    public class Origin
    {
        public static Origin Default { get => new Origin(HorizontalOrigin.Left, VerticalOrigin.Top); }

        public enum HorizontalOrigin
        {
            Left = -1,
            Center = 0,
            Right = 1,
        }

        public enum VerticalOrigin
        {
            Top = -1,
            Middle = 0,
            Bottom = 1,
        }

        public HorizontalOrigin Horizontal { get; private set; }
        public VerticalOrigin Vertical { get; private set; }

        public Origin(HorizontalOrigin horizontal, VerticalOrigin vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }
}