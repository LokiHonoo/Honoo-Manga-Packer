namespace Honoo.MangaPacker.Models
{
    internal sealed class UnpackResult
    {
        public UnpackResult(bool success, string info, string output)
        {
            this.Success = success;
            this.Info = info;
            this.Output = output;
        }

        internal string Info { get; }
        internal string Output { get; }
        internal bool Success { get; }
    }
}