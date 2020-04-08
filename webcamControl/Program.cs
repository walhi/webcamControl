using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Management; // need to add System.Management to your project references.
using System.Diagnostics;
using DirectShowLib;
using HidLibrary;

using System.Threading;
using System.Runtime.InteropServices;

namespace webcamControl
{
    static class Globals
    {
        public static USBControl _USBControl = new USBControl();
    }

    static class Program
    {
        // From https://www.cyberforum.ru/post6898675.html
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hwnd, WinStyle style);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(string lpClassName, string lpWindowName);

        enum WinStyle
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3
        }
        static Mutex mutex = new Mutex(false, "OnlyOneForm");

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!mutex.WaitOne(500, false))
            {
                IntPtr wndHandle = FindWindowByCaption(null, "WebcamControl");
                ShowWindow(wndHandle, WinStyle.ShowNormal);
                return;
            }
            Application.Run(new Form2());
        }

    }
}
