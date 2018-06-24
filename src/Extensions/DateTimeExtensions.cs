using System;

namespace EthereumBalance.Extensions
{
    public static class DatetimeExtensions
    {
        public static int ToUnixTime(this DateTime dt)
        {
            return (int)(dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public static long ToUnixMiliseconds(this DateTime dt)
        {
            return (long)(dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }
    }
}