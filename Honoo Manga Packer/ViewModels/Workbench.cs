using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed partial class Workbench : ObservableObject
    {
        [ObservableProperty]
        private bool _abort;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private bool _isRunning;

        [ObservableProperty]
        private double _progress;

        public ObservableCollection<Tuple<bool, string, Exception?>> Log { get; } = [];
        public ObservableCollection<string> Projects { get; } = [];
    }
}