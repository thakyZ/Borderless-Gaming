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
    /// Interaction logic for InputTextWPF.xaml
    /// </summary>
    public partial class InputTextWPF : Window
    {
        private InputTextViewModel ViewModel => (this.DataContext as InputTextViewModel)!;
        public InputTextWPF()
        {
            InitializeComponent();
            this.DataContext = new InputTextViewModel(this);
        }
    }
}
