using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Honoo.MangaPacker.Classes;
using Honoo.MangaPacker.Models;
using Honoo.MangaPacker.UserControls;
using HonooUI.WPF;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly Settings _settings = ModelLocator.Settings;
        private Workbench _packWorkbench = new();
        private Workbench _unpackWorkbench = new();

        public MainWindowViewModel()
        {
            this.BrowserWorkDirectlyCommand = new RelayCommand(BrowserWorkDirectlyExecute);
            this.UnpackDropCommand = new RelayCommand<DragEventArgs>(UnpackDropExecute, (e) => { return !this.UnpackWorkbench.IsRunning; });
            this.UnpackCommand = new RelayCommand(UnpackExecute);
            this.UnpackClearCommand = new RelayCommand(UnpackClearExecute, () => { return !this.UnpackWorkbench.IsRunning; });
            this.PackDropCommand = new RelayCommand<DragEventArgs>(PackDropExecute, (e) => { return !this.PackWorkbench.IsRunning; });
            this.PackCommand = new RelayCommand(PackExecute);
            this.PackClearCommand = new RelayCommand(PackClearExecute, () => { return !this.PackWorkbench.IsRunning; });
            this.EditPasswordsCommand = new RelayCommand(EditPasswordsExecute);
            this.EditADsCommand = new RelayCommand(EditADsExecute);
            this.EditTagsCommand = new RelayCommand(EditTagsExecute);
            this.ViewErrorCommand = new RelayCommand(ViewErrorExecute);
        }

        public ICommand BrowserWorkDirectlyCommand { get; set; }
        public ICommand EditADsCommand { get; set; }
        public ICommand EditPasswordsCommand { get; set; }
        public ICommand EditTagsCommand { get; set; }
        public ICommand PackClearCommand { get; set; }
        public ICommand PackCommand { get; set; }
        public ICommand PackDropCommand { get; set; }
        public Workbench PackWorkbench { get => _packWorkbench; set => SetProperty(ref _packWorkbench, value); }
        public Settings Settings => _settings;
        public ICommand UnpackClearCommand { get; set; }
        public ICommand UnpackCommand { get; set; }
        public ICommand UnpackDropCommand { get; set; }
        public Workbench UnpackWorkbench { get => _unpackWorkbench; set => SetProperty(ref _unpackWorkbench, value); }
        public ICommand ViewErrorCommand { get; set; }

        private void BrowserWorkDirectlyExecute()
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

        private void EditADsExecute()
        {
            DialogManager.Default.Show(new ADDialogUserControl(), new ADDialogHeaderUserControl());
        }

        private void EditPasswordsExecute()
        {
            DialogManager.Default.Show(new PasswordDialogUserControl(), "解包密码");
        }

        private void EditTagsExecute()
        {
            DialogManager.Default.Show(new TagDialogUserControl(), "标签");
        }

        private void PackClearExecute()
        {
            this.PackWorkbench.Projects.Clear();
        }

        private void PackDropExecute(DragEventArgs? e)
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

        private void PackExecute()
        {
            if (this.PackWorkbench.IsRunning)
            {
                this.PackWorkbench.Abort = true;
            }
            else
            {
                if (this.PackWorkbench.Projects.Count > 0)
                {
                    this.Settings.ErrorMessages.Clear();
                    this.PackWorkbench.Abort = false;
                    this.PackWorkbench.IsRunning = true;
                    this.PackWorkbench.Log.Clear();
                    this.PackWorkbench.HasError = false;
                    Task.Run(() =>
                    {
                        bool dirOk = true;
                        if (!Directory.Exists(_settings.WorkDirectly))
                        {
                            try
                            {
                                Directory.CreateDirectory(_settings.WorkDirectly);
                            }
                            catch (Exception ex)
                            {
                                Tuple<string, string, bool, Exception?> log = new(_settings.WorkDirectly, string.Empty, false, ex);
                                this.Settings.ErrorMessages.Add("无法创建工作目录。");
                                this.PackWorkbench.Log.Add(log);
                                dirOk = false;
                            }
                        }
                        if (dirOk)
                        {
                            for (int i = this.PackWorkbench.Projects.Count - 1; i >= 0; i--)
                            {
                                if (!this.PackWorkbench.Abort)
                                {
                                    if (!Pack.Do(this.PackWorkbench.Projects[i], _settings, out Tuple<string, string, bool, Exception?> log))
                                    {
                                        this.Settings.ErrorMessages.Add($"{log.Item1} -- {log.Item4?.Message}");
                                        this.PackWorkbench.HasError = true;
                                    }
                                    this.PackWorkbench.Projects.RemoveAt(i);
                                    this.PackWorkbench.Log.Add(log);
                                }
                            }
                        }
                        this.PackWorkbench.IsRunning = false;
                    });
                }
            }
        }

        private void UnpackClearExecute()
        {
            this.UnpackWorkbench.Projects.Clear();
        }

        private void UnpackDropExecute(DragEventArgs? e)
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

        private void UnpackExecute()
        {
            if (this.UnpackWorkbench.IsRunning)
            {
                this.UnpackWorkbench.Abort = true;
            }
            else
            {
                if (this.UnpackWorkbench.Projects.Count > 0)
                {
                    this.Settings.ErrorMessages.Clear();
                    this.UnpackWorkbench.Abort = false;
                    this.UnpackWorkbench.IsRunning = true;
                    this.UnpackWorkbench.Log.Clear();
                    this.UnpackWorkbench.HasError = false;
                    bool move = this.Settings.UnpacksMoveToPacks && !this.PackWorkbench.IsRunning && this.PackWorkbench.Projects.Count == 0;
                    Task.Run(() =>
                    {
                        bool dirOk = true;
                        if (!Directory.Exists(_settings.WorkDirectly))
                        {
                            try
                            {
                                Directory.CreateDirectory(_settings.WorkDirectly);
                            }
                            catch (Exception ex)
                            {
                                Tuple<string, string, bool, Exception?> log = new(_settings.WorkDirectly, string.Empty, false, ex);
                                this.Settings.ErrorMessages.Add("无法创建工作目录。");
                                this.PackWorkbench.Log.Add(log);
                                dirOk = false;
                            }
                        }
                        if (dirOk)
                        {
                            for (int i = this.UnpackWorkbench.Projects.Count - 1; i >= 0; i--)
                            {
                                if (!this.UnpackWorkbench.Abort)
                                {
                                    if (!Unpack.Do(this.UnpackWorkbench.Projects[i], _settings, out Tuple<string, string, bool, Exception?> log))
                                    {
                                        this.Settings.ErrorMessages.Add($"{log.Item1} -- {log.Item4?.Message}");
                                        this.UnpackWorkbench.HasError = true;
                                    }
                                    this.UnpackWorkbench.Projects.RemoveAt(i);
                                    this.UnpackWorkbench.Log.Add(log);
                                    if (move && log.Item3)
                                    {
                                        this.PackWorkbench.Projects.Add(log.Item2);
                                    }
                                }
                            }
                        }
                        this.UnpackWorkbench.IsRunning = false;
                        //
                        if (move && this.Settings.PackUnpacks && this.PackWorkbench.Projects.Count > 0)
                        {
                            PackExecute();
                        }
                    });
                }
            }
        }

        private void ViewErrorExecute()
        {
            DialogManager.Default.Show(new ErrorDialogUserControl(), "错误信息");
        }
    }
}