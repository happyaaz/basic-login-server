using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using System.Net.Mail;
using BasicLoginServer.Managers;
using System.Data.SqlClient;
using BasicLoginServer.Models;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace BasicLoginServer.Helpers
{
    public class Sendgrid
    {
        public static string sendgridUsername = CloudConfigurationManager.GetSetting("SendgridUsername");
        public static string sendgridPwd = CloudConfigurationManager.GetSetting("SendgridPwd");
        public static string sendgridServer = CloudConfigurationManager.GetSetting("SendgridServer");
        public static string sendgridApiKey = CloudConfigurationManager.GetSetting("SendgridApiKey");


        //public static void SendEmail(Email email)
        //{

        //    ExecuteSendEmail(email);
        //}


        //static async Task ExecuteSendEmail(Email email)
        //{
        //    try
        //    {
        //        var apiKey = sendgridApiKey;
        //        var client = new SendGridClient(apiKey);
        //        var from = new EmailAddress(email.FromEmail, email.FromName);
        //        var subject = email.Subject;
        //        var to = new EmailAddress(email.ToEmail);
        //        var plainTextContent = email.Text;
        //        var htmlContent = email.Html;
        //        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //        var response = await client.SendEmailAsync(msg);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
    }
}