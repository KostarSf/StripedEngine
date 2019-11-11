using Striped.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Striped.Drawing
{
    internal class PixelLine
    {
        public int Length { get; private set; }
        public bool Drawing { get; set; } = true;

        private List<Pixel> pixelLine;
        private List<Pixel> clearLine;

        public PixelLine(int length)
        {
            Length = length;

            createPixelLine();
        }

        private void createPixelLine()
        {
            pixelLine = new List<Pixel>();
            clearLine = new List<Pixel>();

            SetLength(Length);
        }

        internal void SetLength(int length)
        {
            if (Length < length || pixelLine.Count == 0)
            {
                while (pixelLine.Count < length)
                {
                    pixelLine.Add(new Pixel());
                }
            }
            else if (Length > length)
            {
                pixelLine = pixelLine.GetRange(0, length);
            }

            Length = length;
        }

        internal void Add(string line, int startIndex, Colors colors)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (startIndex + i >= 0 && startIndex + i < Length)
                {
                    pixelLine[startIndex + i].Set(line[i], colors);
                }
            }
        }

        internal bool SameAs(PixelLine line)
        {
            if (Length != line.Length) return false;

            for (int i = 0; i < Length; i++)
            {
                if (!pixelLine[i].SameAs(line.pixelLine[i]))
                {
                    return false;
                }
            }

            return true;
        }

        internal int Draw()
        {
            var drawCallsCount = 0;

            if (pixelLine.Count == 0/* || !Drawing*/)
            {
                Console.ForegroundColor = Graphic.DefaultFontColor;
                Console.BackgroundColor = Graphic.DefaultBackColor;

                if (Console.CursorTop < Console.WindowHeight - 1)
                {
                    Console.CursorTop++;
                    Console.CursorLeft = 0;
                }

                return 0;
            }

            if (!Drawing && Graphic.EnableFastDraw) return 0;

            var linePart = new StringBuilder();
            Colors currentColors = pixelLine[0].Colors;
            int lastIndex = (Length <= Console.WindowWidth ? Length : Console.WindowWidth) - 1;

            for (int pixelIndex = 0; pixelIndex <= lastIndex; pixelIndex++)
            {
                if (pixelLine[pixelIndex].Colors.FontColor == pixelLine[pixelIndex].Colors.BackColor)
                {
                    pixelLine[pixelIndex].Set(' ', pixelLine[pixelIndex].Colors);
                }

                if (!pixelLine[pixelIndex].Colors.SameAs(currentColors) &&      // Проверка на полное совпадение цветов
                    !(pixelLine[pixelIndex].Symbol == ' ' &&                                    // Проверка на прозрачный символ и тот же цвет фона
                      pixelLine[pixelIndex].Colors.BackColor == currentColors.BackColor))       //
                {
                    Console.ForegroundColor = currentColors.FontColor;
                    Console.BackgroundColor = currentColors.BackColor;

                    // Визуализация отрисовки строки
                    if (!Graphic.EnableFastDraw && Graphic.ShowLineUpdates)
                    {
                        if (Drawing) { Console.BackgroundColor = ConsoleColor.DarkRed; }
                        else { Console.BackgroundColor = currentColors.BackColor; }
                    }

                    Console.Write(linePart);
                    drawCallsCount++;

                    currentColors = pixelLine[pixelIndex].Colors;
                    linePart.Clear();
                }

                linePart.Append(pixelLine[pixelIndex].Symbol);
                //linePart[0] = '~';    // Показать начало отрисовок

                if (pixelIndex == lastIndex)
                {
                    Console.ForegroundColor = currentColors.FontColor;
                    Console.BackgroundColor = currentColors.BackColor;

                    // Визуализация отрисовки строки
                    if (!Graphic.EnableFastDraw && Graphic.ShowLineUpdates)
                    {
                        if (Drawing) { Console.BackgroundColor = ConsoleColor.DarkRed; }
                        else { Console.BackgroundColor = currentColors.BackColor; }
                    }

                    Console.Write(linePart);
                    drawCallsCount++;
                }
            }

            Console.ForegroundColor = Graphic.DefaultFontColor;
            Console.BackgroundColor = Graphic.DefaultBackColor;

            /* Вернуть, если не получится */

            //if (Console.CursorTop < Console.WindowHeight - 1 && Length < Console.WindowWidth)
            //{
            //    Console.CursorTop++;
            //    Console.CursorLeft = 0;
            //}

            return drawCallsCount;
        }

        internal void Clear()
        {
            pixelLine.Clear();
            SetLength(Length);
        }
    }
}
