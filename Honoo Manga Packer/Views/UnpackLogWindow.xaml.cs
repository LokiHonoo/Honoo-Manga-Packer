using System.Windows;

namespace Honoo.MangaPacker.Views
{
    /// <summary>
    /// UnpackLogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UnpackLogWindow : Window
    {
        #region Instance

        public static UnpackLogWindow Instance { get; } = new UnpackLogWindow();

        #endregion Instance

        public UnpackLogWindow()
        {
            InitializeComponent();
        }
    }
}