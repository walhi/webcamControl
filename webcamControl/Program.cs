using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Management; // need to add System.Management to your project references.

namespace webcamControl
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }
}
