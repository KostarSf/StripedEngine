using System;
using System.Collections.Generic;
using System.IO;
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

    internal class TerminalPaint : GameCore
    {

        Colors cursorColor = new Colors(Colors.Color.Default, Colors.Color.DarkGreen);
        Colors brushColor = new Colors(Colors.Color.Default, Colors.Color.Green);

        Colors oldCurorColors = new Colors(Colors.Color.Default, Colors.Color.DarkGreen);

        List<PaintPixel> paintPixels = new List<PaintPixel>();
        List<PaintPixel> tempPixels = new List<PaintPixel>();

        bool showDebug;

        bool justSaved;
        bool justLoaded;
        bool clearTempPaint;

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
                        cursorColor = new Colors(oldCurorColors);
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

                                paintPixels.AddRange(tempPixels);
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
                        cursorColor.BackColor = ConsoleColor.DarkCyan;
                        for (int i = paintPixels.Count - 1; i >= 0; i--)
                        {
                            if (paintPixels[i].Position.SameAs(e.Position))
                            {
                                paintPixels.RemoveAt(i);
                            }
                        }
                    }
                    break;
                case MouseButtons.LeftMouseButton:
                    {
                        cursorColor.BackColor = ConsoleColor.Cyan;

                        for (int i = tempPixels.Count - 1; i >= 0; i--)
                        {
                            if (tempPixels[i].Position.SameAs(e.Position))
                            {
                                tempPixels.RemoveAt(i);
                            }
                        }

                        if (e.Position.IsInRectangle(new Coords(3, 2), new Coords(Graphic.Width - 4, Graphic.Height - 3)))
                        {
                            if (tempPixels.Count == 0)
                            {
                                tempPixels.Add(new PaintPixel { Char = ' ', Color = new Colors(brushColor), Position = e.Position });
                            }
                            else
                            {
                                var horDistance = Coords.HorisontalDistance(tempPixels[tempPixels.Count - 1].Position, e.Position);
                                var verDistance = Coords.VerticalDistance(tempPixels[tempPixels.Count - 1].Position, e.Position);

                                if (horDistance > 1 || verDistance > 1)
                                {
                                    var interpolatePath = Coords.GetPath(tempPixels[tempPixels.Count - 1].Position, e.Position);
                                    interpolatePath.RemoveAt(0);

                                    foreach (var coords in interpolatePath)
                                    {
                                        tempPixels.Add(new PaintPixel { Char = ' ', Color = new Colors(brushColor), Position = coords });
                                    }
                                }
                                else
                                {
                                    tempPixels.Add(new PaintPixel { Char = ' ', Color = new Colors(brushColor), Position = e.Position });
                                }
                            }
                        }
                    }
                    break;
                case MouseButtons.MiddleMouseButton:
                    {
                        cursorColor.BackColor = ConsoleColor.Gray;
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
                    case ConsoleKey.D1:
                        {
                            brushColor.BackColor = ConsoleColor.Red;
                            cursorColor.BackColor = ConsoleColor.DarkRed;
                            oldCurorColors.BackColor = ConsoleColor.DarkRed;
                        }
                        break;
                    case ConsoleKey.D2:
                        {
                            brushColor.BackColor = ConsoleColor.Yellow;
                            cursorColor.BackColor = ConsoleColor.DarkYellow;
                            oldCurorColors.BackColor = ConsoleColor.DarkYellow;
                        }
                        break;
                    case ConsoleKey.D3:
                        {
                            brushColor.BackColor = ConsoleColor.Green;
                            cursorColor.BackColor = ConsoleColor.DarkGreen;
                            oldCurorColors.BackColor = ConsoleColor.DarkGreen;
                        }
                        break;
                    case ConsoleKey.D4:
                        {
                            brushColor.BackColor = ConsoleColor.Blue;
                            cursorColor.BackColor = ConsoleColor.DarkBlue;
                            oldCurorColors.BackColor = ConsoleColor.DarkBlue;
                        }
                        break;
                    case ConsoleKey.D5:
                        {
                            brushColor.BackColor = ConsoleColor.Magenta;
                            cursorColor.BackColor = ConsoleColor.DarkMagenta;
                            oldCurorColors.BackColor = ConsoleColor.DarkMagenta;
                        }
                        break;
                    case ConsoleKey.Enter:
                        {
                            string output = JsonConvert.SerializeObject(paintPixels, Formatting.Indented);
                            FileInfo fi = new FileInfo(Environment.CurrentDirectory + "/drawing.json");

                            if (fi.Exists) fi.Delete();

                            using (StreamWriter sw = fi.CreateText())
                            {
                                sw.Write(output);
                            }

                            justSaved = true;
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        {
                            string input = "";
                            FileInfo fi = new FileInfo(Environment.CurrentDirectory + "/drawing.json");

                            if (!fi.Exists) return;

                            using (StreamReader sw = fi.OpenText())
                            {
                                input = sw.ReadToEnd();
                                
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

        public override void OnUpdate()
        {

        }

        public override void OnDraw()
        {
            Graphic.Clear();
            Graphic.FitSizesToWindow();

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
            else
            {
                Graphic.ResetDefaultColors();
            }

            var pixelCount = paintPixels.Count;
            for (int i = 0; i < pixelCount; i++)
            {
                if (i < paintPixels.Count)
                    Graphic.Add(paintPixels[i].Char.ToString(), paintPixels[i].Position, paintPixels[i].Color);
            }

            pixelCount = tempPixels.Count;
            for (int i = 0; i < pixelCount; i++)
            {
                if (i < tempPixels.Count)
                    Graphic.Add(tempPixels[i].Char.ToString(), tempPixels[i].Position, tempPixels[i].Color);
            }

            drawInterface();
            drawDebug();

            if (MousePosition.IsInRectangle(new Coords(3, 2), new Coords(Graphic.Width - 4, Graphic.Height - 3)))
            {
                Graphic.Add(" ", MousePosition, cursorColor);
            }

            Graphic.Draw();
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
            Graphic.Add(" Colors:   (1),   (2),   (3),   (4),   (5)   ", new Coords(1, 0));
            Graphic.Add(" ", new Coords(10, 0), new Colors(Colors.Color.Default, Colors.Color.Red));
            Graphic.Add(" ", new Coords(17, 0), new Colors(Colors.Color.Default, Colors.Color.Yellow));
            Graphic.Add(" ", new Coords(24, 0), new Colors(Colors.Color.Default, Colors.Color.Green));
            Graphic.Add(" ", new Coords(31, 0), new Colors(Colors.Color.Default, Colors.Color.Blue));
            Graphic.Add(" ", new Coords(38, 0), new Colors(Colors.Color.Default, Colors.Color.Magenta));

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Right, Origin.VerticalOrigin.Bottom));
            Graphic.Add(" Draw: LMB, Erase: RMB, Clear: MMB ", new Coords(-1, 0));

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
        }
    }
}
