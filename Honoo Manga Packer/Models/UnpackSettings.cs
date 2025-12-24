using System.Collections.Generic;
using System.IO;

namespace Honoo.MangaPacker.Models
{
    internal sealed class UnpackSettings
    {
        internal UnpackSettings(string workDirectly, bool unpackClearTarget, string unpackEncoding, bool unpackTryPassword, HashSet<string> unpackPasswords, bool unpackSendToPack, bool unpackDelSource)
        {
            this.UnpackDir = Path.Combine(workDirectly, "Unpacks");
            this.UnpackClearTarget = unpackClearTarget;
            this.UnpackEncoding = unpackEncoding;
            this.UnpackTryPassword = unpackTryPassword;
            this.UnpackPasswords = [.. unpackPasswords];
            this.UnpackSendToPack = unpackSendToPack;
            this.UnpackDelSource = unpackDelSource;
        }

        internal bool UnpackClearTarget { get; }
        internal bool UnpackDelSource { get; }
        internal string UnpackDir { get; }
        internal string UnpackEncoding { get; }
        internal HashSet<string> UnpackPasswords { get; }
        internal bool UnpackSendToPack { get; }
        internal bool UnpackTryPassword { get; }
    }
}