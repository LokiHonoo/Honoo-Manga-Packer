using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Honoo.MangaPacker.Models;
using HonooUI.WPF;
using System.Windows.Input;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class TagDialogUserControlViewModel : ObservableObject
    {
        private readonly Settings _settings = ModelLocator.Settings;
        private string _tag = string.Empty;

        public TagDialogUserControlViewModel()
        {
            this.AddTagCommand = new RelayCommand(AddTag, () => { return !string.IsNullOrWhiteSpace(this.Tag); });
            this.MoveUpTagCommand = new RelayCommand<string?>(MoveUpTag);
            this.MoveDownTagCommand = new RelayCommand<string?>(MoveDownTag);
            this.RemoveTagCommand = new RelayCommand<string?>(RemoveTag);
        }

        public ICommand AddTagCommand { get; set; }

        public ICommand MoveDownTagCommand { get; set; }

        public ICommand MoveUpTagCommand { get; set; }

        public ICommand RemoveTagCommand { get; set; }

        public Settings Settings => _settings;

        public string Tag
        {
            get => _tag; set
            {
                SetProperty(ref _tag, value);
                ((IRelayCommand)this.AddTagCommand).NotifyCanExecuteChanged();
            }
        }

        private void AddTag()
        {
            for (int i = this.Settings.Tags.Count - 1; i >= 0; i--)
            {
                if (this.Tag == this.Settings.Tags[i])
                {
                    this.Settings.Tags.RemoveAt(i);
                }
            }
            this.Settings.Tags.Insert(0, this.Tag);
            this.Tag = string.Empty;
        }

        private void MoveDownTag(string? tag)
        {
            for (int i = 0; i < this.Settings.Tags.Count; i++)
            {
                if (tag == this.Settings.Tags[i])
                {
                    if (i != this.Settings.Tags.Count - 1)
                    {
                        this.Settings.Tags.Move(i, i + 1);
                        return;
                    }
                }
            }
        }

        private void MoveUpTag(string? tag)
        {
            for (int i = 0; i < this.Settings.Tags.Count; i++)
            {
                if (tag == this.Settings.Tags[i])
                {
                    if (i != 0)
                    {
                        this.Settings.Tags.Move(i, i - 1);
                        return;
                    }
                }
            }
        }

        private void RemoveTag(string? tag)
        {
            DialogManager.GetDialogHost("SubDialogHost").Show($"删除 \"{tag}\"？", string.Empty,
                DialogButtons.YesNo,
                DialogCloseButton.Ordinary,
                DialogImage.Information,
                ModelLocator.DialogOptionsAuto,
                null,
                (e) =>
                {
                    if (e.DialogResult == DialogResult.Yes)
                    {
                        for (int i = this.Settings.Tags.Count - 1; i >= 0; i--)
                        {
                            if (tag == this.Settings.Tags[i])
                            {
                                this.Settings.Tags.RemoveAt(i);
                            }
                        }
                    }
                }, null);
        }
    }
}