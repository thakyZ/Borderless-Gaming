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
    public partial class InputSizeWPF : Window, IDisposable
    {
        internal InputSizeViewModel ViewModel {
            get => ((this.DataContext ?? new InputSizeViewModel(this)) as InputSizeViewModel)!;
            set => this.DataContext = value;
        }

        public InputSizeWPF(bool keepRatio, Point? windowSize)
        {
            InitializeComponent();
        }

        public new MessageBoxResult ShowDialog()
        {
            return MessageBoxResult.None;
        }

        public void SetTitle(string? title)
        {
            this.ViewModel.Title = title;
        }

        public void SetInstructionsX(string instructionsX)
        {
            this.ViewModel.PositionXLabel = instructionsX;
        }

        public void SetInstructionsY(string instructionsY)
        {
            this.ViewModel.PositionYLabel = instructionsY;
        }

        public void SetInstructionsW(string instructionsW)
        {
            this.ViewModel.SizeWLabel = instructionsW;
        }

        public void SetInstructionsH(string instructionsH)
        {
            this.ViewModel.SizeHLabel = instructionsH;
        }

        public void SetInputX(int positionX)
        {
            this.ViewModel.PositionX = positionX;
        }

        public void SetInputY(int positionY)
        {
            this.ViewModel.PositionY = positionY;
        }

        public void SetInputW(int sizeW)
        {
            this.ViewModel.SizeW = sizeW;
        }

        public void SetInputH(int sizeH)
        {
            this.ViewModel.SizeH = sizeH;
        }

        public (int X, int Y, int Width, int Height) GetCurrentValue()
        {
            return (this.ViewModel.PositionX, this.ViewModel.PositionY, this.ViewModel.SizeH, this.ViewModel.SizeH);
        }

        public void Dispose()
        {
            base.Close();
        }

        private void PositionXSpinner_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void SizeWSpinner_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void PositionYSpinner_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void SizeHSpinner_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}
