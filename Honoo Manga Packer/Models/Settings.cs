using CommunityToolkit.Mvvm.ComponentModel;
using Honoo.Collections.ObjectModel;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Honoo.MangaPacker.Models
{
    public sealed class Settings : ObservableObject
    {
        private readonly ObservableDictionary<string, string> _ads = [];
        private readonly ObservableCollection<string> _errorMessages = [];
        private readonly ObservableCollection<string[]> _passwords = [];
        private readonly ObservableCollection<string> _tags = ["[中国翻訳]"];
        private bool _addTag;
        private bool _addTopTitle;
        private bool _deleteAD;
        private bool _executeAtDrop;
        private bool _moveToRecycleBin;
        private bool _packUnpacks;
        private bool _passwordRemoveConfirm = true;
        private bool _resetName;
        private string _selectedTag = string.Empty;
        private bool _settingExpanded = true;
        private bool _tagRemoveConfirm = true;
        private string _token = string.Empty;
        private bool _topmost;
        private bool _unpacksMoveToPacks;
        private int _windowLeft = 300;
        private int _windowTop = 300;
        private string _workDirectly = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MangaPack");
        public bool AddTag { get => _addTag; set => SetProperty(ref _addTag, value); }
        public bool AddTopTitle { get => _addTopTitle; set => SetProperty(ref _addTopTitle, value); }
        public ObservableDictionary<string, string> ADs => _ads;
        public bool DeleteAD { get => _deleteAD; set => SetProperty(ref _deleteAD, value); }
        public ObservableCollection<string> ErrorMessages => _errorMessages;
        public bool ExecuteAtDrop { get => _executeAtDrop; set => SetProperty(ref _executeAtDrop, value); }
        public bool MoveToRecycleBin { get => _moveToRecycleBin; set => SetProperty(ref _moveToRecycleBin, value); }
        public bool PackUnpacks { get => _packUnpacks; set => SetProperty(ref _packUnpacks, value); }
        public bool PasswordRemoveConfirm { get => _passwordRemoveConfirm; set => SetProperty(ref _passwordRemoveConfirm, value); }
        public ObservableCollection<string[]> Passwords => _passwords;
        public bool ResetName { get => _resetName; set => SetProperty(ref _resetName, value); }
        public string SelectedTag { get => _selectedTag; set => SetProperty(ref _selectedTag, value); }
        public bool SettingExpanded { get => _settingExpanded; set => SetProperty(ref _settingExpanded, value); }
        public bool TagRemoveConfirm { get => _tagRemoveConfirm; set => SetProperty(ref _tagRemoveConfirm, value); }
        public ObservableCollection<string> Tags => _tags;
        public string Token { get => _token; set => SetProperty(ref _token, value); }
        public bool Topmost { get => _topmost; set => SetProperty(ref _topmost, value); }
        public bool UnpacksMoveToPacks { get => _unpacksMoveToPacks; set => SetProperty(ref _unpacksMoveToPacks, value); }
        public int WindowLeft { get => _windowLeft; set => SetProperty(ref _windowLeft, value); }
        public int WindowTop { get => _windowTop; set => SetProperty(ref _windowTop, value); }
        public string WorkDirectly { get => _workDirectly; set => SetProperty(ref _workDirectly, value); }
    }
}