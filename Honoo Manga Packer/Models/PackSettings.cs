using System.IO;

namespace Honoo.MangaPacker.Models
{
    internal sealed class PackSettings
    {
        internal PackSettings(string workDirectly, bool packClearTarget, bool packRemoveAD, bool packRemoveNest, bool packAddNest, bool packConvertToWebP, bool packDelSource)
        {
            this.PackDir = Path.Combine(workDirectly, "Packs");
            this.PackClearTarget = packClearTarget;
            this.PackRemoveAD = packRemoveAD;
            this.ADBackupDir = Path.Combine(workDirectly, "ADBackup");
            this.PackRemoveNest = packRemoveNest;
            this.PackAddNest = packAddNest;
            this.PackConvertToWebP = packConvertToWebP;
            this.PackDelSource = packDelSource;
        }

        internal string ADBackupDir { get; }
        internal bool PackAddNest { get; }
        internal bool PackClearTarget { get; }
        internal bool PackConvertToWebP { get; }
        internal bool PackDelSource { get; }
        internal string PackDir { get; }
        internal bool PackRemoveAD { get; }
        internal bool PackRemoveNest { get; }
    }
}