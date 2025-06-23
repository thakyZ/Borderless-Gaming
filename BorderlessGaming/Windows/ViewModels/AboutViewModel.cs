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
    internal class AboutViewModel : INotifyPropertyChanged
    {
        private AboutWPF Parent { get; }
        public AboutViewModel(AboutWPF parent)
        {
            this.Parent = parent;
        }

#pragma warning disable CS8618
        public AboutViewModel() { }
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

        public string Title => "Input Position & Size";
        public string PositionXLabel => "Position X";
        public string PositionYLabel => "Position Y";
        public string SizeWLabel => "Size W";
        public string SizeHLabel => "Size H";
        public string OKButtonLabel => "OK";
        public string CancelButtonLabel => "Cancel";

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

        public ICommand PositionXSpinnerValueChanged => new CommandImpl<int>(OnPositionXSpinnerValueChanged);
        public ICommand PositionYSpinnerValueChanged => new CommandImpl<int>(OnPositionYSpinnerValueChanged);
        public ICommand SizeWSpinnerValueChanged => new CommandImpl<int>(OnSizeWSpinnerValueChanged);
        public ICommand SizeHSpinnerValueChanged => new CommandImpl<int>(OnSizeHSpinnerValueChanged);
        public ICommand CancelButtonClicked => new CommandImpl(OnCancelButtonClicked);
        public ICommand OKButtonClicked => new CommandImpl(OnOKButtonClicked);

        #endregion

        #endregion Bindings

        #region Methods

        #region Event Methods

        private void OnPositionXSpinnerValueChanged(int newValue)
        {

        }

        private void OnPositionYSpinnerValueChanged(int newValue)
        {

        }

        private void OnSizeWSpinnerValueChanged(int newValue)
        {

        }

        private void OnSizeHSpinnerValueChanged(int newValue)
        {

        }

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
