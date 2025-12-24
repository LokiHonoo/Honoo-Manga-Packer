using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honoo.MangaPacker.ViewModels
{
    public sealed partial class UnpackLogWindowViewModel : ObservableObject
    {
        public UnpackWorkbench UnpackWorkbench { get; } = UnpackWorkbench.Instance;
    }
}
