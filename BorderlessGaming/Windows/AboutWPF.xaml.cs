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
using System.Windows.Shapes;

using BorderlessGaming.Windows.ViewModels;

namespace BorderlessGaming.Windows
{
    /// <summary>
    /// Interaction logic for AboutWPF.xaml
    /// </summary>
    public partial class AboutWPF : Window
    {
        private AboutViewModel ViewModel => (this.DataContext as AboutViewModel)!;
        public AboutWPF()
        {
            InitializeComponent();
            this.DataContext = new AboutViewModel(this);
        }
    }
}
