using System;

namespace Honoo.MangaPacker.Classes
{
    internal static class XValueHelper
    {
        internal static bool TryParse(string hex, out byte[]? value)
        {
            if (!string.IsNullOrWhiteSpace(hex))
            {
                try
                {
                    byte[] result = new byte[hex.Length / 2];
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                    }
                    value = result;
                    return true;
                }
                catch
                {
                }
            }
            value = default;
            return false;
        }
    }
}