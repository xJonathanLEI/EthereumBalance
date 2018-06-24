using System;
using System.Numerics;

namespace EthereumBalance.Extensions
{
    public static class BigIntegerExtensions
    {
        public static string ToDecString(this BigInteger bigInt, int decimals = 0, int precision = 0)
        {
            string ret = bigInt.ToString();
            if (decimals == 0)
            {
                if (precision == 0)
                    return ret;
                else
                {
                    string trailingZeros = "";
                    while (trailingZeros.Length < precision)
                        trailingZeros += "0";
                    return ret + "." + trailingZeros;
                }
            }
            while (ret.Length < decimals)
                ret = "0" + ret;
            if (ret.Length == decimals)
                ret = "0." + ret;
            else
                ret = ret.Substring(0, ret.Length - decimals) + "." + ret.Substring(ret.Length - decimals);
            if (precision == 0)
                return ret.Remove(ret.IndexOf("."));
            else if (ret.Length > ret.IndexOf(".") + precision + 1)
                return ret.Remove(ret.IndexOf(".") + precision + 1);
            else
                return ret;
        }

        public static byte[] ToBigEndianBytes(this BigInteger bigInt, int length)
        {
            byte[] bytes = bigInt.ToByteArray();
            if (bytes.Length > length) throw new FormatException();
            Array.Reverse(bytes);
            if (bytes.Length == length) return bytes;
            byte[] ret = new byte[length];
            Array.Copy(bytes, 0, ret, length - bytes.Length, bytes.Length);
            return ret;
        }

        public static byte[] ToMinimumBigEndianBytes(this BigInteger bigInt)
        {
            byte[] bytes = bigInt.ToByteArray();
            Array.Reverse(bytes);
            int firstNonZeroIndex = -1;
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] != 0)
                {
                    firstNonZeroIndex = i;
                    break;
                }
            if (firstNonZeroIndex == -1) return new byte[0];
            byte[] ret = new byte[bytes.Length - firstNonZeroIndex];
            Array.Copy(bytes, firstNonZeroIndex, ret, 0, ret.Length);
            return ret;
        }
    }
}