using System.IO;

namespace Honoo.MangaPacker.Models
{
    internal sealed class PackSettings
    {
        internal PackSettings(string workDirectly, bool packClearTarget, bool packRemoveAD, bool packRemoveNest, bool packAddNest, bool packDelSource)
        {
            this.PackDir = Path.Combine(workDirectly, "Packs");
            this.PackClearTarget = packClearTarget;
            this.PackRemoveAD = packRemoveAD;
            this.ADBackupDir = new DirectoryInfo(Path.Combine(workDirectly, "ADBackup"));
            this.PackRemoveNest = packRemoveNest;
            this.PackAddNest = packAddNest;
            this.PackDelSource = packDelSource;
        }

        internal DirectoryInfo ADBackupDir { get; }

        internal bool PackAddNest { get; }
        internal bool PackClearTarget { get; }
        internal bool PackDelSource { get; }
        internal string PackDir { get; }
        internal bool PackRemoveAD { get; }
        internal bool PackRemoveNest { get; }
    }
}