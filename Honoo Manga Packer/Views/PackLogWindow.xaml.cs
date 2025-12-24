using System.Windows;

namespace Honoo.MangaPacker.Views
{
    /// <summary>
    /// PackLogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PackLogWindow : Window
    {
        #region Instance

        public static PackLogWindow Instance { get; } = new PackLogWindow();

        #endregion Instance

        public PackLogWindow()
        {
            InitializeComponent();
        }
    }
}