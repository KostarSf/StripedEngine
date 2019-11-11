using Striped.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Striped.Drawing
{
    internal class Frame
    {
        private int width;
        private int height;

        private int lastWidth;
        private int lastHeight;

        private List<PixelLine> frame;
        private List<PixelLine> lastFrame;

        internal int DrawCallsCount { get; set; }

        public Frame(int width, int height)
        {
            this.width = width;
            this.height = height;

            createFrame();
        }

        private void createFrame()
        {
            frame = new List<PixelLine>();

            for (int i = 0; i < height; i++)
            {
                frame.Add(new PixelLine(width));
            }
        }

        internal void SetSizes(int width, int height)
        {
            frame = changeSizesOf(frame, width, height);

            this.height = height;
            this.width = width;
        }

        private List<PixelLine> changeSizesOf(List<PixelLine> frame, int width, int height)
        {
            if (height > this.height)
            {
                while (frame.Count < height)
                {
                    frame.Add(new PixelLine(this.width));
                }
            }
            else if (height < this.height)
            {
                frame = frame.GetRange(0, height);
            }

            if (width != this.width)
            {
                foreach (var pixelLine in frame)
                {
                    pixelLine.SetLength(width);
                }
            }

            return frame;
        }

        internal void Add(string line, Coords coords, Colors colors, Origin origin)
        {
            Coords margin = getMargins(line, origin);

            if (margin.Y + coords.Y >= 0 && margin.Y + coords.Y < height)
            {
                frame[margin.Y + coords.Y].Add(line, margin.X + coords.X, colors);
            }
        }

        internal void AddLine(Coords startPoint, Coords endPoint, string fill, Colors colors, Origin origin)
        {
            int fillIndex = 0;

            void Swap(ref int a, ref int b)
            {
                int tmp = a;
                a = b;
                b = tmp;
            }

            int x0 = startPoint.X;
            int x1 = endPoint.X;
            int y0 = startPoint.Y;
            int y1 = endPoint.Y;

            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1; 
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                Add(fill[fillIndex].ToString(), new Coords(steep ? y : x, steep ? x : y), colors, origin);
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }

                if (fillIndex < fill.Length - 1) fillIndex++;
                else fillIndex = 0;
            }
        }

        private Coords getMargins(string line, Origin origin)
        {
            var margin = Coords.Zero;

            switch (origin.Horizontal)
            {
                case Origin.HorizontalOrigin.Center:
                    margin.X = width / 2 - line.Length / 2;
                    break;
                case Origin.HorizontalOrigin.Right:
                    margin.X = width - line.Length - 0;
                    break;
            }

            switch (origin.Vertical)
            {
                case Origin.VerticalOrigin.Middle:
                    margin.Y = height / 2;
                    break;
                case Origin.VerticalOrigin.Bottom:
                    margin.Y = height - 1;
                    break;
            }

            return margin;
        }

        internal void Clear()
        {
            createFrame();

            //foreach (var pixelLine in frame)
            //{
            //    pixelLine.Clear();
            //}
        }

        internal void Draw()
        {
            DrawCallsCount = 0;

            var currentFrame = new List<PixelLine>(frame);

            if (currentFrame.Count == 0) return;

            msWindowsFix();

            if (lastWidth != width || lastHeight != height)
            {
                Console.Clear();
            }
            else if (lastFrame != null)
            {
                int minLineCount = lastFrame.Count > currentFrame.Count ? currentFrame.Count : lastFrame.Count;

                //for (int i = 0; i < minLineCount; i++)
                //{
                //    if (currentFrame[i].SameAs(lastFrame[i]))
                //    {
                //        currentFrame[i].Drawing = false;
                //    }
                //}
            }

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            for (int lineIndex = 0; lineIndex < Console.WindowHeight; lineIndex++)
            {
                if (lineIndex > height - 1) break;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    && lineIndex == Console.WindowHeight - 1 && width >= Console.WindowWidth - 1)
                {
                    currentFrame[lineIndex].SetLength(Console.WindowWidth - 1);
                }
                else
                {
                    currentFrame[lineIndex].SetLength(width);
                }

                if (lastFrame != null && lineIndex < lastFrame.Count &&
                    currentFrame[lineIndex].SameAs(lastFrame[lineIndex]))
                {
                    currentFrame[lineIndex].Drawing = false;
                }

                if (lineIndex <= Console.WindowHeight - 1)
                {
                    Console.SetCursorPosition(0, lineIndex);
                    DrawCallsCount += currentFrame[lineIndex].Draw();
                }
            }

            lastWidth = width;
            lastHeight = height;

            lastFrame = new List<PixelLine>(frame);
        }

        private void msWindowsFix()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (lastHeight == height)
                {
                    // Console.BufferHeight = Console.WindowHeight;
                    // Console.WindowWidth = Console.BufferWidth;
                }
            }
        }

    }
}
