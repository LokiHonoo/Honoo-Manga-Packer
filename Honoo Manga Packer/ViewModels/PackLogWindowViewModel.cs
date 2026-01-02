using CommunityToolkit.Mvvm.ComponentModel;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed partial class PackLogWindowViewModel : ObservableObject
    {
        public PackWorkbench PackWorkbench { get; } = PackWorkbench.Instance;
    }
}