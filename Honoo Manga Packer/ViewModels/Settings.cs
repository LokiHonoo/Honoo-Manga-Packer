using CommunityToolkit.Mvvm.ComponentModel;
using Honoo.Collections.ObjectModel;
using Honoo.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace Honoo.MangaPacker.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:考虑将公共类型设为内部类型", Justification = "<挂起>")]
    public sealed class Settings : ObservableObject
    {
        private static readonly string _adFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ad.xml");
        private static readonly string _configFlie = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mp.xml");
        private static readonly string _passwordFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pwd.xml");
        private readonly ObservableDictionary<string, int> _ads = [];
        private readonly ObservableCollection<string> _tags = ["[中国翻訳]"];
        private bool _addTag;
        private bool _addTopTitle;
        private bool _clearWorkDirectly = false;
        private bool _deleteAD;
        private bool _executeAtDrop;
        private bool _moveToRecycleBin;
        private bool _packUnpacks;
        private bool _passwordRemoveConfirm = true;
        private ObservableDictionary<string, int> _passwords = [];
        private bool _resetName = true;
        private string _selectedTag = string.Empty;
        private bool _settingExpanded = true;
        private bool _tagRemoveConfirm = true;
        private bool _topmost;
        private string _unpackEncoding = "UTF-8";
        private bool _unpacksMoveToPacks;
        private int _windowLeft = 300;
        private int _windowTop = 300;
        private string _workDirectly = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MangaPacker");
        public static Settings Instance { get; } = new Settings();
        public bool AddTag { get => _addTag; set => SetProperty(ref _addTag, value); }
        public bool AddTopTitle { get => _addTopTitle; set => SetProperty(ref _addTopTitle, value); }
        public ObservableDictionary<string, int> ADs => _ads;
        public bool ClearWorkDirectly { get => _clearWorkDirectly; set => SetProperty(ref _clearWorkDirectly, value); }
        public bool DeleteAD { get => _deleteAD; set => SetProperty(ref _deleteAD, value); }
        public bool ExecuteAtDrop { get => _executeAtDrop; set => SetProperty(ref _executeAtDrop, value); }
        public bool MoveToRecycleBin { get => _moveToRecycleBin; set => SetProperty(ref _moveToRecycleBin, value); }
        public bool PackUnpacks { get => _packUnpacks; set => SetProperty(ref _packUnpacks, value); }
        public bool PasswordRemoveConfirm { get => _passwordRemoveConfirm; set => SetProperty(ref _passwordRemoveConfirm, value); }
        public ObservableDictionary<string, int> Passwords { get => _passwords; set => SetProperty(ref _passwords, value); }
        public bool ResetName { get => _resetName; set => SetProperty(ref _resetName, value); }
        public string SelectedTag { get => _selectedTag; set => SetProperty(ref _selectedTag, value); }
        public bool SettingExpanded { get => _settingExpanded; set => SetProperty(ref _settingExpanded, value); }
        public bool TagRemoveConfirm { get => _tagRemoveConfirm; set => SetProperty(ref _tagRemoveConfirm, value); }
        public ObservableCollection<string> Tags => _tags;
        public bool Topmost { get => _topmost; set => SetProperty(ref _topmost, value); }
        public string UnpackEncoding { get => _unpackEncoding; set => SetProperty(ref _unpackEncoding, value); }
        public string[] UnpackEncodings { get; } = ["UTF-8", "GBK"];
        public bool UnpacksMoveToPacks { get => _unpacksMoveToPacks; set => SetProperty(ref _unpacksMoveToPacks, value); }
        public int WindowLeft { get => _windowLeft; set => SetProperty(ref _windowLeft, value); }
        public int WindowTop { get => _windowTop; set => SetProperty(ref _windowTop, value); }
        public string WorkDirectly { get => _workDirectly; set => SetProperty(ref _workDirectly, value); }

        internal static void LoadAD()
        {
            using (XConfigManager manager = File.Exists(_adFile) ? new(_adFile) : new())
            {
                Instance.ADs.Clear();

                foreach (var property in manager.Default.Properties)
                {
                    Instance.ADs.Add(property.Key, ((XString)property.Value).GetInt32Value());
                }
            }
        }

        internal static void LoadConfig()
        {
            using (XConfigManager manager = File.Exists(_configFlie) ? new(_configFlie) : new())
            {
                Instance.WindowTop = manager.Default.Properties.GetValue("WindowTop", new XString(Instance.WindowTop.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
                Instance.WindowLeft = manager.Default.Properties.GetValue("WindowLeft", new XString(Instance.WindowLeft.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
                Instance.Topmost = manager.Default.Properties.GetValue("Topmost", new XString(Instance.Topmost.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.SettingExpanded = manager.Default.Properties.GetValue("SettingExpanded", new XString(Instance.SettingExpanded.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.WorkDirectly = manager.Default.Properties.GetValue("WorkDirectly", new XString(Instance.WorkDirectly)).GetStringValue();
                Instance.ResetName = manager.Default.Properties.GetValue("ResetName", new XString(Instance.ResetName.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.ExecuteAtDrop = manager.Default.Properties.GetValue("ExecuteAtDrop", new XString(Instance.ExecuteAtDrop.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.ClearWorkDirectly = manager.Default.Properties.GetValue("ClearWorkDirectly", new XString(Instance.ClearWorkDirectly.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.MoveToRecycleBin = manager.Default.Properties.GetValue("MoveToRecycleBin", new XString(Instance.MoveToRecycleBin.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.PasswordRemoveConfirm = manager.Default.Properties.GetValue("PasswordRemoveConfirm", new XString(Instance.PasswordRemoveConfirm.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.UnpackEncoding = manager.Default.Properties.GetValue("UnpackEncoding", new XString(Instance.UnpackEncoding.ToString(CultureInfo.InvariantCulture))).GetStringValue();
                Instance.UnpacksMoveToPacks = manager.Default.Properties.GetValue("UnpacksMoveToPacks", new XString(Instance.UnpacksMoveToPacks.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.PackUnpacks = manager.Default.Properties.GetValue("PackUnpacks", new XString(Instance.PackUnpacks.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.DeleteAD = manager.Default.Properties.GetValue("DeleteAD", new XString(Instance.DeleteAD.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.AddTopTitle = manager.Default.Properties.GetValue("AddTopTitle", new XString(Instance.AddTopTitle.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                if (manager.Default.Properties.TryGetValue("Tags", out XList tags))
                {
                    Instance.Tags.Clear();
                    foreach (var tag in tags.Properties)
                    {
                        Instance.Tags.Add(((XString)tag).GetStringValue());
                    }
                }
                Instance.AddTag = manager.Default.Properties.GetValue("AddTag", new XString(Instance.AddTag.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                Instance.SelectedTag = manager.Default.Properties.GetValue("SelectedTag", new XString(string.Empty)).GetStringValue();
                Instance.TagRemoveConfirm = manager.Default.Properties.GetValue("TagRemoveConfirm", new XString(Instance.TagRemoveConfirm.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            }
        }

        internal static void LoadPassword()
        {
            using (XConfigManager manager = File.Exists(_passwordFile) ? new(_passwordFile) : new())
            {
                var ps = new List<KeyValuePair<string, int>>();
                Instance.Passwords.Clear();
                foreach (var password in manager.Default.Properties)
                {
                    ps.Add(new KeyValuePair<string, int>(password.Key, ((XString)password.Value).GetInt32Value()));
                }
                ps.Sort((a, b) => a.Value.CompareTo(b.Value));
                foreach (var password in ps)
                {
                    Instance.Passwords.Add(password.Key, password.Value);
                }
            }
        }

        internal static void SaveConfig()
        {
            using (XConfigManager manager = new())
            {
                manager.Default.Properties.AddOrUpdate("WindowTop", new XString(Instance.WindowTop.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("WindowLeft", new XString(Instance.WindowLeft.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("Topmost", new XString(Instance.Topmost.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("SettingExpanded", new XString(Instance.SettingExpanded.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("WorkDirectly", new XString(Instance.WorkDirectly));
                manager.Default.Properties.AddOrUpdate("ResetName", new XString(Instance.ResetName.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("ExecuteAtDrop", new XString(Instance.ExecuteAtDrop.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("ClearWorkDirectly", new XString(Instance.ClearWorkDirectly.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("MoveToRecycleBin", new XString(Instance.MoveToRecycleBin.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("PasswordRemoveConfirm", new XString(Instance.PasswordRemoveConfirm.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("UnpackEncoding", new XString(Instance.UnpackEncoding.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("UnpacksMoveToPacks", new XString(Instance.UnpacksMoveToPacks.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("PackUnpacks", new XString(Instance.PackUnpacks.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("DeleteAD", new XString(Instance.DeleteAD.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("AddTopTitle", new XString(Instance.AddTopTitle.ToString(CultureInfo.InvariantCulture)));
                XList tags = manager.Default.Properties.AddOrUpdate("Tags", new XList());
                foreach (var tag in Instance.Tags)
                {
                    tags.Properties.Add(new XString(tag));
                }
                manager.Default.Properties.AddOrUpdate("AddTag", new XString(Instance.AddTag.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("SelectedTag", new XString(Instance.SelectedTag));
                manager.Default.Properties.AddOrUpdate("TagRemoveConfirm", new XString(Instance.TagRemoveConfirm.ToString(CultureInfo.InvariantCulture)));
                manager.Save(_configFlie);
            }
        }

        internal static void SavePassword()
        {
            using (XConfigManager manager = new())
            {
                foreach (var password in Instance.Passwords)
                {
                    manager.Default.Properties.AddOrUpdate(password.Key, new XString(password.Value.ToString(CultureInfo.InvariantCulture)));
                }
                manager.Save(_passwordFile);
            }
        }
    }
}