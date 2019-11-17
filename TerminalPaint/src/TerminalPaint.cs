using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Striped.Drawing;
using Striped.Engine;
using Striped.Utils;

namespace TestApp
{
    class PaintPixel
    {
        public Coords Position { get; set; }
        public Colors Color { get; set; }
        public char Char { get; set; }
    }

    class Brush
    {
        public Colors BrushColor { get; set; }
        public Colors FirstColor { get; set; }
        public Colors SecondColor { get; set; }
        public char Char { get; set; }
        public bool Erase { get; set; }
    }

    class HighlightColor
    {
        public Colors Color { get; set; }
        public string Text { get; set; }
        public Coords Coords { get; set; }
    }

    internal class TerminalPaint : Core
    {
        Brush brush = new Brush
        {
            Char = ' ',
            FirstColor = new Colors(Colors.Color.Default, Colors.Color.White),
            SecondColor = new Colors(Colors.Color.Default, Colors.Color.Black),
            BrushColor = new Colors(Colors.Color.Default, Colors.Color.White),
            Erase = true,
        };

        HighlightColor highlight = new HighlightColor
        {
            Color = new Colors(Colors.Color.Green, Colors.Color.Default),
            Text = "(8)",
            Coords = new Coords(69, 0),
        };

        List<PaintPixel> paintPixels = new List<PaintPixel>();
        List<PaintPixel> tempPixels = new List<PaintPixel>();

        bool showDebug;

        bool justSaved;
        bool justLoaded;
        bool clearTempPaint;

        bool showHelp;

        public TerminalPaint()
        {
            Title = "KostarSf's Terminal Paint";

            TickRate = 60;
            FrameRate = 60;

            EnableNativeInput = true;

            Graphic.EnableFastDraw = true;
            Graphic.ShowLineUpdates = false;

            this.Start();
        }

        public override void OnMouseEvent(MouseEventInfo e)
        {
            switch (e.ButtonPressed)
            {
                case MouseButtons.None:
                    {
                        if (tempPixels.Count > 0)
                        {
                            if (!clearTempPaint)
                            {
                                for (int i = paintPixels.Count - 1; i >= 0; i--)
                                {
                                    foreach (var coords in tempPixels)
                                    {
                                        if (paintPixels[i].Position.SameAs(coords.Position))
                                        {
                                            paintPixels.RemoveAt(i);
                                            break;
                                        }
                                    }
                                }

                                if (!brush.Erase)
                                {
                                    paintPixels.AddRange(tempPixels);
                                }
                            }
                            else
                            {
                                clearTempPaint = false;
                            }

                            tempPixels.Clear();
                        }
                    }
                    break;
                case MouseButtons.RightMouseButton:
                    {
                        if (brush.SecondColor.BackColor == ConsoleColor.Black)
                        {
                            brush.Erase = true;
                        }
                        else
                        {
                            brush.Erase = false;
                        }

                        DrawColor(brush.SecondColor, e.Position);
                    }
                    break;
                case MouseButtons.LeftMouseButton:
                    {
                        brush.Erase = false;
                        DrawColor(brush.FirstColor, e.Position);
                    }
                    break;
                case MouseButtons.MiddleMouseButton:
                    {
                        paintPixels.Clear();
                    }
                    break;
                case MouseButtons.LeftAndRightMouseButton:
                    {
                        tempPixels.Clear();
                        clearTempPaint = true;
                    }
                    break;
            }
        }

        private void DrawColor(Colors color, Coords coords)
        {
            for (int i = tempPixels.Count - 1; i >= 0; i--)
            {
                if (tempPixels[i].Position.SameAs(coords))
                {
                    tempPixels.RemoveAt(i);
                }
            }

            if (coords.IsInRectangle(new Coords(3, 2), new Coords(Graphic.Width - 4, Graphic.Height - 3)))
            {
                if (tempPixels.Count == 0)
                {
                    tempPixels.Add(new PaintPixel { Char = ' ', Color = new Colors(color), Position = coords });
                }
                else
                {
                    var horDistance = Coords.HorisontalDistance(tempPixels[tempPixels.Count - 1].Position, coords);
                    var verDistance = Coords.VerticalDistance(tempPixels[tempPixels.Count - 1].Position, coords);

                    if (horDistance > 1 || verDistance > 1)
                    {
                        var interpolatePath = Coords.GetPath(tempPixels[tempPixels.Count - 1].Position, coords);
                        interpolatePath.RemoveAt(0);

                        foreach (var _coords in interpolatePath)
                        {
                            tempPixels.Add(new PaintPixel { Char = ' ', Color = new Colors(color), Position = _coords });
                        }
                    }
                    else
                    {
                        tempPixels.Add(new PaintPixel { Char = ' ', Color = new Colors(color), Position = coords });
                    }
                }
            }
        }

