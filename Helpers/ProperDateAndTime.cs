using BasicLoginServer.Managers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace BasicLoginServer.Helpers
{
    public class ProperDateAndTime
    {
        public static string GetNeededTimezone(string connectionString)
        {
            string timezone = String.Empty;

            string sqlGetTimezone = "SELECT timezone FROM tblCommonData";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmdToGetTimezone = new SqlCommand(sqlGetTimezone, conn);
                timezone = cmdToGetTimezone.ExecuteScalar().ToString();
            }
            return timezone;
        }


        public static string ReturnFormattedDateForDatabase(string givenDate, bool mobile, string hotelName = "")
        {
            string dateFormat = ReturnUserDateFormat(mobile, hotelName);

            return DateTime.ParseExact(givenDate, dateFormat, CultureInfo.InvariantCulture).
                ToString("yyyy-MM-dd");
        }


        public static string ReturnUserDateFormat(bool mobile, string hotelName = "")
        {
            string dateFormat = string.Empty;
            if (mobile == false)
            {
                dateFormat = GetCurrentClaimValues.GetCurrentUserDateFormat();
            }
            else
            {
                string connectionString = CommonManager.ReturnNeededConnectionStringForHotelWithNameSent(hotelName);

                string sqlGetDateFormat = "SELECT dateFormat FROM tblCommonData";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmdToGetDateFormat = new SqlCommand(sqlGetDateFormat, conn);
                    dateFormat = cmdToGetDateFormat.ExecuteScalar().ToString();
                }
            }
            return dateFormat;
        }


        public static string GetNeededCultureInfo(string connectionString)
        {
            string currentCulture = String.Empty;
            string sqlGetCurrentCulture = "SELECT cultureInfo FROM tblCommonData";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmdToGetCurrentCulture = new SqlCommand(sqlGetCurrentCulture, conn);
                currentCulture = cmdToGetCurrentCulture.ExecuteScalar().ToString();
            }
            return currentCulture;
        }


        public static DateTime GetCustomerTimeZones(string connectionString, bool mobile)
        {
            if (mobile == false)
            {
                string _TimeValue = GetCurrentClaimValues.GetCurrentUserTimezone();
                Thread.CurrentThread.CurrentCulture = new CultureInfo(GetCurrentClaimValues.GetCurrentUserCultureInfo());
                DateTime _Now = DateTime.Now;
                TimeZoneInfo _TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_TimeValue);
                return TimeZoneInfo.ConvertTime(_Now, _TimeZone);
            }
            else
            {
                string _TimeValue = GetNeededTimezone(connectionString);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(GetNeededCultureInfo(connectionString));
                DateTime _Now = DateTime.Now;
                TimeZoneInfo _TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_TimeValue);
                return TimeZoneInfo.ConvertTime(_Now, _TimeZone);
            }
        }


        public static string ReturnCorrectCurrentDate(string connectionString, bool mobile)
        {
            string currentDateTime = GetCustomerTimeZones(connectionString, mobile).ToString();
            string[] splitCurrentDateTime = currentDateTime.Split(' ');
            string currentDate = splitCurrentDateTime[0];

            return currentDate;
        }


        public static string ReturnTomorrowDate()
        {
            string _TimeValue = GetCurrentClaimValues.GetCurrentUserTimezone();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(GetCurrentClaimValues.GetCurrentUserCultureInfo());
            DateTime _Now = DateTime.Now.AddDays(1);
            TimeZoneInfo _TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_TimeValue);
            string tomorrowDateTime = TimeZoneInfo.ConvertTime(_Now, _TimeZone).ToString();
            string[] splitTomorrowDateTime = tomorrowDateTime.Split(' ');
            string tomorrowDate = splitTomorrowDateTime[0];

            return tomorrowDate;
        }


        public static string ReturnCorrectCurrentTime(string connectionString, bool mobile)
        {

            string currentDateTime = GetCustomerTimeZones(connectionString, mobile).ToString();
            string[] splitCurrentDateTime = currentDateTime.Split(' ');
            string currentTime = splitCurrentDateTime[1] + " " + splitCurrentDateTime[2];

            return currentTime;
        }


        public static string ReturnFormattedDateForCalendarFutureDates(string givenDate)
        {
            //string _TimeValue = GetCurrentClaimValues.GetCurrentUserTimezone();
            //Thread.CurrentThread.CurrentCulture = new CultureInfo(GetCurrentClaimValues.GetCurrentUserCultureInfo());
            //DateTime _givenDate;
            //DateTime.TryParse(givenDate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out _givenDate);
            //TimeZoneInfo _TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_TimeValue);
            //string convertedDateTime = TimeZoneInfo.ConvertTime(_givenDate, _TimeZone).
            //    ToString(ReturnUserDateFormat(mobile));
            string _givenDate = DateTime.Parse(givenDate).ToString(GetCurrentClaimValues.GetCurrentUserDateFormat());
            //string[] splitConvertedDateTime = givenDate.Split(' ');
            //string convertedDate = splitConvertedDateTime[0];


            return _givenDate;
        }
    }
}