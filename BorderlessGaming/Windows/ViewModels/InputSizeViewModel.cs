#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BorderlessGaming.Windows.ViewModels
{
    internal class InputSizeViewModel : INotifyPropertyChanged
    {
        private InputSizeWPF? Parent { get; }
        public InputSizeViewModel(InputSizeWPF parent)
        {
            this.Parent = parent;
        }

#pragma warning disable CS8618
        public InputSizeViewModel() { }
#pragma warning restore CS8618

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged<T>(ref T property, T value, [CallerMemberName] string callerName = "")
        {
            if (property is not null || value is not null || property?.Equals(value) == true)
            {
                property = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));
            }
        }

        #endregion INotifyPropertyChanged Implementation

        #region Bindings

        #region Localization Bindings
        
        private string? title;
        public string? Title { // "Input Position & Size";
            get => title;
            set => NotifyPropertyChanged<string?>(ref title, value);
        }
        private string? positionXLabel;
        public string? PositionXLabel { // "Position X";
            get => positionXLabel;
            set => NotifyPropertyChanged<string?>(ref positionXLabel, value);
        }
        private string? positionYLabel;
        public string? PositionYLabel { // "Position Y";
            get => positionYLabel;
            set => NotifyPropertyChanged<string?>(ref positionYLabel, value);
        }
        private string? sizeWLabel;
        public string? SizeWLabel { // "Size W";
            get => sizeWLabel;
            set => NotifyPropertyChanged<string?>(ref sizeWLabel, value);
        }
        private string? sizeHLabel;
        public string? SizeHLabel { // "Size H";
            get => sizeHLabel;
            set => NotifyPropertyChanged<string?>(ref sizeHLabel, value);
        }
        private string? oKButtonLabel;
        public string? OKButtonLabel { // "OK";
            get => oKButtonLabel;
            set => NotifyPropertyChanged<string?>(ref oKButtonLabel, value);
        }
        private string? cancelButtonLabel;
        public string? CancelButtonLabel { // "Cancel";
            get => cancelButtonLabel;
            set => NotifyPropertyChanged<string?>(ref cancelButtonLabel, value);
        }

        #endregion Localization Bindings

        #region Value Bindings

        private int positionX = 0;
        public int PositionX
        {
            get => positionX;
            set => NotifyPropertyChanged<int>(ref positionX, value);
        }

        private int positionY = 0;
        public int PositionY
        {
            get => positionY;
            set => NotifyPropertyChanged<int>(ref positionY, value);
        }

        private int sizeW = 0;
        public int SizeW
        {
            get => sizeW;
            set => NotifyPropertyChanged<int>(ref sizeW, value);
        }

        private int sizeH = 0;
        public int SizeH
        {
            get => sizeH;
            set => NotifyPropertyChanged<int>(ref sizeH, value);
        }

        #endregion Value Bindings

        #region Commands
        public ICommand CancelButtonClicked => new CommandImpl(OnCancelButtonClicked);
        public ICommand OKButtonClicked => new CommandImpl(OnOKButtonClicked);

        #endregion

        #endregion Bindings

        #region Methods

        #region Event Methods

        private void OnCancelButtonClicked()
        {
        }

        private void OnOKButtonClicked()
        {
        }

        #endregion Event Methods

        #endregion Methods
    }
}