        private void RemovePixel(Coords coords)
        {
            for (int i = paintPixels.Count - 1; i >= 0; i--)
            {
                if (paintPixels[i].Position.SameAs(coords))
                {
                    paintPixels.RemoveAt(i);
                }
            }
        }

        public override void OnKeyPress(KeyEventInfo e)
        {
            if (e.Pressed)
            {
                switch (e.Key)
                {
                    case ConsoleKey.Q:
                        {
                            showDebug = !showDebug;
                        }
                        break;
                    case ConsoleKey.H:
                        {
                            showHelp = !showHelp;
                        }
                        break;
                    case ConsoleKey.D1:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.Red), new Colors(Colors.Color.Default, Colors.Color.DarkRed));
                            highlight.Coords.X = 13;
                            highlight.Text = "(1)";
                        }
                        break;
                    case ConsoleKey.D2:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.Yellow), new Colors(Colors.Color.Default, Colors.Color.DarkYellow));
                            highlight.Coords.X = 21;
                            highlight.Text = "(2)";
                        }
                        break;
                    case ConsoleKey.D3:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.Green), new Colors(Colors.Color.Default, Colors.Color.DarkGreen));
                            highlight.Coords.X = 29;
                            highlight.Text = "(3)";
                        }
                        break;
                    case ConsoleKey.D4:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.Cyan), new Colors(Colors.Color.Default, Colors.Color.DarkCyan));
                            highlight.Coords.X = 37;
                            highlight.Text = "(4)";
                        }
                        break;
                    case ConsoleKey.D5:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.Blue), new Colors(Colors.Color.Default, Colors.Color.DarkBlue));
                            highlight.Coords.X = 45;
                            highlight.Text = "(5)";
                        }
                        break;
                    case ConsoleKey.D6:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.Magenta), new Colors(Colors.Color.Default, Colors.Color.DarkMagenta));
                            highlight.Coords.X = 53;
                            highlight.Text = "(6)";
                        }
                        break;
                    case ConsoleKey.D7:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.Gray), new Colors(Colors.Color.Default, Colors.Color.DarkGray));
                            highlight.Coords.X = 61;
                            highlight.Text = "(7)";
                        }
                        break;
                    case ConsoleKey.D8:
                        {
                            ChangePaintColors(new Colors(Colors.Color.Default, Colors.Color.White), new Colors(Colors.Color.Default, Colors.Color.Black));
                            highlight.Coords.X = 69;
                            highlight.Text = "(8)";
                        }
                        break;
                    case ConsoleKey.Enter:
                        {
                            string output = JsonConvert.SerializeObject(paintPixels/*, Formatting.Indented*/);

                            if (!Directory.Exists("./temp"))
                            {
                                Directory.CreateDirectory("./temp");
                            }

                            FileInfo fi = new FileInfo(Environment.CurrentDirectory + "/temp/draving.json");

                            if (fi.Exists) fi.Delete();

                            using (StreamWriter sw = fi.CreateText())
                            {
                                sw.Write(output);
                            }

                            if (File.Exists("./save.tp"))
                            {
                                File.Delete("./save.tp");
                            }

                            ZipFile.CreateFromDirectory("./temp", "./save.tp", CompressionLevel.Fastest, false);

                            if (Directory.Exists("./temp"))
                            {
                                Directory.Delete("./temp", true);
                            }

                            justSaved = true;
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        {
                            string input = "";

                            if (!File.Exists("./save.tp")) return;

                            if (Directory.Exists("./temp"))
                            {
                                Directory.Delete("./temp", true);
                            }

                            ZipFile.ExtractToDirectory("./save.tp", "./temp");

                            var fi = new FileInfo("./temp/draving.json");

                            if (!fi.Exists) return;

                            using (StreamReader sw = fi.OpenText())
                            {
                                input = sw.ReadToEnd();
                            }

                            if (Directory.Exists("./temp"))
                            {
                                Directory.Delete("./temp", true);
                            }

                            paintPixels = JsonConvert.DeserializeObject<List<PaintPixel>>(input);

                            justLoaded = true;
                        }
                        break;
                    case ConsoleKey.Escape:
                        {
                            this.Stop();
                        }
                        break;
                }
            }
        }

        private void ChangePaintColors(Colors firstColor, Colors secondColor)
        {
            brush.FirstColor = firstColor;
            brush.SecondColor = secondColor;
            brush.BrushColor = firstColor;
        }

        public override void OnUpdate()
        {

        }

        public override void OnDraw()
        {
            Graphic.Clear();
            Graphic.FitSizesToWindow();

            var temp1 = GetSafePixelsList(paintPixels);

            for (int i = 0; i < temp1.Count; i++)
            {
                Graphic.Add(temp1[i].Char.ToString(), temp1[i].Position, temp1[i].Color);
            }

            var temp2 = GetSafePixelsList(tempPixels);

            for (int i = 0; i < temp2.Count; i++)
            {
                Graphic.Add(temp2[i].Char.ToString(), temp2[i].Position, temp2[i].Color);
            }
            temp2.Clear();

            if (justSaved || justLoaded)
            {
                if (justSaved)
                {
                    Graphic.AddRectangle(" ", new Colors(Colors.Color.Default, Colors.Color.Green), new Coords(3, 2), new Coords(Graphic.Width - 4, Graphic.Height - 3));
                    justSaved = false;
                }

                if (justLoaded)
                {
                    Graphic.AddRectangle(" ", new Colors(Colors.Color.Default, Colors.Color.Gray), new Coords(3, 2), new Coords(Graphic.Width - 4, Graphic.Height - 3));
                    justLoaded = false;
                }
            }

            drawInterface();
            drawHelp();
            drawDebug();

            if (MousePosition.IsInRectangle(new Coords(3, 2), new Coords(Graphic.Width - 4, Graphic.Height - 3)))
            {
                Graphic.Add(" ", MousePosition, brush.BrushColor);
            }

            Graphic.Draw();
        }

        private List<PaintPixel> GetSafePixelsList(List<PaintPixel> listPixels)
        {
            List<PaintPixel> outList = new List<PaintPixel>(Console.WindowWidth * Console.WindowHeight);
            outList.AddRange(listPixels);

            for (int i = outList.Count - 1; i >= 0; i--)
            {
                if (outList[i] == null)
                {
                    outList.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            return outList;
        }

        void drawInterface()
        {
            Graphic.AddRectangleBorder(" ", " ", " ", " ", " ", " ", " ", " ", Colors.Default, Coords.Zero, new Coords(Graphic.Width - 1, Graphic.Height - 1));
            Graphic.AddRectangleBorder(" ", " ", " ", " ", " ", " ", " ", " ", Colors.Default, new Coords(1, 0), new Coords(Graphic.Width - 2, Graphic.Height - 1));

            Graphic.AddRectangleBorder("═", "║", "═", "║", "╔", "╗", "╝", "╚", Colors.Default, new Coords(2, 1), new Coords(Graphic.Width - 3, Graphic.Height - 2));

            Graphic.Add(" Debug: Q ", new Coords(1, 0), new Colors(Colors.Color.DarkGray, Colors.Color.Default));

            if (MousePosition.IsInRectangle(new Coords(3, 2), new Coords(Graphic.Width - 4, Graphic.Height - 3)))
            {
                Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Center, Origin.VerticalOrigin.Top));
                Graphic.Add($"{MousePosition.X - 2}", new Coords(-2, 0));
                Graphic.Add($"{MousePosition.Y - 1}", new Coords(2, 0));
            }

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Right, Origin.VerticalOrigin.Top));
            Graphic.Add(" Save: ENT, Load: SPC ", new Coords(-1, 0));

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Left, Origin.VerticalOrigin.Bottom));
            Graphic.Add(" Colors:", new Coords(1, 0));
            Graphic.Add("    (1),    (2),    (3),    (4),    (5),    (6),    (7),    (8).", new Coords(9, 0), new Colors(Colors.Color.DarkGray, Colors.Color.Default));
            Graphic.Add(highlight.Text, highlight.Coords, highlight.Color);
            Graphic.Add(" ", new Coords(10, 0), new Colors(Colors.Color.Default, Colors.Color.Red));
            Graphic.Add(" ", new Coords(11, 0), new Colors(Colors.Color.Default, Colors.Color.DarkRed));
            Graphic.Add(" ", new Coords(18, 0), new Colors(Colors.Color.Default, Colors.Color.Yellow));
            Graphic.Add(" ", new Coords(19, 0), new Colors(Colors.Color.Default, Colors.Color.DarkYellow));
            Graphic.Add(" ", new Coords(26, 0), new Colors(Colors.Color.Default, Colors.Color.Green));
            Graphic.Add(" ", new Coords(27, 0), new Colors(Colors.Color.Default, Colors.Color.DarkGreen));
            Graphic.Add(" ", new Coords(34, 0), new Colors(Colors.Color.Default, Colors.Color.Cyan));
            Graphic.Add(" ", new Coords(35, 0), new Colors(Colors.Color.Default, Colors.Color.DarkCyan));
            Graphic.Add(" ", new Coords(42, 0), new Colors(Colors.Color.Default, Colors.Color.Blue));
            Graphic.Add(" ", new Coords(43, 0), new Colors(Colors.Color.Default, Colors.Color.DarkBlue));
            Graphic.Add(" ", new Coords(50, 0), new Colors(Colors.Color.Default, Colors.Color.Magenta));
            Graphic.Add(" ", new Coords(51, 0), new Colors(Colors.Color.Default, Colors.Color.DarkMagenta));
            Graphic.Add(" ", new Coords(58, 0), new Colors(Colors.Color.Default, Colors.Color.Gray));
            Graphic.Add(" ", new Coords(59, 0), new Colors(Colors.Color.Default, Colors.Color.DarkGray));
            Graphic.Add(" ", new Coords(66, 0), new Colors(Colors.Color.Default, Colors.Color.White));
            Graphic.Add(" ", new Coords(67, 0), new Colors(Colors.Color.Default, Colors.Color.Black));

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Right, Origin.VerticalOrigin.Bottom));
            Graphic.Add(" First: LMB, Second: RMB, Clear: MMB ", new Coords(-1, 0));

            Graphic.SetOrigin(Origin.Default);
        }

        void drawHelp()
        {
            if (!showHelp) return;

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Center, Origin.VerticalOrigin.Middle));

            Graphic.AddRectangle(" ", Colors.Default, new Coords(0, -5), new Coords(30, 4));
            Graphic.AddRectangleBorder("═", "║", "═", "║", "╔", "╗", "╝", "╚", Colors.Default, new Coords(-15, -5), new Coords(15, 4));

            Graphic.Add("Terminal Paint v1.0 100", new Coords(0, -3));
            Graphic.Add("by KostarSf", new Coords(0, -2));

            Graphic.Add("powered by", new Coords(0, 1));
            Graphic.Add("StripedEngine", new Coords(0, 2));

            Graphic.SetOrigin(Origin.Default);
        }

        void drawDebug()
        {
            if (!showDebug) return;

            Graphic.AddRectangle(" ", Colors.Default, new Coords(2, 1), new Coords(27, 7));
            Graphic.AddRectangleBorder("═", "│", "─", "║", "╔", "╤", "┘", "╟", Colors.Default, new Coords(2, 1), new Coords(27, 7));

            Graphic.Add($"Tick rate: .....: {CurrentTickRate}/{TickRate}", new Coords(3, 2));
            Graphic.Add($"Frame rate: ....: {CurrentFrameRate}/{FrameRate}", new Coords(3, 3));
            Graphic.Add($"Draw calls: ....: {Graphic.LastDrawCallsCount}", new Coords(3, 4));
            Graphic.Add($"Pixels painted .: {paintPixels.Count}", new Coords(3, 5));
            Graphic.Add($"Pixels in temp .: {tempPixels.Count}", new Coords(3, 6));

            Graphic.SetOrigin(Origin.Default);
        }
    }
}
