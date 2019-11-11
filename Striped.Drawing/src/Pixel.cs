using Striped.Utils;

namespace Striped.Drawing
{
    internal class Pixel
    {
        public char Symbol { get; private set; }
        public Colors Colors { get; private set; }

        public Pixel()
        {
            Symbol = ' ';
            Colors = new Colors(Colors.Color.Default, Colors.Color.Default);
        }

        internal void Set(char symbol, Colors colors)
        {
            Symbol = symbol;
            Colors = colors;
        }

        internal bool SameAs(Pixel pixel)
        {
            return Symbol == pixel.Symbol && Colors.SameAs(pixel.Colors);
        }
    }
}
