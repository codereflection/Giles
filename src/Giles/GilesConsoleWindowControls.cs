using System;
using System.Runtime.InteropServices;

namespace Giles
{
    public class GilesConsoleWindowControls
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        static extern IntPtr GetConsoleWindow();

        static readonly IntPtr GilesConsole = GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        const int SWP_NOSIZE = 0x0001;
        public static void SetConsoleWindowPosition(int x, int y)
        {
            SetWindowPos(GilesConsole, x, y, 0, 0, 0, SWP_NOSIZE);
        }
    }
}