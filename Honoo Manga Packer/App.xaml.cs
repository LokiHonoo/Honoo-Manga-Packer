using Honoo.MangaPacker.ViewModels;
using HonooUI.WPF;
using System;
using System.Text;
using System.Windows;

namespace Honoo.MangaPacker
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:考虑将公共类型设为内部类型", Justification = "<挂起>")]
    public partial class App : Application
    {
        #region App

        public App()
        {
            this.Startup += App_Startup;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Settings.Instance.SavePassword();
            Settings.Instance.SaveConfig();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (Honoo.Threading.App.PrevInstance("4Bvmw9BMhj2BQFHb"))
            {
                Environment.Exit(0);
            }
            else
            {
                DialogLocalization.Default = new DialogLocalization("确 定", "取 消");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                try
                {
                    Settings.Instance.LoadPassword();
                    Settings.Instance.LoadConfig();
                }
                catch
                {
                }

                this.Exit += App_Exit;
            }
        }

        #endregion App
    }
}