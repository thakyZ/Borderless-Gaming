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
    internal class InputTextViewModel : INotifyPropertyChanged
    {
        private InputTextWPF? Parent { get; }
        public InputTextViewModel(InputTextWPF parent)
        {
            this.Parent = parent;
        }

#pragma warning disable CS8618
        public InputTextViewModel() { }
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

        private string? input;
        public string? Input
        {
            get => input;
            set => NotifyPropertyChanged<string?>(ref input, value);
        }

        private string? instructions;
        public string? Instructions
        {
            get => instructions;
            set => NotifyPropertyChanged<string?>(ref instructions, value);
        }

        #endregion Value Bindings

        #region Commands

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
