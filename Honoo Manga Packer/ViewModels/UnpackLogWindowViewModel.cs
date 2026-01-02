using CommunityToolkit.Mvvm.ComponentModel;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed partial class UnpackLogWindowViewModel : ObservableObject
    {
        public UnpackWorkbench UnpackWorkbench { get; } = UnpackWorkbench.Instance;
    }
}