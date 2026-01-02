using CommunityToolkit.Mvvm.ComponentModel;
using Honoo.Configuration;
using System;
using System.Collections.Generic;
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
        private static readonly string _configFlie = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
        private static readonly string _passwordFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pwd.xml");

        [ObservableProperty] private long[] _ads = [];
        [ObservableProperty] private bool _packAddNest;
        [ObservableProperty] private bool _packClearTarget;
        [ObservableProperty] private bool _packConvertToWebP;
        [ObservableProperty] private bool _packDelSource;
        [ObservableProperty] private bool _packRemoveAD;
        [ObservableProperty] private bool _packRemoveNest;
        [ObservableProperty] private bool _settingExpanded = true;
        [ObservableProperty] private bool _topmost;
        [ObservableProperty] private bool _unpackClearTarget;
        [ObservableProperty] private bool _unpackDelSource;
        [ObservableProperty] private string _unpackEncoding = "UTF-8";
        [ObservableProperty] private bool _unpackSendToPack;
        [ObservableProperty] private bool _unpackTryPassword;
        [ObservableProperty] private int _windowLeft = 300;
        [ObservableProperty] private int _windowTop = 300;
        [ObservableProperty] private string _workDirectly = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "HMP-Work");
        public string[] UnpackEncodings { get; } = ["UTF-8", "Unicode", "GBK", "Big5", "Shift-JIS"];
        public HashSet<string> UnpackPasswords { get; set; } = [];

        internal void LoadConfig()
        {
            using XConfigManager manager = new(_configFlie, true);
            this.WindowTop = manager.Default.Properties.GetValue("WindowTop", new XString(this.WindowTop.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
            this.WindowLeft = manager.Default.Properties.GetValue("WindowLeft", new XString(this.WindowLeft.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
            this.Topmost = manager.Default.Properties.GetValue("Topmost", new XString(this.Topmost.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.SettingExpanded = manager.Default.Properties.GetValue("SettingExpanded", new XString(this.SettingExpanded.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();

            this.WorkDirectly = manager.Default.Properties.GetValue("WorkDirectly", new XString(this.WorkDirectly)).GetStringValue();

            this.UnpackEncoding = manager.Default.Properties.GetValue("UnpackEncoding", new XString(this.UnpackEncoding.ToString(CultureInfo.InvariantCulture))).GetStringValue();
            this.UnpackClearTarget = manager.Default.Properties.GetValue("UnpackClearTarget", new XString(this.UnpackClearTarget.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.UnpackTryPassword = manager.Default.Properties.GetValue("UnpackTryPassword", new XString(this.UnpackTryPassword.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.UnpackSendToPack = manager.Default.Properties.GetValue("UnpackSendToPack", new XString(this.UnpackSendToPack.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.UnpackDelSource = manager.Default.Properties.GetValue("UnpackDelSource", new XString(this.UnpackDelSource.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();

            this.PackClearTarget = manager.Default.Properties.GetValue("PackClearTarget", new XString(this.PackClearTarget.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.PackRemoveAD = manager.Default.Properties.GetValue("PackRemoveAD", new XString(this.PackRemoveAD.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.PackRemoveNest = manager.Default.Properties.GetValue("PackRemoveNest", new XString(this.PackRemoveNest.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.PackAddNest = manager.Default.Properties.GetValue("PackAddNest", new XString(this.PackAddNest.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.PackConvertToWebP = manager.Default.Properties.GetValue("PackConvertToWebP", new XString(this.PackConvertToWebP.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            this.PackDelSource = manager.Default.Properties.GetValue("PackDelSource", new XString(this.PackDelSource.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
        }

        internal void LoadPassword()
        {
            if (File.Exists(_passwordFile))
            {
                using XConfigManager manager = new(_passwordFile);
                foreach (var prop in manager.Default.Properties.GetValue<XList>("Passwords").Properties)
                {
                    this.UnpackPasswords.Add(((XString)prop).GetStringValue());
                }
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

            manager.Default.Properties.AddOrUpdate("UnpackEncoding", new XString(this.UnpackEncoding.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("UnpackClearTarget", new XString(this.UnpackClearTarget.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("UnpackTryPassword", new XString(this.UnpackTryPassword.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("UnpackSendToPack", new XString(this.UnpackSendToPack.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("UnpackDelSource", new XString(this.UnpackDelSource.ToString(CultureInfo.InvariantCulture)));

            manager.Default.Properties.AddOrUpdate("PackClearTarget", new XString(this.PackClearTarget.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("PackRemoveAD", new XString(this.PackRemoveAD.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("PackRemoveNest", new XString(this.PackRemoveNest.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("PackAddNest", new XString(this.PackAddNest.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("PackConvertToWebP", new XString(this.PackConvertToWebP.ToString(CultureInfo.InvariantCulture)));
            manager.Default.Properties.AddOrUpdate("PackDelSource", new XString(this.PackDelSource.ToString(CultureInfo.InvariantCulture)));

            manager.Save(_configFlie);
        }

        internal void SavePassword()
        {
            using XConfigManager manager = new();
            XList list = manager.Default.Properties.Add("Passwords", new XList());
            foreach (var password in this.UnpackPasswords)
            {
                list.Properties.AddString(password);
            }
            manager.Save(_passwordFile);
        }
    }
}