using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace hacker_buddy_console
{
    internal class Controller : IDisposable
    {
        private GlobalKeyboardHook _globalKeyboardHook;

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            //Debug.WriteLine(e.KeyboardData.VirtualCode);

            if (e.KeyboardData.VirtualCode != GlobalKeyboardHook.VkSnapshot)
                return;

            // seems, not needed in the life.
            //if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown &&
            //    e.KeyboardData.Flags == GlobalKeyboardHook.LlkhfAltdown)
            //{
            //    MessageBox.Show("Alt + Print Screen");
            //    e.Handled = true;
            //}
            //else

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                Console.WriteLine("Got you! " + e.KeyboardData.VirtualCode + " " + e.KeyboardData.TimeStamp);
                e.Handled = true;
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }


        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        public event EventHandler HackerSleepingEvent;
        public event EventHandler HackerAwakenEvent;

        public void StartLogging()
        {
            DateTime lastKeyCapture = DateTime.Now;
            while (true)
            {
                var elapsed = DateTime.Now.Subtract(lastKeyCapture);
                if (elapsed > TimeSpan.FromSeconds(5))
                {
                    HackerSleepingEvent?.Invoke(null, null);
                    lastKeyCapture = DateTime.Now;
                }
                else
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop--;
                    Console.WriteLine(elapsed);
                }

                Thread.Sleep(100);

                for (int i = 0; i < 255; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    // 32769 should be used for windows 10.
                    if (keyState == 1 || keyState == -32767 || keyState == 32769)
                    {
                        lastKeyCapture = DateTime.Now;
                        HackerAwakenEvent?.Invoke(null, null);
                        Console.WriteLine((char)i);
                        break;
                    }
                }
            }
        }
    }
}
