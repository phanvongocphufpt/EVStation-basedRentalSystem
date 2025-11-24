using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common
{
    /// <summary>
    /// Helper class để xử lý thời gian theo múi giờ Vietnam (GMT+7)
    /// </summary>
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        /// <summary>
        /// Lấy thời gian hiện tại theo múi giờ Vietnam (GMT+7)
        /// </summary>
        public static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }

        /// <summary>
        /// Convert DateTime từ UTC sang Vietnam timezone
        /// </summary>
        public static DateTime ConvertToVietnamTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                // Nếu là Unspecified, giả định là UTC
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), VietnamTimeZone);
            }
            if (utcDateTime.Kind == DateTimeKind.Utc)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
            }
            // Nếu đã là Local, convert sang UTC rồi sang Vietnam
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.ToUniversalTime(), VietnamTimeZone);
        }

        /// <summary>
        /// Convert DateTime từ Vietnam timezone sang UTC
        /// </summary>
        public static DateTime ConvertToUtc(DateTime vietnamDateTime)
        {
            if (vietnamDateTime.Kind == DateTimeKind.Unspecified)
            {
                // Giả định là Vietnam time
                return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(vietnamDateTime, DateTimeKind.Unspecified), VietnamTimeZone);
            }
            return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, VietnamTimeZone);
        }

        /// <summary>
        /// Lấy thời gian hiện tại theo Vietnam timezone, dạng nullable
        /// </summary>
        public static DateTime? GetVietnamTimeNullable()
        {
            return GetVietnamTime();
        }
    }
}

