using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Honoo.MangaPacker.Models;
using Honoo.MangaPacker.Views;
using HonooUI.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed partial class MainWindowViewModel : ObservableObject
    {
        #region Members

        public ICommand BrowserWorkDirectlyCommand { get;  }
        public ICommand EditPasswordsCommand { get;  }
        public ICommand PackClearCommand { get;  }
        public ICommand PackCommand { get;  }
        public ICommand PackDropCommand { get;  }
        public PackWorkbench PackWorkbench { get; } = PackWorkbench.Instance;
        public Settings Settings { get; } = Settings.Instance;
        public ICommand UnpackClearCommand { get;  }
        public ICommand UnpackCommand { get;  }
        public ICommand UnpackDropCommand { get;  }
        public UnpackWorkbench UnpackWorkbench { get; } = UnpackWorkbench.Instance;
        public string? Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        public ICommand ViewPackLogCommand { get;  }
        public ICommand ViewUnpackLogCommand { get;  }

        #endregion Members

        public MainWindowViewModel()
        {
            this.UnpackDropCommand = new RelayCommand<DragEventArgs>(UnpackDropCommandExecute, (e) => { return !this.UnpackWorkbench.IsRunning; });
            this.UnpackClearCommand = new RelayCommand(UnpackClearCommandExecute, () => { return !this.UnpackWorkbench.IsRunning; });
            this.UnpackCommand = new RelayCommand(UnpackCommandExecute);
            this.ViewUnpackLogCommand = new RelayCommand(ViewUnpackLogCommandExecute);
            this.EditPasswordsCommand = new RelayCommand(EditPasswordsCommandExecute);
            this.PackDropCommand = new RelayCommand<DragEventArgs>(PackDropCommandExecute, (e) => { return !this.PackWorkbench.IsRunning; });
            this.PackClearCommand = new RelayCommand(PackClearCommandExecute, () => { return !this.PackWorkbench.IsRunning; });
            this.PackCommand = new RelayCommand(PackCommandExecute);
            this.ViewPackLogCommand = new RelayCommand(ViewPackLogCommandExecute);
            this.BrowserWorkDirectlyCommand = new RelayCommand(BrowserWorkDirectlyCommandExecute);
        }

        private void ViewPackLogCommandExecute()
        {
            throw new NotImplementedException();
        }

        private void ViewUnpackLogCommandExecute()
        {
            UnpackLogWindow.Instance.Show();
        }

        private void BrowserWorkDirectlyCommandExecute()
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

        private void EditPasswordsCommandExecute()
        {
            var stackPanel = new StackPanel();
            var textBlock = new TextBlock() { Text = "每行一个密码：" };
            var textBox = new TextBox
            {
                Width = 280,
                Height = 280,
                Margin = new Thickness(0, 10, 0, 0),
                TextWrapping = TextWrapping.NoWrap,
                AcceptsReturn = true,
                Text = string.Join(Environment.NewLine, this.Settings.UnpackPasswords)
            };
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            DialogManager.Default!.Show(stackPanel,
                           "解包密码",
                           DialogButtons.All,
                           DialogDefaultButton.TrueButton,
                           DialogImage.None,
                           DialogSize.Default,
                           DialogLocalization.Default,
                           null,
                           (e) =>
                           {
                               if (e.DialogResult == true)
                               {
                                   this.Settings.UnpackPasswords = [.. textBox.Text.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
                               }
                           },
                           null);
        }

        private void PackClearCommandExecute()
        {
            this.PackWorkbench.Projects.Clear();
        }

        private void PackCommandExecute()
        {
            if (this.PackWorkbench.IsRunning)
            {
                this.PackWorkbench.Abort = true;
            }
            else
            {
                this.PackWorkbench.Abort = false;
                this.PackWorkbench.Logs.Clear();
                this.PackWorkbench.HasError = false;
                if (this.PackWorkbench.Projects.Count > 0)
                {
                    var settings = new PackSettings(this.Settings.WorkDirectly,
                                                    this.Settings.PackClearTarget,
                                                    this.Settings.PackRemoveAD,
                                                    this.Settings.PackRemoveNest,
                                                    this.Settings.PackAddNest,
                                                    this.Settings.UnpackDelSource);
                    if (settings.PackClearTarget && Directory.Exists(settings.PackDir))
                    {
                        try
                        {
                            foreach (var di in Directory.GetDirectories(settings.PackDir))
                            {
                                Directory.Delete(di, true);
                            }
                            foreach (var fi in Directory.GetFiles(settings.PackDir))
                            {
                                File.Delete(fi);
                            }
                        }
                        catch
                        {
                            this.PackWorkbench.Logs.Add("无法删除旧目录和文件。 \"" + settings.PackDir + "\"。");
                            this.PackWorkbench.HasError = true;
                        }
                    }
                    if (!Directory.Exists(settings.PackDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(settings.PackDir);
                        }
                        catch
                        {
                            this.PackWorkbench.Logs.Add("无法创建工作目录。 \"" + settings.PackDir + "\"。");
                            this.PackWorkbench.HasError = true;
                        }
                    }
                    if (!this.PackWorkbench.HasError)
                    {
                        this.PackWorkbench.IsRunning = true;
                        Task.Run(() =>
                        {
                            for (int i = this.PackWorkbench.Projects.Count - 1; i >= 0; i--)
                            {
                                if (!this.PackWorkbench.Abort)
                                {
                                    PackResult result = Pack.Do(this.PackWorkbench.Projects[i], settings);
                                    Application.Current.Dispatcher.Invoke(new Action(() =>
                                    {
                                        this.PackWorkbench.Logs.Add(result.Info);
                                    }));
                                    if (!result.Success)
                                    {
                                        this.PackWorkbench.HasError = true;
                                    }
                                    this.PackWorkbench.Projects.RemoveAt(i);
                                }
                            }
                            this.PackWorkbench.IsRunning = false;
                        });
                    }
                }
            }
        }

        private void PackDropCommandExecute(DragEventArgs? e)
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
                            foreach (var entry2 in this.PackWorkbench.Projects)
                            {
                                if (entry2 == entry)
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
                    }
                }
            }
        }

        private void UnpackClearCommandExecute()
        {
            this.UnpackWorkbench.Projects.Clear();
        }

        private void UnpackCommandExecute()
        {
            if (this.UnpackWorkbench.IsRunning)
            {
                this.UnpackWorkbench.Abort = true;
            }
            else
            {
                this.UnpackWorkbench.Abort = false;
                this.UnpackWorkbench.Logs.Clear();
                this.UnpackWorkbench.HasError = false;
                if (this.UnpackWorkbench.Projects.Count > 0)
                {
                    var settings = new UnpackSettings(this.Settings.WorkDirectly,
                                                      this.Settings.UnpackClearTarget,
                                                      this.Settings.UnpackEncoding,
                                                      this.Settings.UnpackTryPassword,
                                                      this.Settings.UnpackPasswords,
                                                      this.Settings.UnpackSendToPack,
                                                      this.Settings.UnpackDelSource);
                    if (settings.UnpackClearTarget && Directory.Exists(settings.UnpackDir))
                    {
                        try
                        {
                            foreach (var di in Directory.GetDirectories(settings.UnpackDir))
                            {
                                Directory.Delete(di, true);
                            }
                            foreach (var fi in Directory.GetFiles(settings.UnpackDir))
                            {
                                File.Delete(fi);
                            }
                        }
                        catch
                        {
                            this.UnpackWorkbench.Logs.Add("无法删除旧目录和文件。 \"" + settings.UnpackDir + "\"。");
                            this.UnpackWorkbench.HasError = true;
                        }
                    }
                    if (!Directory.Exists(settings.UnpackDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(settings.UnpackDir);
                        }
                        catch
                        {
                            this.UnpackWorkbench.Logs.Add("无法创建工作目录。 \"" + settings.UnpackDir + "\"。");
                            this.UnpackWorkbench.HasError = true;
                        }
                    }
                    if (!this.UnpackWorkbench.HasError)
                    {
                        this.UnpackWorkbench.IsRunning = true;
                        Task.Run(() =>
                        {
                            for (int i = this.UnpackWorkbench.Projects.Count - 1; i >= 0; i--)
                            {
                                if (!this.UnpackWorkbench.Abort)
                                {
                                    UnpackResult result = Unpack.Do(this.UnpackWorkbench.Projects[i], settings);
                                    if (result.Success)
                                    {
                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                        {
                                            this.UnpackWorkbench.Logs.Add(result.Info);
                                            if (settings.UnpackSendToPack && !this.PackWorkbench.IsRunning)
                                            {
                                                this.PackWorkbench.Projects.Add(result.Output);
                                            }
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                        {
                                            this.UnpackWorkbench.Logs.Add(result.Info);
                                        }));
                                        this.UnpackWorkbench.HasError = true;
                                    }
                                    this.UnpackWorkbench.Projects.RemoveAt(i);
                                }
                            }
                            this.UnpackWorkbench.IsRunning = false;
                        });
                    }
                }
            }
        }

        private void UnpackDropCommandExecute(DragEventArgs? e)
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
                            foreach (var project in this.UnpackWorkbench.Projects)
                            {
                                if (project == entry)
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
                    }
                }
            }
        }
    }
}