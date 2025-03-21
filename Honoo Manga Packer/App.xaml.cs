using Honoo.MangaPacker.Models;
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
            Settings.SavePassword();
            Settings.SaveConfig();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (Honoo.Threading.App.PrevInstance("4Bvmw9BMhj2BQFHb"))
            {
                Current.Shutdown();
            }
            else
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                try
                {
                    Settings.LoadPassword();
                    Settings.LoadConfig();
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