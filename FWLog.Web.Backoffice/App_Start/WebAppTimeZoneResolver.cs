using DartDigital.Library.Web.Globalization;
using System;

namespace FWLog.Web.Backoffice.App_Start
{
    public class WebAppTimeZoneResolver : IWebAppTimeZoneResolver
    {
        private TimeZoneInfo _standardTimeZoneInfo;

        public WebAppTimeZoneResolver(TimeZoneInfo standardTimeZone)
        {
            _standardTimeZoneInfo = standardTimeZone;
        }

        public DateTime ConvertSessionTimeToUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, _standardTimeZoneInfo);
        }

        public DateTime ConvertUtcToSessionTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, _standardTimeZoneInfo);
        }

        public TimeZoneInfo GetStandardTimeZoneInfo()
        {
            return _standardTimeZoneInfo;
        }
    }
}