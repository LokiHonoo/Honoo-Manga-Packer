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
    public sealed partial class Settings : ObservableObject
    {
        #region Instance

        public static Settings Instance { get; } = new Settings();

        #endregion Instance

        private static readonly string _adFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ad.xml");
        private static readonly string _configFlie = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mp.xml");
        private static readonly string _passwordFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pwd.xml");

        [ObservableProperty] private bool _addTag;
        [ObservableProperty] private bool _addTopTitle;
        [ObservableProperty] private bool _clearWorkDirectly = false;
        [ObservableProperty] private bool _deleteAD;
        [ObservableProperty] private bool _executeAtDrop;
        [ObservableProperty] private bool _moveToRecycleBin;
        [ObservableProperty] private bool _packUnpacks;
        [ObservableProperty] private bool _passwordRemoveConfirm = true;
        [ObservableProperty] private bool _resetName = true;
        [ObservableProperty] private string _selectedTag = string.Empty;
        [ObservableProperty] private bool _settingExpanded = true;
        [ObservableProperty] private bool _tagRemoveConfirm = true;
        [ObservableProperty] private bool _topmost;
        [ObservableProperty] private string _unpackEncoding = "UTF-8";
        [ObservableProperty] private bool _unpacksMoveToPacks;
        [ObservableProperty] private int _windowLeft = 300;
        [ObservableProperty] private int _windowTop = 300;
        [ObservableProperty] private string _workDirectly = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MangaPacker");
        public ObservableDictionary<string, int> ADs { get; } = [];
        public ObservableDictionary<string, int> Passwords { get; } = [];
        public ObservableCollection<string> Tags { get; } = ["[中国翻訳]"];

        public string[] UnpackEncodings { get; } = ["UTF-8", "Unicode", "GBK", "Big5", "Shift-JIS"];

        internal void LoadAD()
        {
            using XConfigManager manager = File.Exists(_adFile) ? new(_adFile) : new();
            this.ADs.Clear();

            foreach (var property in manager.Default.Properties)
            {
                this.ADs.Add(property.Key, ((XString)property.Value).GetInt32Value());
            }
        }

        internal void LoadConfig()
        {
            using XConfigManager manager = new(_configFlie, true);
            this.WindowTop = manager.Default.Properties.GetValue("WindowTop", new XString(this.WindowTop.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
            this.WindowLeft = manager.Default.Properties.GetValue("WindowLeft", new XString(this.WindowLeft.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
            this.Topmost = manager.Default.Properties.GetValue("Topmost", new XString(this.Topmost.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.SettingExpanded = manager.Default.Properties.GetValue("SettingExpanded", new XString(this.SettingExpanded.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.WorkDirectly = manager.Default.Properties.GetValue("WorkDirectly", new XString(this.WorkDirectly)).GetStringValue();
            this.ResetName = manager.Default.Properties.GetValue("ResetName", new XString(this.ResetName.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.ExecuteAtDrop = manager.Default.Properties.GetValue("ExecuteAtDrop", new XString(this.ExecuteAtDrop.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.ClearWorkDirectly = manager.Default.Properties.GetValue("ClearWorkDirectly", new XString(this.ClearWorkDirectly.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.MoveToRecycleBin = manager.Default.Properties.GetValue("MoveToRecycleBin", new XString(this.MoveToRecycleBin.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.PasswordRemoveConfirm = manager.Default.Properties.GetValue("PasswordRemoveConfirm", new XString(this.PasswordRemoveConfirm.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.UnpackEncoding = manager.Default.Properties.GetValue("UnpackEncoding", new XString(this.UnpackEncoding.ToString(CultureInfo.InvariantCulture))).GetStringValue();
            this.UnpacksMoveToPacks = manager.Default.Properties.GetValue("UnpacksMoveToPacks", new XString(this.UnpacksMoveToPacks.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.PackUnpacks = manager.Default.Properties.GetValue("PackUnpacks", new XString(this.PackUnpacks.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.DeleteAD = manager.Default.Properties.GetValue("DeleteAD", new XString(this.DeleteAD.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.AddTopTitle = manager.Default.Properties.GetValue("AddTopTitle", new XString(this.AddTopTitle.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            if (manager.Default.Properties.TryGetValue("Tags", out XList tags))
            {
                this.Tags.Clear();
                foreach (var tag in tags.Properties)
                {
                    this.Tags.Add(((XString)tag).GetStringValue());
                }
            }
            this.AddTag = manager.Default.Properties.GetValue("AddTag", new XString(this.AddTag.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.SelectedTag = manager.Default.Properties.GetValue("SelectedTag", new XString(string.Empty)).GetStringValue();
            this.TagRemoveConfirm = manager.Default.Properties.GetValue("TagRemoveConfirm", new XString(this.TagRemoveConfirm.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
        }

        internal void LoadPassword()
        {
            using XConfigManager manager = File.Exists(_passwordFile) ? new(_passwordFile) : new();
            var ps = new List<KeyValuePair<string, int>>();
            this.Passwords.Clear();
            foreach (var password in manager.Default.Properties)
            {
                ps.Add(new KeyValuePair<string, int>(password.Key, ((XString)password.Value).GetInt32Value()));
            }
            ps.Sort((a, b) => a.Value.CompareTo(b.Value));
            foreach (var password in ps)
            {
                this.Passwords.Add(password.Key, password.Value);
            }
        }

        internal void SaveConfig()
        {
            using XConfigManager manager = new();
            manager.Default.Properties.AddOrUpdate("WindowTop", new XString(this.WindowTop.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("WindowLeft", new XString(this.WindowLeft.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("Topmost", new XString(this.Topmost.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("SettingExpanded", new XString(this.SettingExpanded.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("WorkDirectly", new XString(this.WorkDirectly));
            manager.Default.Properties.AddOrUpdate("ResetName", new XString(this.ResetName.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("ExecuteAtDrop", new XString(this.ExecuteAtDrop.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("ClearWorkDirectly", new XString(this.ClearWorkDirectly.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("MoveToRecycleBin", new XString(this.MoveToRecycleBin.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("PasswordRemoveConfirm", new XString(this.PasswordRemoveConfirm.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("UnpackEncoding", new XString(this.UnpackEncoding.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("UnpacksMoveToPacks", new XString(this.UnpacksMoveToPacks.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("PackUnpacks", new XString(this.PackUnpacks.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("DeleteAD", new XString(this.DeleteAD.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("AddTopTitle", new XString(this.AddTopTitle.ToString(CultureInfo.InvariantCulture)));
            XList tags = manager.Default.Properties.AddOrUpdate("Tags", new XList());
            foreach (var tag in this.Tags)
            {
                tags.Properties.Add(new XString(tag));
            }
            manager.Default.Properties.AddOrUpdate("AddTag", new XString(this.AddTag.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("SelectedTag", new XString(this.SelectedTag));
            manager.Default.Properties.AddOrUpdate("TagRemoveConfirm", new XString(this.TagRemoveConfirm.ToString(CultureInfo.InvariantCulture)));
            manager.Save(_configFlie);
        }

        internal void SavePassword()
        {
            using XConfigManager manager = new();
            foreach (var password in this.Passwords)
            {
                manager.Default.Properties.AddOrUpdate(password.Key, new XString(password.Value.ToString(CultureInfo.InvariantCulture)));
            }
            manager.Save(_passwordFile);
        }
    }
}