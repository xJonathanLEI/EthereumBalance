using System;

namespace EthereumBalance.Extensions
{
    public static class LongExtensions
    {
        public static byte[] ToBigEndianBytes(this long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            int leadingZeros = 0;
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] == 0)
                    leadingZeros++;
                else break;

            if (leadingZeros == 0)
                return bytes;

            byte[] minBytes = new byte[bytes.Length - leadingZeros];
            for (int i = 0; i < minBytes.Length; i++)
                minBytes[i] = bytes[i + leadingZeros];
            return minBytes;
        }
    }
}