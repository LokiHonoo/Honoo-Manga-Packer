using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Honoo.IO.Hashing;
using Honoo.MangaPacker.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed class ADDialogUserControlViewModel : ObservableObject
    {
        private readonly string _adDirectly = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ads");
        private readonly Crc32 _crc = new();
        private readonly Settings _settings = ModelLocator.Settings;
        private string _adFile = string.Empty;
        private ImageSource? _adImage;
        private string _checksum = string.Empty;

        public ADDialogUserControlViewModel()
        {
            this.OpenFileDialogCommand = new RelayCommand(OpenFileDialogExecute);
            this.AddADCommand = new RelayCommand(AddADExecute, () => { return !string.IsNullOrWhiteSpace(this.Checksum); });
        }

        public ICommand AddADCommand { get; set; }

        public string ADFile { get => _adFile; set => SetProperty(ref _adFile, value); }

        public ImageSource? ADImage { get => _adImage; set => SetProperty(ref _adImage, value); }

        public string Checksum
        {
            get => _checksum;
            set
            {
                SetProperty(ref _checksum, value);
                ((IRelayCommand)this.AddADCommand).NotifyCanExecuteChanged();
            }
        }

        public ICommand OpenFileDialogCommand { get; set; }

        public Settings Settings => _settings;

        private void AddADExecute()
        {
            this.Settings.ADs.Remove(this.Checksum);
            string ext = Path.GetExtension(this.ADFile);
            if (!Directory.Exists(_adDirectly))
            {
                Directory.CreateDirectory(_adDirectly);
            }
            File.Copy(this.ADFile, Path.Combine(_adDirectly, this.Checksum + ext), true);
            this.Settings.Token = Guid.NewGuid().ToString();
            this.Settings.ADs.Add(this.Checksum, ext);
            this.ADFile = string.Empty;
            this.ADImage = null;
            this.Checksum = string.Empty;
        }

        private void OpenFileDialogExecute()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "AD files|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                this.ADFile = dialog.FileName;
                using (FileStream fs = new(ADFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int b;
                    while (true)
                    {
                        b = fs.ReadByte();
                        if (b >= 0)
                        {
                            _crc.Update((byte)b);
                        }
                        else
                        {
                            break;
                        }
                    }
                    this.Checksum = _crc.ComputeFinal(CrcStringFormat.Hex).ToUpperInvariant();
                }
                try
                {
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    img.EndInit();
                    this.ADImage = img;
                }
                catch
                {
                    this.ADImage = null;
                }
            }
        }
    }
}