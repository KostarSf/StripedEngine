using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static ConsoleLib.NativeMethods;

namespace Striped.Engine
{
    public class GameCore
    {
        public delegate void OnKeyPressHandler(KeyEventInfo e);
        public delegate void OnMouseHandler(MouseEventInfo e);
        public static event OnKeyPressHandler KeyPressEvent;
        public static event OnMouseHandler MouseEvent;

        private enum CoreState
        {
            Starting,
            Running,
            Stopping
        }

        private Task onUpdateTick;
        private Task onUpdateFrame;
        private Task onKeyPress;

        private Task onNativeInput;

        private CoreState currentState { get; set; }

        private bool enableDefaultKeyListener = true;

        public int FrameRate { get; set; }
        public int TickRate { get; set; }
        public int CurrentFrameRate { get; private set; }
        public int CurrentTickRate { get; private set; }

        public string Title { get => Console.Title; set => Console.Title = value; }

        public bool EnableNativeInput { get; set; } = false;

        public Utils.Coords MousePosition { get; private set; } = new Utils.Coords(0, 0);

        public GameCore()
        {
            ConsoleLib.ConsoleListener.KeyEvent += ConsoleListener_KeyEvent;
            ConsoleLib.ConsoleListener.MouseEvent += ConsoleListener_MouseEvent;

            KeyPressEvent += OnKeyPress;
            MouseEvent += OnMouseEvent;
        }

        private void ConsoleListener_MouseEvent(MOUSE_EVENT_RECORD r)
        {
            MousePosition.X = r.dwMousePosition.X;
            MousePosition.Y = r.dwMousePosition.Y;

            var mouseEvent = new MouseEventInfo
            {
                Position = new Utils.Coords(r.dwMousePosition.X, r.dwMousePosition.Y),
                ButtonPressed = MouseEventInfo.GetPressedButton(r.dwButtonState),
                EventFlag = MouseEventInfo.GetEventFlag(r.dwEventFlags)
            };

            MouseEvent(mouseEvent);
        }

        private void ConsoleListener_KeyEvent(KEY_EVENT_RECORD r)
        {
            var keyEvent = new KeyEventInfo
            {
                Pressed = r.bKeyDown,
                Key = KeyEventInfo.GetConsoleKey(r.wVirtualKeyCode),
                KeyChar = r.UnicodeChar,
                Modifiers = KeyEventInfo.GetModifiers(r.dwControlKeyState)
            };

            KeyPressEvent(keyEvent);
        }

        public virtual void OnUpdate() { }

        public virtual void OnDraw() { }

        public virtual void OnKeyPress(KeyEventInfo e) { }

        public virtual void OnMouseEvent(MouseEventInfo e) { }

        private void TickCycle()
        {
            var timer = new Stopwatch();

            while (currentState != CoreState.Stopping)
            {
                timer.Restart();

                OnUpdate();

                if (timer.ElapsedMilliseconds < 1000 / TickRate)
                {
                    Thread.Sleep(1000 / TickRate - (int)timer.ElapsedMilliseconds);
                }

                timer.Stop();
                CurrentTickRate = 1000 / (int)timer.ElapsedMilliseconds;
            }
        }

        private void FrameCycle()
        {
            var timer = new Stopwatch();

            while (currentState != CoreState.Stopping)
            {
                timer.Restart();

                Console.CursorVisible = false;
                OnDraw();

                if (timer.ElapsedMilliseconds < 1000 / FrameRate)
                {
                    Thread.Sleep(1000 / FrameRate - (int)timer.ElapsedMilliseconds);
                }

                timer.Stop();
                CurrentFrameRate = 1000 / (int)timer.ElapsedMilliseconds;
            }
        }

        private void InputKeyCycle()
        {
            ConsoleKeyInfo key;

            while (currentState != CoreState.Stopping || enableDefaultKeyListener)
            {
                key = Console.ReadKey(true);
                KeyPressEvent(new KeyEventInfo
                {
                    Pressed = true,
                    Key = key.Key,
                    KeyChar = key.KeyChar,
                    Modifiers = KeyEventInfo.GetModifiers(key.Modifiers),
                });
            }
        }

        public void Start()
        {
            currentState = CoreState.Running;

            onUpdateTick = new Task(TickCycle);
            onUpdateFrame = new Task(FrameCycle);
            onKeyPress = new Task(InputKeyCycle);

            onNativeInput = new Task(enableNativeInput);

            onUpdateTick.Start();
            onUpdateFrame.Start();

            if (EnableNativeInput &&
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                onNativeInput.Start();
            }
            else
            {
                onKeyPress.Start();
            }

            onUpdateTick.Wait();
            onUpdateFrame.Wait();
        }

        private void enableNativeInput()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            uint mode = 0;
            GetConsoleMode(inHandle, ref mode);

            mode &= ~ENABLE_QUICK_EDIT_MODE;
            mode |= ENABLE_WINDOW_INPUT;
            mode |= ENABLE_MOUSE_INPUT;

            SetConsoleMode(inHandle, mode);

            ConsoleLib.ConsoleListener.Start();
        }

        public void Stop()
        {
            currentState = CoreState.Stopping;
            ConsoleLib.ConsoleListener.Stop();

            Thread.Sleep(50);

            Console.Clear();
            Console.WriteLine("Программа завершила работу.");
            Console.WriteLine("Нажмите любую клавишу для выхода.. ");
            Console.ReadKey(true);
        }
    }
}
