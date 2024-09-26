using Honoo.Configuration;
using Honoo.MangaPacker.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Honoo.MangaPacker
{
    public partial class App : Application
    {
        private static readonly string _adListFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ads.xml");
        private static readonly string _appConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Honoo Manga Toolbox");
        private static readonly string _appConfigFlie = Path.Combine(_appConfigDirectory, "MangaPacker.xml");
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
                LoadConfig();
                this.Exit += App_Exit;
            }
        }

        #endregion App

        public static void LoadConfig()
        {
            using (HonooSettingsManager manager = File.Exists(_appConfigFlie) ? new(_appConfigFlie) : new())
            {
                ModelLocator.Settings.WindowTop = manager.Default.Properties.GetValue("WindowTop", ModelLocator.Settings.WindowTop);
                ModelLocator.Settings.WindowLeft = manager.Default.Properties.GetValue("WindowLeft", ModelLocator.Settings.WindowLeft);
                ModelLocator.Settings.Topmost = manager.Default.Properties.GetValue("Topmost", ModelLocator.Settings.Topmost);
            }

            using (HonooSettingsManager manager = File.Exists(_configFlie) ? new(_configFlie) : new())
            {
                ModelLocator.Settings.SettingExpanded = manager.Default.Properties.GetValue("SettingExpanded", ModelLocator.Settings.SettingExpanded);
                ModelLocator.Settings.WorkDirectly = manager.Default.Properties.GetValue("WorkDirectly", ModelLocator.Settings.WorkDirectly);
                ModelLocator.Settings.ExecuteAtDrop = manager.Default.Properties.GetValue("ExecuteAtDrop", ModelLocator.Settings.ExecuteAtDrop);
                ModelLocator.Settings.ResetName = manager.Default.Properties.GetValue("ResetName", ModelLocator.Settings.ResetName);
                ModelLocator.Settings.MoveToRecycleBin = manager.Default.Properties.GetValue("MoveToRecycleBin", ModelLocator.Settings.MoveToRecycleBin);
                ModelLocator.Settings.UnpacksMoveToPacks = manager.Default.Properties.GetValue("UnpacksMoveToPacks", ModelLocator.Settings.UnpacksMoveToPacks);
                ModelLocator.Settings.PackUnpacks = manager.Default.Properties.GetValue("PackUnpacks", ModelLocator.Settings.PackUnpacks);
                ModelLocator.Settings.DeleteAD = manager.Default.Properties.GetValue("DeleteAD", ModelLocator.Settings.DeleteAD);
                ModelLocator.Settings.AddTopTitle = manager.Default.Properties.GetValue("AddTopTitle", ModelLocator.Settings.AddTopTitle);
                ModelLocator.Settings.AddTag = manager.Default.Properties.GetValue("AddTag", ModelLocator.Settings.AddTag);
                ModelLocator.Settings.SelectedTag = manager.Default.Properties.GetValue("SelectedTag", string.Empty);
                //
                if (manager.Default.Properties.TryGetValue("Passwords", out string[][] passwords))
                {
                    Array.Sort(passwords, (x, y) => { return string.CompareOrdinal(x[1], y[1]); });
                    ModelLocator.Settings.Passwords.Clear();
                    foreach (string[] password in passwords)
                    {
                        ModelLocator.Settings.Passwords.Add(password);
                    }
                }
                ModelLocator.Settings.PasswordRemoveConfirm = manager.Default.Properties.GetValue("PasswordRemoveConfirm", ModelLocator.Settings.PasswordRemoveConfirm);
                //
                if (manager.Default.Properties.TryGetValue("Tags", out string[] tags))
                {
                    ModelLocator.Settings.Tags.Clear();
                    foreach (string tag in tags)
                    {
                        ModelLocator.Settings.Tags.Add(tag);
                    }
                }
                ModelLocator.Settings.TagRemoveConfirm = manager.Default.Properties.GetValue("TagRemoveConfirm", ModelLocator.Settings.TagRemoveConfirm);
            }

            using (HonooSettingsManager manager = File.Exists(_adListFile) ? new(_adListFile) : new())
            {
                ModelLocator.Settings.Token = manager.Default.Properties.GetValue("Token", Guid.NewGuid().ToString());
                if (manager.Sections.TryGetValue("ADs", out HonooSection section))
                {
                    foreach (HonooProperty property in section.Properties)
                    {
                        ModelLocator.Settings.ADs.Add(property.Key, property.GetValue(string.Empty));
                    }
                }
            }
        }

        public static void SaveConfig()
        {
            using (HonooSettingsManager manager = new())
            {
                manager.Default.Properties.AddOrUpdate("WindowTop", ModelLocator.Settings.WindowTop);
                manager.Default.Properties.AddOrUpdate("WindowLeft", ModelLocator.Settings.WindowLeft);
                manager.Default.Properties.AddOrUpdate("Topmost", ModelLocator.Settings.Topmost);
                if (!Directory.Exists(_appConfigDirectory))
                {
                    Directory.CreateDirectory(_appConfigDirectory);
                }
                manager.Save(_appConfigFlie);
            }

            using (HonooSettingsManager manager = new())
            {
                manager.Default.Properties.AddOrUpdate("WindowTop", ModelLocator.Settings.WindowTop);
                manager.Default.Properties.AddOrUpdate("WindowLeft", ModelLocator.Settings.WindowLeft);
                manager.Default.Properties.AddOrUpdate("Topmost", ModelLocator.Settings.Topmost);
                manager.Default.Properties.AddOrUpdate("SettingExpanded", ModelLocator.Settings.SettingExpanded);
                manager.Default.Properties.AddOrUpdate("WorkDirectly", ModelLocator.Settings.WorkDirectly);
                manager.Default.Properties.AddOrUpdate("ExecuteAtDrop", ModelLocator.Settings.ExecuteAtDrop);
                manager.Default.Properties.AddOrUpdate("ResetName", ModelLocator.Settings.ResetName);
                manager.Default.Properties.AddOrUpdate("MoveToRecycleBin", ModelLocator.Settings.MoveToRecycleBin);
                manager.Default.Properties.AddOrUpdate("UnpacksMoveToPacks", ModelLocator.Settings.UnpacksMoveToPacks);
                manager.Default.Properties.AddOrUpdate("PackUnpacks", ModelLocator.Settings.PackUnpacks);
                manager.Default.Properties.AddOrUpdate("DeleteAD", ModelLocator.Settings.DeleteAD);
                manager.Default.Properties.AddOrUpdate("AddTopTitle", ModelLocator.Settings.AddTopTitle);
                manager.Default.Properties.AddOrUpdate("AddTag", ModelLocator.Settings.AddTag);
                manager.Default.Properties.AddOrUpdate("SelectedTag", ModelLocator.Settings.SelectedTag);
                manager.Default.Properties.AddOrUpdate("Passwords", ModelLocator.Settings.Passwords.ToArray());
                manager.Default.Properties.AddOrUpdate("PasswordRemoveConfirm", ModelLocator.Settings.PasswordRemoveConfirm);
                manager.Default.Properties.AddOrUpdate("Tags", ModelLocator.Settings.Tags.ToArray());
                manager.Default.Properties.AddOrUpdate("TagRemoveConfirm", ModelLocator.Settings.TagRemoveConfirm);
                manager.Save(_configFlie);
            }

            using (HonooSettingsManager manager = File.Exists(_adListFile) ? new(_adListFile) : new())
            {
                bool update = false;
                if (manager.Default.Properties.TryGetValue("Token", out string token))
                {
                    if (token != ModelLocator.Settings.Token)
                    {
                        update = true;
                    }
                }
                else
                {
                    manager.Default.Properties.Add("Token", ModelLocator.Settings.Token);
                    update = true;
                }
                if (update)
                {
                    manager.Sections.Clear();
                    HonooSection sections = manager.Sections.GetOrAdd("ADs");
                    foreach (var ad in ModelLocator.Settings.ADs)
                    {
                        sections.Properties.Add(ad.Key, ad.Value);
                    }
                    manager.Save(_adListFile);
                }
            }
        }
    }
}