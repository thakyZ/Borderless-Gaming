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
    /// Interaction logic for DesktopAreaSelectorWPF.xaml
    /// </summary>
    public partial class DesktopAreaSelectorWPF : Window
    {
        private DesktopAreaSelectorViewModel ViewModel => (this.DataContext as DesktopAreaSelectorViewModel)!;
        public DesktopAreaSelectorWPF()
        {
            InitializeComponent();
            this.DataContext = new DesktopAreaSelectorViewModel(this);
        }
    }
}
