using System;

namespace FiiiCoin.Wallet.Win.Common.Utils
{
    public class DateTimeUtil
    {
        internal static long GetDateTimeStamp(DateTime dateTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var timeStampDouble = (dateTime - startTime).TotalMilliseconds;
            return Convert.ToInt64(timeStampDouble);
        }
    }
}
