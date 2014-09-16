using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Database;

namespace UserAdminApp.Server {
    public class Utils {

        /// <summary>
        /// Send Reset password email
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="link"></param>
        public static void sendResetPasswordMail(string toFullName, string toEmail, string link) {

            SettingsMailServer settings = SettingsMailServer.Settings;

            try {

                if (!settings.Enabled) {
                    throw new InvalidOperationException("Mail service not enabled in the configuration.");
                }

                MailAddress fromAddress = new MailAddress(settings.Username, "UserAdminApp Reset Password");
                MailAddress toAddress = new MailAddress(toEmail);

                const string subject = "EVENT: UserAdminApp Reset Password";

                string body = string.Format(
                    "Hi {0}<br><br>"+
                    "We received a request to reset your password<br><br>" +
                    "Click <a href='{1}'>here</a> to set a new password<br><br>"+
                    "Thanks,<br>"+
                    " - The UserAdminApp Team<br>",
                    toFullName, link);

                var smtp = new SmtpClient {
                    Host = settings.Host,
                    Port = settings.Port,
                    EnableSsl = settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, settings.Password)
                };

                using (var message = new MailMessage(fromAddress, toAddress) {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body

                }) {
                    smtp.Send(message);
                }
            }
            catch (Exception e) {
                throw e;
                // TODO:
                //LogWriter.WriteLine(string.Format("ERROR: Failed to send registration email event. {0}", e.Message));
            }
        }

    }
}
