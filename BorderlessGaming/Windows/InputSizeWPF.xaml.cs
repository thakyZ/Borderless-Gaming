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
    /// Interaction logic for InputSizeWPF.xaml
    /// </summary>
    public partial class InputSizeWPF : Window
    {
        private InputSizeViewModel ViewModel => (this.DataContext as InputSizeViewModel)!;
        public InputSizeWPF()
        {
            InitializeComponent();
            this.DataContext = new InputSizeViewModel(this);
        }

        private void PositionXSpinner_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<object> @event)
        {
            if (@event.NewValue is int @int) {
                ViewModel.PositionX = @int;
            }
        }

        private void PositionYSpinner_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<object> @event)
        {
            if (@event.NewValue is int @int) {
                ViewModel.PositionY = @int;
            }
        }

        private void SizeWSpinner_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<object> @event)
        {
            if (@event.NewValue is int @int) {
                ViewModel.SizeW = @int;
            }
        }

        private void SizeHSpinner_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<object> @event)
        {
            if (@event.NewValue is int @int) {
                ViewModel.SizeH = @int;
            }
        }
    }
}
