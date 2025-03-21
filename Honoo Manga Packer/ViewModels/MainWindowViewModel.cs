using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Honoo.MangaPacker.Models;
using Honoo.MangaPacker.UserControls;
using HonooUI.WPF;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Honoo.MangaPacker.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:考虑将公共类型设为内部类型", Justification = "<挂起>")]
    public sealed class MainWindowViewModel : ObservableObject
    {
        private Workbench _packWorkbench = new();
        private Workbench _unpackWorkbench = new();

        public MainWindowViewModel()
        {
            this.BrowserWorkDirectlyCommand = new RelayCommand(BrowserWorkDirectly);
            this.UnpackDropCommand = new RelayCommand<DragEventArgs>(UnpackDrop, (e) => { return !this.UnpackWorkbench.IsRunning; });
            this.UnpackCommand = new RelayCommand(UnpackIt);
            this.UnpackClearCommand = new RelayCommand(UnpackClear, () => { return !this.UnpackWorkbench.IsRunning; });
            this.PackDropCommand = new RelayCommand<DragEventArgs>(PackDrop, (e) => { return !this.PackWorkbench.IsRunning; });
            this.PackCommand = new RelayCommand(PackIt);
            this.PackClearCommand = new RelayCommand(PackClear, () => { return !this.PackWorkbench.IsRunning; });
            this.EditPasswordsCommand = new RelayCommand(EditPasswords);
            this.EditTagsCommand = new RelayCommand(EditTags);
            this.ViewUnpackErrorCommand = new RelayCommand(ViewUnpackError);
            this.ViewPackErrorCommand = new RelayCommand(ViewPackError);
        }

        public ICommand BrowserWorkDirectlyCommand { get; set; }
        public ICommand EditPasswordsCommand { get; set; }
        public ICommand EditTagsCommand { get; set; }
        public ICommand PackClearCommand { get; set; }
        public ICommand PackCommand { get; set; }
        public ICommand PackDropCommand { get; set; }
        public ObservableCollection<string> PackErrorMessages { get; } = [];
        public Workbench PackWorkbench { get => _packWorkbench; set => SetProperty(ref _packWorkbench, value); }
        public Settings Settings => ModelLocator.Settings;
        public ICommand UnpackClearCommand { get; set; }
        public ICommand UnpackCommand { get; set; }
        public ICommand UnpackDropCommand { get; set; }
        public ObservableCollection<string> UnpackErrorMessages { get; } = [];
        public Workbench UnpackWorkbench { get => _unpackWorkbench; set => SetProperty(ref _unpackWorkbench, value); }
        public ICommand ViewPackErrorCommand { get; set; }
        public ICommand ViewUnpackErrorCommand { get; set; }

        private void BrowserWorkDirectly()
        {
            OpenFolderDialog dialog = new()
            {
                InitialDirectory = this.Settings.WorkDirectly
            };
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.Settings.WorkDirectly = dialog.FolderName;
            }
        }

        private void EditPasswords()
        {
            DialogManager.Default.Show(new PasswordDialogUserControl(), "解包密码");
        }

        private void EditTags()
        {
            DialogManager.Default.Show(new TagDialogUserControl(), "标签");
        }

        private void PackClear()
        {
            this.PackWorkbench.Projects.Clear();
        }

        private void PackDrop(DragEventArgs? e)
        {
            if (e != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] entries = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (entries.Length > 0)
                    {
                        foreach (var entry in entries)
                        {
                            bool exists = false;
                            foreach (var project in this.PackWorkbench.Projects)
                            {
                                if (project == entry)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists)
                            {
                                this.PackWorkbench.Projects.Add(entry);
                            }
                        }
                        if (this.Settings.ExecuteAtDrop && this.PackWorkbench.Projects.Count > 0)
                        {
                            this.PackCommand.Execute(null);
                        }
                    }
                }
            }
        }

        private void PackIt()
        {
            if (this.PackWorkbench.IsRunning)
            {
                this.PackWorkbench.Abort = true;
            }
            else
            {
                if (this.PackWorkbench.Projects.Count > 0)
                {
                    this.PackErrorMessages.Clear();
                    bool dirOk = true;
                    var dir = new DirectoryInfo(Path.Combine(Path.Combine(this.Settings.WorkDirectly, "Packs")));
                    if (this.Settings.PackClearWorkDirectly)
                    {
                        if (dir.Exists)
                        {
                            try
                            {
                                foreach (var di in dir.GetDirectories())
                                {
                                    di.Delete(true);
                                }
                                foreach (var fi in dir.GetFiles())
                                {
                                    fi.Delete();
                                }
                            }
                            catch (Exception ex)
                            {
                                Tuple<bool, string, Exception?> log = new(false, dir.FullName, ex);
                                this.PackErrorMessages.Add("无法删除旧目录和文件。");
                                this.PackWorkbench.Log.Add(log);
                                dirOk = false;
                            }
                        }
                    }
                    if (dirOk && !dir.Exists)
                    {
                        try
                        {
                            dir.Create();
                        }
                        catch (Exception ex)
                        {
                            Tuple<bool, string, Exception?> log = new(false, dir.FullName, ex);
                            this.PackErrorMessages.Add("无法创建工作目录。");
                            this.PackWorkbench.Log.Add(log);
                            dirOk = false;
                        }
                    }
                    if (dirOk)
                    {
                        this.PackWorkbench.Abort = false;
                        this.PackWorkbench.IsRunning = true;
                        this.PackWorkbench.Log.Clear();
                        this.PackWorkbench.HasError = false;
                        Task.Run(() =>
                        {
                            for (int i = this.PackWorkbench.Projects.Count - 1; i >= 0; i--)
                            {
                                if (!this.PackWorkbench.Abort)
                                {
                                    if (!Pack.Do(this.PackWorkbench.Projects[i], this.Settings, out Tuple<bool, string, Exception?> log))
                                    {
                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                        {
                                            this.PackErrorMessages.Add($"{log.Item3?.Message} -- {log.Item2}");
                                        }));
                                        this.PackWorkbench.HasError = true;
                                    }
                                    this.PackWorkbench.Projects.RemoveAt(i);
                                    this.PackWorkbench.Log.Add(log);
                                }
                            }

                            this.PackWorkbench.IsRunning = false;
                        });
                    }
                }
            }
        }

        private void UnpackClear()
        {
            this.UnpackWorkbench.Projects.Clear();
        }

        private void UnpackDrop(DragEventArgs? e)
        {
            if (e != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] entries = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (entries.Length > 0)
                    {
                        foreach (var entry in entries)
                        {
                            bool exists = false;
                            foreach (var entry2 in this.UnpackWorkbench.Projects)
                            {
                                if (entry2 == entry)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists)
                            {
                                this.UnpackWorkbench.Projects.Add(entry);
                            }
                        }
                        if (this.Settings.ExecuteAtDrop && this.UnpackWorkbench.Projects.Count > 0)
                        {
                            this.UnpackCommand.Execute(null);
                        }
                    }
                }
            }
        }

        private void UnpackIt()
        {
            if (this.UnpackWorkbench.IsRunning)
            {
                this.UnpackWorkbench.Abort = true;
            }
            else
            {
                if (this.UnpackWorkbench.Projects.Count > 0)
                {
                    this.UnpackErrorMessages.Clear();
                    bool dirOk = true;
                    var dir = new DirectoryInfo(Path.Combine(Path.Combine(this.Settings.WorkDirectly, "Unpacks")));
                    if (this.Settings.UnpackClearWorkDirectly)
                    {
                        if (dir.Exists)
                        {
                            try
                            {
                                foreach (var di in dir.GetDirectories())
                                {
                                    di.Delete(true);
                                }
                                foreach (var fi in dir.GetFiles())
                                {
                                    fi.Delete();
                                }
                            }
                            catch (Exception ex)
                            {
                                Tuple<bool, string, Exception?> log = new(false, dir.FullName, ex);
                                this.UnpackErrorMessages.Add("无法删除旧目录和文件。");
                                this.PackWorkbench.Log.Add(log);
                                dirOk = false;
                            }
                        }
                    }
                    if (dirOk && !dir.Exists)
                    {
                        try
                        {
                            dir.Create();
                        }
                        catch (Exception ex)
                        {
                            Tuple<bool, string, Exception?> log = new(false, dir.FullName, ex);
                            this.UnpackErrorMessages.Add("无法创建工作目录。");
                            this.PackWorkbench.Log.Add(log);
                            dirOk = false;
                        }
                    }
                    if (dirOk)
                    {
                        this.UnpackWorkbench.Abort = false;
                        this.UnpackWorkbench.IsRunning = true;
                        this.UnpackWorkbench.Log.Clear();
                        this.UnpackWorkbench.HasError = false;
                        bool move = this.Settings.UnpacksMoveToPacks && !this.PackWorkbench.IsRunning && this.PackWorkbench.Projects.Count == 0;
                        Task.Run(() =>
                        {
                            for (int i = this.UnpackWorkbench.Projects.Count - 1; i >= 0; i--)
                            {
                                if (!this.UnpackWorkbench.Abort)
                                {
                                    if (!Models.Unpack.Do(this.UnpackWorkbench.Projects[i], this.Settings, out Tuple<bool, string, Exception?> log))
                                    {
                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                        {
                                            this.UnpackErrorMessages.Add($"{log.Item3?.Message} -- {log.Item2}");
                                        }));
                                        this.UnpackWorkbench.HasError = true;
                                    }
                                    this.UnpackWorkbench.Projects.RemoveAt(i);
                                    this.UnpackWorkbench.Log.Add(log);
                                    if (move && log.Item1)
                                    {
                                        this.PackWorkbench.Projects.Add(log.Item2);
                                    }
                                }
                            }
                            this.UnpackWorkbench.IsRunning = false;
                            //
                            if (move && this.Settings.PackUnpacks && !this.UnpackWorkbench.HasError && this.PackWorkbench.Projects.Count > 0)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    PackIt();
                                }));
                            }
                        });
                    }
                }
            }
        }

        private void ViewPackError()
        {
            var listView = new ListView() { Width = 400, Height = 380, ItemsSource = this.PackErrorMessages };
            DialogManager.Default.Show(listView, "错误信息");
        }

        private void ViewUnpackError()
        {
            var listView = new ListView() { Width = 400, Height = 380, ItemsSource = this.UnpackErrorMessages };
            DialogManager.Default.Show(listView, "错误信息");
        }
    }
}