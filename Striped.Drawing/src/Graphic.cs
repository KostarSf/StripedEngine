using Striped.Utils;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Striped.Drawing
{
    /// <summary>
    /// Позволяет интерактивно отрисовывать цветные данные в терминале.
    /// </summary>
    public static class Graphic
    {
        private static ConsoleColor defaultFontColor;
        private static ConsoleColor defaultBackColor;

        private static ConsoleColor defaultFontColorBackup;
        private static ConsoleColor defaultBackColorBackup;

        private static Frame frame;


        private static Origin currentOrigin;

        /// <summary>
        /// Текущая ширина кадра
        /// </summary>
        public static int Width { get; private set; }
        /// <summary>
        /// Текущая высота кадра
        /// </summary>
        public static int Height { get; private set; }

        /// <summary>
        /// Включение режима оптимизации вывода
        /// </summary>
        public static bool EnableFastDraw { get; set; }

        /// <summary>
        /// Показывает обновления строк при включенном режиме оптимизации. (работает только при отключении данного режима)
        /// </summary>
        public static bool ShowLineUpdates { get; set; }

        /// <summary>
        /// Количество вызовов отрисовки, совершённых в предыдущем фрейме
        /// </summary>
        public static int LastDrawCallsCount { get; internal set; }

        /// <summary>
        /// Стандартный цвет текста вашего терминала. Объявляется при вызове конструктора класса.
        /// </summary>
        public static ConsoleColor DefaultFontColor { 
            get => defaultFontColor; 
            set 
            {
                defaultFontColor = value;
                Colors.Default.FontColor = value;
                Clear();
            }  
        }

        /// <summary>
        /// Стандартный цвет фона вашего терминала. Объявляется при вызове конструктора класса.
        /// </summary>
        public static ConsoleColor DefaultBackColor 
        { 
            get => defaultBackColor; 
            set 
            {
                defaultBackColor = value;
                Colors.Default.BackColor = value;
                Clear();
            }
        }

        static Graphic()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.BufferHeight = Console.WindowHeight;
                Console.WindowWidth = Console.BufferWidth;
            }

            Console.Clear();

            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            DefaultFontColor = Console.ForegroundColor;
            defaultFontColorBackup = DefaultFontColor;
            DefaultBackColor = Console.BackgroundColor;
            defaultBackColorBackup = DefaultBackColor;

            currentOrigin = Origin.Default;

            initFrame();
        }

        private static void initFrame()
        {
            if (frame != null)
            {
                frame.SetSizes(Width, Height);
            }
            else
            {
                frame = new Frame(Width, Height);
            }
        }

        /// <summary>
        /// Установка размеров рисуемого кадра
        /// </summary>
        /// <param name="width">Ширина в символах</param>
        /// <param name="height">Высота в символах</param>
        public static void SetSizes(int width, int height)
        {
            Width = width;
            Height = height;

            frame.SetSizes(Width, Height);
        }

        /// <summary>
        /// Установка размеров рисуемого кадра равными размерам окна терминала.
        /// </summary>
        public static void FitSizesToWindow()
        {
            SetSizes(Console.WindowWidth, Console.WindowHeight);
        }

        /// <summary>
        /// Задаёт начало координат для последующих добавляемых в кадр объектов
        /// </summary>
        /// <param name="topLeft"></param>
        public static void SetOrigin(Origin newOrigin)
        {
            currentOrigin = newOrigin;
        }

        /// <summary>
        /// Добавление текстовой строки в определённые координаты с задаваемыми цветами текста и фона.
        /// </summary>
        /// <param name="line">Текстовая строка</param>
        /// <param name="coords">Координаты строки</param>
        /// <param name="colors">Цвета строки</param>
        public static void Add(string line, Coords coords, Colors colors)
        {
            if (frame == null) return;
            
            frame.Add(line, coords, colors, currentOrigin);
        }

        /// <summary>
        /// Добавление текстовой строки в определённые координаты со стандартными цветами текста и фона.
        /// </summary>
        /// <param name="line">Текстовая строка</param>
        /// <param name="coords">Координаты строки</param>
        public static void Add(string line, Coords coords)
        {
            if (frame == null) return;

            Add(line, coords, new Colors(Colors.Color.Default, Colors.Color.Default));
        }

        /// <summary>
        /// Добавление линии
        /// </summary>
        /// <param name="fill">Символ заполнения</param>
        /// <param name="colors">Цвета линии</param>
        /// <param name="startPoint">Координата начальной точки</param>
        /// <param name="endPoint">Координата конечной точки</param>
        public static void AddLine(string fill, Colors colors, Coords startPoint, Coords endPoint)
        {
            if (frame == null) return;

            frame.AddLine(startPoint, endPoint, fill, colors, currentOrigin);
        }

        /// <summary>
        /// Добавление произвольного контура
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="colors">Цвета контура</param>
        /// <param name="startPoint">Координаты начальной точки</param>
        /// <param name="extPoints">Координаты дополнительных точек</param>
        public static void AddShape(string fill, Colors colors, Coords startPoint, params Coords[] extPoints)
        {
            if (frame == null) return;

            if (extPoints.Length == 0)
            {
                Add(fill.ToString(), startPoint, colors);
                return;
            }

            var lastPoint = startPoint;

            foreach (var point in extPoints)
            {
                frame.AddLine(lastPoint, point, fill, colors, currentOrigin);
                lastPoint = point;
            }
        }

        /// <summary>
        /// Создание окружности
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="colors"></param>
        /// <param name="coords"></param>
        /// <param name="radius"></param>
        public static void AddCircle(string fill, Colors colors, Coords coords, int radius)
        {
            if (frame == null) return;

            int x = radius;
            int y = 0;
            int radiusError = 1 - x;
            while (x >= y)
            {
                Add(fill, new Coords(x + coords.X, y + coords.Y), colors);
                Add(fill, new Coords(y + coords.X, x + coords.Y), colors);
                Add(fill, new Coords(-x + coords.X, y + coords.Y), colors);
                Add(fill, new Coords(-y + coords.X, x + coords.Y), colors);
                Add(fill, new Coords(-x + coords.X, -y + coords.Y), colors);
                Add(fill, new Coords(-y + coords.X, -x + coords.Y), colors);
                Add(fill, new Coords(x + coords.X, -y + coords.Y), colors);
                Add(fill, new Coords(y + coords.X, -x + coords.Y), colors);
                y++;

                if (radiusError < 0)
                {
                    radiusError += 2 * y + 1;
                }
                else
                {
                    x--;
                    radiusError += 2 * (y - x + 1);
                }
            }
        }

        public static void AddRectangle(string fill, Colors colors, Coords pos1, Coords pos2)
        {
            string genLine(string fill, int length)
            {
                var output = new StringBuilder();

                var fillIndex = 0;

                for (int i = 0; i <= length; i++)
                {
                    if (fillIndex == fill.Length)
                    {
                        fillIndex = 0;
                    }

                    output.Append(fill[fillIndex]);
                    fillIndex++;
                }

                return output.ToString();
            }

            void swap<T>(ref T a, ref T b)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }

            if (pos1.X > pos2.X)
            {
                swap(ref pos1, ref pos2);
            }

            var x1 = pos1.X;
            var x2 = pos2.X;

            var y1 = pos1.Y;
            var y2 = pos2.Y;

            if (y1 > y2)
            {
                swap(ref y1, ref y2);
            }

            for (int i = y1; i <= y2; i++)
            {
                Add(genLine(fill, x2 - x1), new Coords(x1, i), colors);
            }
        }

        public static void AddRectangleBorder(string t, string r, string b, string l, string lt, string rt, string rb, string lb, Colors colors, Coords pos1, Coords pos2)
        {
            void swap<T>(ref T a, ref T b)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }

            if (pos1.X > pos2.X)
            {
                swap(ref pos1, ref pos2);
            }

            var x1 = pos1.X;
            var x2 = pos2.X;

            var y1 = pos1.Y;
            var y2 = pos2.Y;

            AddLine(t, colors, new Coords(x1, y1), new Coords(x2, y1));
            AddLine(r, colors, new Coords(x2, y1), new Coords(x2, y2));
            AddLine(b, colors, new Coords(x1, y2), new Coords(x2, y2));
            AddLine(l, colors, new Coords(x1, y1), new Coords(x1, y2));

            Add(lt, new Coords(x1, y1), colors);
            Add(rt, new Coords(x2, y1), colors);
            Add(rb, new Coords(x2, y2), colors);
            Add(lb, new Coords(x1, y2), colors);
        }

        /// <summary>
        /// Очистка кадра ото всех выводимых элементов.
        /// </summary>
        public static void Clear()
        {
            if (frame == null) return;

            frame.Clear();
        }

        /// <summary>
        /// Выводит кадр в окно терминала
        /// </summary>
        public static void Draw()
        {
            if (frame == null) return;

            frame.Draw();
            SetOrigin(Origin.Default);

            LastDrawCallsCount = frame.DrawCallsCount;
        }

        /// <summary>
        /// Заменяет стандартные цвета вывода. Исходные значения равны стандартным цветам вашего терминала.
        /// </summary>
        /// <param name="colors"></param>
        public static void SetDefaultColors(Colors colors)
        {
            DefaultFontColor = colors.FontColor;
            DefaultBackColor = colors.BackColor;
            Colors.Default = colors;
        }

        /// <summary>
        /// Сбрасывает стандартные цвета вывода до значений по-умолчанию вашего терминала, если они были перезаписаны функцией 
        /// <see cref="SetDefaultColors(Colors)"/>
        /// </summary>
        public static void ResetDefaultColors()
        {
            DefaultFontColor = defaultFontColorBackup;
            DefaultBackColor = defaultBackColorBackup;
            Colors.Default = new Colors(DefaultFontColor, DefaultBackColor);
        }

    }
}
