#nullable enable
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using BorderlessGaming.Forms;
using BorderlessGaming.Logic.Misc;
using BorderlessGaming.Logic.Models;
using BorderlessGaming.Logic.Windows;
using BorderlessGaming.Windows;

namespace BorderlessGaming
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool useWPF = true;

            if (useWPF)
            {
                MainWPF();
            }
            else
            {
                var task = Task.Run(async () => await MainFormAsync());
                task.ConfigureAwait(true);
                task.Wait();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void MainWPF()
        {
            RunApplicationWPF(preStartup: async () =>
            {
                Tools.Setup();

                //use github updating for non-steam
                if (SettingsWrapper.Instance.StartupOptions.IsSteam is false && SettingsWrapper.Instance.Settings.CheckForUpdates is true)
                {
                    await Tools.CheckForUpdates();
                }
            });
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static async Task MainFormAsync()
        {
            Tools.Setup();

            //use github updating for non-steam
            if (SettingsWrapper.Instance.StartupOptions.IsSteam is false && SettingsWrapper.Instance.Settings.CheckForUpdates is true)
            {
                await Tools.CheckForUpdates();
            }

            RunApplicationForms();
        }

        private static void RunApplicationWPF(Action preStartup)
        {
            new AppWPF(preStartup).Run(new MainWindowWPF());
        }

        private static void RunApplicationForms()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            ForegroundManager.Subscribe();
            Application.Run(new MainWindow());

        }
    }
}
