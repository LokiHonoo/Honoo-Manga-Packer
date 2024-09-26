using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class Workbench : ObservableObject
    {
        private readonly ObservableCollection<Tuple<string, string, bool, Exception?>> _Log = [];
        private readonly ObservableCollection<string> _projects = [];
        private bool _abort;
        private bool _hasError;
        private bool _isRunning;
        private double _progress;
        public bool Abort { get => _abort; set => SetProperty(ref _abort, value); }
        public bool HasError { get => _hasError; set => SetProperty(ref _hasError, value); }
        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }
        public ObservableCollection<Tuple<string, string, bool, Exception?>> Log => _Log;
        public double Progress { get => _progress; set => SetProperty(ref _progress, value); }
        public ObservableCollection<string> Projects => _projects;
    }
}