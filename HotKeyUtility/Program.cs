using log4net;
using System;
using System.Windows.Forms;

namespace HotkeyUtility
{
    static class Program
    {
        public static ILog LoggerObj;
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HiddenForm());
        }
    }
}