using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace BasicLoginServer.Helpers
{
    public static class GetCurrentClaimValues
    {
        public static string GetCurrentUserEmployer()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string userEmployer = identity.FindFirstValue("userEmployer");
            return userEmployer;
        }


        public static string GetCurrentUserRole()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string userRole = identity.FindFirstValue("userRole");
            return userRole;
        }


        public static string GetCurrentUserName()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string userId = identity.Name;
            return userId;
        }


        public static string GetCurrentUserUniqueDatabaseId()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string userUniqueDatabaseId = identity.FindFirstValue("userUniqueDatabaseId");
            return userUniqueDatabaseId;
        }


        public static string GetCurrentUserSendgridEmail()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string sendgridEmail = identity.FindFirstValue("sendgridEmail");
            return sendgridEmail;
        }


        public static string GetCurrentUserTimezone()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string timezone = identity.FindFirstValue("timezone");
            return timezone;
        }


        public static string GetCurrentUserCultureInfo()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string cultureInfo = identity.FindFirstValue("cultureInfo");
            return cultureInfo;
        }


        public static string GetCurrentUserSendgridEmailAuthor()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string sendgridEmailAuthor = identity.FindFirstValue("sendgridEmailAuthor");
            return sendgridEmailAuthor;
        }


        public static string GetCurrentUserPhoneNumber()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string phoneNumber = identity.FindFirstValue("phoneNumber");
            return phoneNumber;
        }


        public static string GetCurrentUserDestination()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string destination = identity.FindFirstValue("destination");
            return destination;
        }


        public static string GetCurrentUserDestinationAirportCode()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string destinationAirportCode = identity.FindFirstValue("destinationAirportCode");
            return destinationAirportCode;
        }


        public static string GetCurrentUserDateFormat()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string dateFormat = identity.FindFirstValue("dateFormat");
            return dateFormat;
        }
    }
}