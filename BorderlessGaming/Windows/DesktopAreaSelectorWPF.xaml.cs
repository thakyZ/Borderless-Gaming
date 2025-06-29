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
    public partial class DesktopAreaSelectorWPF : Window, IDisposable
    {
        internal DesktopAreaSelectorViewModel ViewModel {
            get => ((this.DataContext ?? new DesktopAreaSelectorViewModel(this)) as DesktopAreaSelectorViewModel)!;
            set => this.DataContext = value;
        }

        internal Point? WindowSize { get; set; }
        internal bool Unknown { get; set; }
        
        public DesktopAreaSelectorWPF() : this(false, null) { }
        public DesktopAreaSelectorWPF(bool unknown, Point? windowSize)
        {
            this.InitializeComponent();
            this.Unknown = unknown;
            this.WindowSize = windowSize;
        }

        public new MessageBoxResult ShowDialog()
        {
            return MessageBoxResult.None;
        }

        public Int32Rect GetCurrentValue()
        {
            return new Int32Rect(this.ViewModel.PositionX, this.ViewModel.PositionY, this.ViewModel.SizeH, this.ViewModel.SizeH);
        }

        public void Dispose()
        {
            base.Close();
        }
    }
}
