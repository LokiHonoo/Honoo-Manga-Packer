using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Honoo.Collections.ObjectModel;
using HonooUI.WPF;
using System.Windows.Input;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class PasswordDialogUserControlViewModel : ObservableObject
    {
        private string _password = string.Empty;

        public PasswordDialogUserControlViewModel()
        {
            this.AddPasswordCommand = new RelayCommand(AddPassword, () => { return !string.IsNullOrWhiteSpace(this.Password); });
            this.RemovePasswordCommand = new RelayCommand<string?>(RemovePassword);
        }

        public ICommand AddPasswordCommand { get; set; }

        public string Password
        {
            get => _password; set
            {
                SetProperty(ref _password, value);
                ((IRelayCommand)this.AddPasswordCommand).NotifyCanExecuteChanged();
            }
        }

        public ICommand RemovePasswordCommand { get; set; }
        public Settings Settings => Settings.Instance;

        private void AddPassword()
        {
            var psswords = new ObservableDictionary<string, int>();
            if (this.Settings.Passwords.TryGetValue(this.Password, out int weights))
            {
                this.Settings.Passwords.Remove(this.Password);
            }
            psswords.Add(this.Password, weights);
            foreach (var password in this.Settings.Passwords)
            {
                psswords.Add(password.Key, password.Value);
            }
            this.Settings.Passwords = psswords;
            this.Password = string.Empty;
        }

        private void RemovePassword(string? password)
        {
            if (password != null)
            {
                DialogManager.GetDialogHost("SubDialogHost").Show($"删除 \"{password}\"？", string.Empty,
                    DialogButtons.YesNo,
                    DialogCloseButton.Ordinary,
                    DialogImage.Information,
                    DialogOptions.Default,
                    null,
                    (e) =>
                    {
                        if (e.DialogResult == DialogResult.Yes)
                        {
                            this.Settings.Passwords.Remove(password);
                        }
                    }, null);
            }
        }
    }
}