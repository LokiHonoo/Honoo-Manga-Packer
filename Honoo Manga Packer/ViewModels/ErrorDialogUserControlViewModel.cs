using CommunityToolkit.Mvvm.ComponentModel;
using Honoo.MangaPacker.Models;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class ErrorDialogUserControlViewModel : ObservableObject
    {
        private readonly Settings _settings = ModelLocator.Settings;

        public ErrorDialogUserControlViewModel()
        {
        }

        public Settings Settings => _settings;
    }
}