using Sovoma;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zusisuplib;

namespace ZusiMeter
{
    class Startup
    {
        private static readonly string _appGuid = "5DED5276-60FF-419F-B64D-864637E44C6C";

        //---------------------------------------------------------------------
        [STAThread]
        static void Main()
        {
            string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;

            if (executablePath != null)
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(executablePath));

            }

            App app = new();
            app.InitializeComponent();
            _ = app.Run();
        }
    }
}
