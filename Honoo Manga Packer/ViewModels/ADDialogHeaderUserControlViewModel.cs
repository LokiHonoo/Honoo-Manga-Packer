using CommunityToolkit.Mvvm.ComponentModel;
using Honoo.MangaPacker.Models;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class ADDialogHeaderUserControlViewModel : ObservableObject
    {
        private readonly Settings _settings = ModelLocator.Settings;

        public ADDialogHeaderUserControlViewModel()
        {
        }

        public Settings Settings => _settings;
    }
}