using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed partial class PackWorkbench : ObservableObject
    {
        #region Instance

        public static PackWorkbench Instance { get; } = new PackWorkbench();

        #endregion Instance

        [ObservableProperty]
        private bool _abort;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private bool _isRunning;

        [ObservableProperty]
        private double _progress;

        public ObservableCollection<string> Logs { get; } = [];
        public ObservableCollection<string> Projects { get; } = [];
    }
}