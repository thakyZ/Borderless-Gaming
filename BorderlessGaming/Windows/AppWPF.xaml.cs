#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BorderlessGaming.Windows
{
    /// <summary>
    /// Interaction logic for AppWPF.xaml
    /// </summary>
    public partial class AppWPF : Application
    {
        private MainWindowWPF MainWindowWPF => (this.MainWindow as MainWindowWPF)!;
        public AppWPF(Action preStartup)
        {
            preStartup.Invoke();
        }

        private void OnExit(object? sender, System.Windows.ExitEventArgs @event)
        {
        }

        private void OnStartup(object? sender, System.Windows.StartupEventArgs @event)
        {
            this.MainWindow = new MainWindowWPF();
            this.MainWindow.Show();
        }
    }
}
