using Honoo.Configuration;
using Honoo.MangaPacker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Honoo.MangaPacker
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:考虑将公共类型设为内部类型", Justification = "<挂起>")]
    public partial class App : Application
    {
        private static readonly string _adListFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ads.xml");
        private static readonly string _configFlie = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mp.xml");

        #region App

        public App()
        {
            this.Startup += App_Startup;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            SaveConfig();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (Honoo.Threading.App.PrevInstance("4Bvmw9BMhj2BQFHb"))
            {
                Current.Shutdown();
            }
            else
            {
                try
                {
                    LoadConfig();
                }
                catch
                {
                }
                this.Exit += App_Exit;
            }
        }

        #endregion App

        public static void LoadConfig()
        {
            using (XConfigManager manager = File.Exists(_configFlie) ? new(_configFlie) : new())
            {
                ModelLocator.Settings.WindowTop = manager.Default.Properties.GetValue("WindowTop", new XString(ModelLocator.Settings.WindowTop.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
                ModelLocator.Settings.WindowLeft = manager.Default.Properties.GetValue("WindowLeft", new XString(ModelLocator.Settings.WindowLeft.ToString(CultureInfo.InvariantCulture))).GetInt32Value();
                ModelLocator.Settings.Topmost = manager.Default.Properties.GetValue("Topmost", new XString(ModelLocator.Settings.Topmost.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.SettingExpanded = manager.Default.Properties.GetValue("SettingExpanded", new XString(ModelLocator.Settings.SettingExpanded.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.WorkDirectly = manager.Default.Properties.GetValue("WorkDirectly", new XString(ModelLocator.Settings.WorkDirectly)).GetStringValue();
                ModelLocator.Settings.ResetName = manager.Default.Properties.GetValue("ResetName", new XString(ModelLocator.Settings.ResetName.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.MoveToRecycleBin = manager.Default.Properties.GetValue("MoveToRecycleBin", new XString(ModelLocator.Settings.MoveToRecycleBin.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.ExecuteAtDrop = manager.Default.Properties.GetValue("ExecuteAtDrop", new XString(ModelLocator.Settings.ExecuteAtDrop.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                if (manager.Default.Properties.TryGetValue("Passwords", out XDictionary passwords))
                {
                    List<string[]> ps = [];
                    ModelLocator.Settings.Passwords.Clear();
                    foreach (var password in passwords.Properties)
                    {
                        ps.Add([password.Key, ((XString)password.Value).GetStringValue()]);
                    }
                    ps.Sort((x, y) => { return string.CompareOrdinal(x[1], y[1]); });
                    foreach (var password in ps)
                    {
                        ModelLocator.Settings.Passwords.Add(password);
                    }
                }
                ModelLocator.Settings.PasswordRemoveConfirm = manager.Default.Properties.GetValue("PasswordRemoveConfirm", new XString(ModelLocator.Settings.PasswordRemoveConfirm.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.UnpacksMoveToPacks = manager.Default.Properties.GetValue("UnpacksMoveToPacks", new XString(ModelLocator.Settings.UnpacksMoveToPacks.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.PackUnpacks = manager.Default.Properties.GetValue("PackUnpacks", new XString(ModelLocator.Settings.PackUnpacks.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.DeleteAD = manager.Default.Properties.GetValue("DeleteAD", new XString(ModelLocator.Settings.DeleteAD.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.AddTopTitle = manager.Default.Properties.GetValue("AddTopTitle", new XString(ModelLocator.Settings.AddTopTitle.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();

                if (manager.Default.Properties.TryGetValue("Tags", out XList tags))
                {
                    ModelLocator.Settings.Tags.Clear();
                    foreach (var tag in tags.Properties)
                    {
                        ModelLocator.Settings.Tags.Add(((XString)tag).GetStringValue());
                    }
                }
                ModelLocator.Settings.AddTag = manager.Default.Properties.GetValue("AddTag", new XString(ModelLocator.Settings.AddTag.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
                ModelLocator.Settings.SelectedTag = manager.Default.Properties.GetValue("SelectedTag", new XString(string.Empty)).GetStringValue();
                ModelLocator.Settings.TagRemoveConfirm = manager.Default.Properties.GetValue("TagRemoveConfirm", new XString(ModelLocator.Settings.TagRemoveConfirm.ToString(CultureInfo.InvariantCulture))).GetBooleanValue();
            }

            using (XConfigManager manager = File.Exists(_adListFile) ? new(_adListFile) : new())
            {
                ModelLocator.Settings.ADToken = manager.Default.Properties.GetValue("Token", new XString(Guid.NewGuid().ToString())).GetStringValue();
                ModelLocator.Settings.ADs.Clear();
                if (manager.Sections.TryGetValue("ADs", out XDictionary ads))
                {
                    foreach (var property in ads.Properties)
                    {
                        ModelLocator.Settings.ADs.Add(property.Key, ((XString)property.Value).GetInt32Value());
                    }
                }
            }
        }

        public static void SaveConfig()
        {
            using (XConfigManager manager = new())
            {
                manager.Default.Properties.AddOrUpdate("WindowTop", new XString(ModelLocator.Settings.WindowTop.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("WindowLeft", new XString(ModelLocator.Settings.WindowLeft.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("Topmost", new XString(ModelLocator.Settings.Topmost.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("SettingExpanded", new XString(ModelLocator.Settings.SettingExpanded.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("WorkDirectly", new XString(ModelLocator.Settings.WorkDirectly));
                manager.Default.Properties.AddOrUpdate("ResetName", new XString(ModelLocator.Settings.ResetName.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("MoveToRecycleBin", new XString(ModelLocator.Settings.MoveToRecycleBin.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("ExecuteAtDrop", new XString(ModelLocator.Settings.ExecuteAtDrop.ToString(CultureInfo.InvariantCulture)));
                XDictionary passwords = manager.Default.Properties.AddOrUpdate("Passwords", new XDictionary());
                foreach (var password in ModelLocator.Settings.Passwords)
                {
                    passwords.Properties.AddOrUpdate(password[0], new XString(password[1].ToString(CultureInfo.InvariantCulture)));
                }
                manager.Default.Properties.AddOrUpdate("PasswordRemoveConfirm", new XString(ModelLocator.Settings.PasswordRemoveConfirm.ToString(CultureInfo.InvariantCulture)));

                manager.Default.Properties.AddOrUpdate("UnpacksMoveToPacks", new XString(ModelLocator.Settings.UnpacksMoveToPacks.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("PackUnpacks", new XString(ModelLocator.Settings.PackUnpacks.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("DeleteAD", new XString(ModelLocator.Settings.DeleteAD.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("AddTopTitle", new XString(ModelLocator.Settings.AddTopTitle.ToString(CultureInfo.InvariantCulture)));
                XList tags = manager.Default.Properties.AddOrUpdate("Tags", new XList());
                foreach (var tag in ModelLocator.Settings.Tags)
                {
                    tags.Properties.Add(new XString(tag));
                }
                manager.Default.Properties.AddOrUpdate("AddTag", new XString(ModelLocator.Settings.AddTag.ToString(CultureInfo.InvariantCulture)));
                manager.Default.Properties.AddOrUpdate("SelectedTag", new XString(ModelLocator.Settings.SelectedTag));
                manager.Default.Properties.AddOrUpdate("TagRemoveConfirm", new XString(ModelLocator.Settings.TagRemoveConfirm.ToString(CultureInfo.InvariantCulture)));
                manager.Save(_configFlie);
            }
        }
    }
}