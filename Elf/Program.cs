using System;
using System.Windows.Forms;

using ElfCore.Controllers;
using ElfCore.Forms;
using ElfCore.Util;

namespace Elf {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LaunchEditor(args);
        }


        private static void LaunchEditor(string[] args) {
            Settings settings = Settings.Instance;
            settings.Style = Settings.SettingsStyle.Xml;

            Workshop workshop = Workshop.Instance;
            workshop.RunMode = RunMode.Standalone;
            workshop.Initialize();

            Application.Run(new Editor(args));

            settings.Save();
            workshop.Dispose();
        }
    }
}