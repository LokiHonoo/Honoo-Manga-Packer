using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Honoo.MangaPacker.Models;
using HonooUI.WPF;
using System.Globalization;
using System.Windows.Input;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class PasswordDialogUserControlViewModel : ObservableObject
    {
        private readonly Settings _settings = ModelLocator.Settings;
        private string _password = string.Empty;

        public PasswordDialogUserControlViewModel()
        {
            this.AddPasswordCommand = new RelayCommand(AddPasswordExecute, () => { return !string.IsNullOrWhiteSpace(this.Password); });
            this.RemovePasswordCommand = new RelayCommand<string?>(RemovePasswordExecute);
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
        public Settings Settings => _settings;

        private void AddPasswordExecute()
        {
            int weights = 0;
            for (int i = this.Settings.Passwords.Count - 1; i >= 0; i--)
            {
                if (this.Password == this.Settings.Passwords[i][0])
                {
                    if (int.TryParse(this.Settings.Passwords[i][1], out int w))
                    {
                        weights += w;
                    }
                    this.Settings.Passwords.RemoveAt(i);
                }
            }
            this.Settings.Passwords.Insert(0, [this.Password, weights.ToString(CultureInfo.InvariantCulture)]);
            this.Password = string.Empty;
        }

        private void RemovePasswordExecute(string? password)
        {
            DialogOptions dialogOptions = new() { Buttons = DialogButtons.YesNo, Image = DialogImage.Information };
            DialogManager.GetDialogHost("SubDialogHost").Show($"删除 \"{password}\"？", string.Empty, dialogOptions, (e) =>
            {
                if (e.DialogResult == DialogResult.Yes)
                {
                    for (int i = this.Settings.Passwords.Count - 1; i >= 0; i--)
                    {
                        if (password == this.Settings.Passwords[i][0])
                        {
                            this.Settings.Passwords.RemoveAt(i);
                        }
                    }
                }
            }, null);
        }
    }
}