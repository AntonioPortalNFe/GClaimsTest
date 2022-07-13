using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Commom.Extensions
{
    public static class DateExtensions
    {

        /// <summary>
        /// Convert a DateTime value to a UNIX Timestamp
        /// </summary>
        /// <param name="value">date to convert///<returns></returns>
        public static long ConvertDateTimeToTimestamp(this DateTime value)
        {
            TimeSpan epoch = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());            
            return (long)epoch.TotalSeconds;
        }
    }
}
