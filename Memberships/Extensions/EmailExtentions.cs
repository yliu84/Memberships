﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;


namespace Memberships.Extensions
{
    public static class EmailExtentions
    {
        public static void Send(this IdentityMessage message)
        {
            try
            {
                //Read settings from Web.Config
                var password = ConfigurationManager.AppSettings["password"];
                var from = ConfigurationManager.AppSettings["from"];
                var host = ConfigurationManager.AppSettings["host"];
                var port = int.Parse(ConfigurationManager.AppSettings["port"]);

                //Create the email to send
                var email = new MailMessage(from, message.Destination, message.Subject, message.Body);
                email.IsBodyHtml = true;

                //Create an SMTP client to send the eamil
                var client = new SmtpClient(host, port);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from, password);

                //Send the email
                client.Send(email);
            }
            catch
            {

            }
           


        }
    }
}