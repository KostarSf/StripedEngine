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

    internal class MyGameCore : GameCore
    {
        public delegate void OnDrawHandler();
        public event OnDrawHandler DrawEvent;

        int mouseCount, keyCount;
        MouseEventInfo mouseEvent = new MouseEventInfo();
        KeyEventInfo keyEvent = new KeyEventInfo();

        Colors cursorColor = new Colors(Colors.Color.Default, Colors.Color.DarkGreen);
        Colors brushColor = new Colors(Colors.Color.Default, Colors.Color.Green);

        Colors oldCurorColors = new Colors(Colors.Color.Default, Colors.Color.DarkGreen);

        Window paintWindow = new Window 
        { 
            Position = new Coords(15, 5), 
            Width = 20, 
            Height = 10,
            BorderColor = Colors.Default,
            Border = WindowBorder.Default,
            Control = WindowControl.None,
            CanResize = true,
        };

        List<PaintPixel> paintPixels = new List<PaintPixel>();
        List<PaintPixel> tempPixels = new List<PaintPixel>();

        bool showDebug = true;

        bool justSaved;
        bool justLoaded;
        bool clearTempPaint;

        bool mousePressed;

        public MyGameCore()
        {
            Title = "KostarSf's Terminal Paint";

            TickRate = 60;
            FrameRate = 60;

            EnableNativeInput = true;

            Graphic.EnableFastDraw = true;
            Graphic.ShowLineUpdates = true;

            AddObject(paintWindow);

            this.Start();
        }

        public override void OnMouseEvent(MouseEventInfo e)
        {
            mouseCount++;
            mouseEvent = e;

            switch(e.EventFlag)
            {
                case Striped.Engine.MouseEvent.Pressed:
                    {
                        mousePressed = true;
                    }
                    break;
                case Striped.Engine.MouseEvent.None:
                    {
                        mousePressed = false;
                    }
                    break;
            }

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

                        //for (int i = paintPixels.Count - 1; i >= 0; i--)
                        //{
                        //    if (paintPixels[i].Position.SameAs(e.Position))
                        //    {
                        //        paintPixels.RemoveAt(i);
                        //    }
                        //}

                        for (int i = tempPixels.Count - 1; i >= 0; i--)
                        {
                            if (tempPixels[i].Position.SameAs(e.Position))
                            {
                                tempPixels.RemoveAt(i);
                            }
                        }


                        if (tempPixels.Count == 0)
                        {
                            //paintPixels.Add(new PaintPixel { Char = ' ', Color = new Colors(brushColor), Position = e.Position });
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

                                foreach(var coords in interpolatePath)
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
            keyCount++;
            keyEvent = e;

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
                    Graphic.DefaultBackColor = ConsoleColor.Green;
                    justSaved = false;
                }

                if (justLoaded)
                {
                    Graphic.DefaultBackColor = ConsoleColor.Gray;
                    justLoaded = false;
                }
            }
            else
            {
                Graphic.ResetDefaultColors();
            }

            /*!! Раскомментить если не выйдет */

            //var pixelCount = paintPixels.Count;
            //for (int i = 0; i < pixelCount; i++)
            //{
            //    if (i < paintPixels.Count)
            //        Graphic.Add(paintPixels[i].Char.ToString(), paintPixels[i].Position, paintPixels[i].Color);
            //}

            //pixelCount = tempPixels.Count;
            //for (int i = 0; i < pixelCount; i++)
            //{
            //    if (i < tempPixels.Count)
            //        Graphic.Add(tempPixels[i].Char.ToString(), tempPixels[i].Position, tempPixels[i].Color);
            //}

            showInterface();
            //showHelp();
            //showInputDebug();

            //Graphic.Add(" ", MousePosition, cursorColor);

            Graphic.Draw();
        }

        private void showInterface()
        {
            Graphic.AddRectangle(" ", new Colors(Colors.Color.Default, Colors.Color.Gray), Coords.Zero, new Coords(Graphic.Width - 1, 0));
            Graphic.Add(" Файл ", new Coords(1, 0), new Colors(Colors.Color.Black, Colors.Color.Gray));
            Graphic.Add(" Правка ", new Coords(8, 0), new Colors(Colors.Color.Black, Colors.Color.Gray));
            Graphic.Add(" Помощь ", new Coords(17, 0), new Colors(Colors.Color.Black, Colors.Color.Gray));

            if (MousePosition.IsInRectangle(new Coords(1, 0), new Coords(6, 0)))
            {
                if (mousePressed)
                {
                    Graphic.Add(" Файл ", new Coords(1, 0), new Colors(Colors.Color.Black, Colors.Color.DarkGray));
                }
                else
                {
                    Graphic.Add(" Файл ", new Coords(1, 0), new Colors(Colors.Color.White, Colors.Color.DarkGray));
                }
            }

            if (MousePosition.IsInRectangle(new Coords(8, 0), new Coords(15, 0)))
            {
                if (mousePressed)
                {
                    Graphic.Add(" Правка ", new Coords(8, 0), new Colors(Colors.Color.Black, Colors.Color.DarkGray));
                }
                else
                {
                    Graphic.Add(" Правка ", new Coords(8, 0), new Colors(Colors.Color.White, Colors.Color.DarkGray));
                }
            }

            if (MousePosition.IsInRectangle(new Coords(17, 0), new Coords(24, 0)))
            {
                if (mousePressed)
                {
                    Graphic.Add(" Помощь ", new Coords(17, 0), new Colors(Colors.Color.Black, Colors.Color.DarkGray));
                }
                else
                {
                    Graphic.Add(" Помощь ", new Coords(17, 0), new Colors(Colors.Color.White, Colors.Color.DarkGray));
                }
            }

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Right, Origin.VerticalOrigin.Top));
            Graphic.Add($" {this.MousePosition.X} {this.MousePosition.Y} ", Coords.Zero, new Colors(Colors.Color.Black, Colors.Color.Gray));

            Graphic.SetOrigin(Origin.Default);

            DrawEvent();
            //Graphic.AddRectangleBorder("═", "│", "─", "│", "╒", "╕", "┘", "└", new Colors(Colors.Color.Default, Colors.Color.Default), new Coords(15, 5), new Coords(70, 15));
        }

        void showHelp()
        {
            Graphic.Add(" Дебаг: Q ", Coords.Zero, new Colors(Colors.Color.DarkGray, Colors.Color.Default));

            //var verPos = Graphic.Height - 1;
            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Left, Origin.VerticalOrigin.Bottom));
            Graphic.Add(" Цвета:   (1),   (2),   (3),   (4),   (5)   ", new Coords(0, -1));
            Graphic.Add(" ", new Coords(8, -1), new Colors(Colors.Color.Default, Colors.Color.Red));
            Graphic.Add(" ", new Coords(15, -1), new Colors(Colors.Color.Default, Colors.Color.Yellow));
            Graphic.Add(" ", new Coords(22, -1), new Colors(Colors.Color.Default, Colors.Color.Green));
            Graphic.Add(" ", new Coords(29, -1), new Colors(Colors.Color.Default, Colors.Color.Blue));
            Graphic.Add(" ", new Coords(36, -1), new Colors(Colors.Color.Default, Colors.Color.Magenta));
            Graphic.Add(" Рисовать: ЛКМ, Стереть: ПКМ, Очистить: СКМ ", new Coords(0, 0));

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Right, Origin.VerticalOrigin.Bottom));
            Graphic.Add(" Сохранить: ENT, Загрузить: SPC ", new Coords(0, 0));

            Graphic.SetOrigin(new Origin(Origin.HorizontalOrigin.Right, Origin.VerticalOrigin.Top));
            Graphic.Add($"{this.MousePosition.X} {this.MousePosition.Y}", Coords.Zero);

            Graphic.SetOrigin(Origin.Default);
        }

        void showInputDebug()
        {
            if (!showDebug) return;

            Graphic.Add($"Tick rate: .....: {CurrentTickRate}/{TickRate}", new Coords(0, 0));
            Graphic.Add($"Frame rate: ....: {CurrentFrameRate}/{FrameRate}", new Coords(0, 1));
            Graphic.Add($"Draw calls: ....: {Graphic.LastDrawCallsCount}", new Coords(0, 2));
            Graphic.Add($"Pixels painted .: {paintPixels.Count}", new Coords(0, 3));
            Graphic.Add($"Pixels in temp .: {tempPixels.Count}", new Coords(0, 4));

            //Graphic.Add($"- Mouse Events -", new Coords(0, 5));
            //Graphic.Add($"Events count ...: {mouseCount}", new Coords(0, 6));
            //Graphic.Add($"Button pressed .: {mouseEvent.ButtonPressed}", new Coords(0, 7));
            //Graphic.Add($"Click flags ....: {mouseEvent.EventFlag}", new Coords(0, 8));
            //Graphic.Add($"Position .......: {mouseEvent.Position.X} {mouseEvent.Position.Y}", new Coords(0, 9));

            //Graphic.Add($"- Key Events -", new Coords(0, 11));
            //Graphic.Add($"Events count ..: {keyCount}", new Coords(0, 12));
            //Graphic.Add($"Key name ......: {keyEvent.Key}", new Coords(0, 13));
            //Graphic.Add($"Key modifiers .: {keyEvent.Modifiers}", new Coords(0, 14));
            //Graphic.Add($"Key pressed ...: {keyEvent.Pressed}", new Coords(0, 15));
            //Graphic.Add($"Key char ......: {keyEvent.KeyChar}", new Coords(0, 16));
        }
    }
}