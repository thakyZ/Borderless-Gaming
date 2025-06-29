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
    public partial class InputTextWPF : Window, IDisposable
    {
        internal InputTextViewModel ViewModel {
            get => ((this.DataContext ?? new InputTextViewModel(this)) as InputTextViewModel)!;
            set => this.DataContext = value;
        }

        public InputTextWPF()
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

        public void SetInstructions(string? instructions)
        {
            this.ViewModel.Instructions = instructions;
        }

        public void SetInput(string? input)
        {
            this.ViewModel.Input = input;
        }

        public string? GetCurrentValue()
        {
            return this.ViewModel.Input;
        }

        public void Dispose()
        {
            base.Close();
        }
    }
}
