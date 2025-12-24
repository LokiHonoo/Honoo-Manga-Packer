namespace Honoo.MangaPacker.Models
{
    internal sealed class PackResult
    {
        public PackResult(bool success, string info)
        {
            this.Success = success;
            this.Info = info;
        }

        internal string Info { get; }
        internal bool Success { get; }
    }
}