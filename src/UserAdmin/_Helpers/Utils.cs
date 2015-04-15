using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace UserAdmin {
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

                MailAddress fromAddress = new MailAddress(settings.Username, "Polyjuice App");
                MailAddress toAddress = new MailAddress(toEmail);

                const string subject = "Polyjuice App, Reset password request";

                string body = string.Format(
                    "Hi {0}<br><br>"+
                    "We received a request to reset your password<br><br>" +
                    "Click <a href='{1}'>here</a> to set a new password<br><br>"+
                    "Thanks<br>",
                    
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

        static public bool IsValidEmail(string email) {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            } catch {
                return false;
            }
        }

        static public string GetGravatarUrl(string email) {

            using (MD5 md5Hash = MD5.Create()) {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++) {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return "http://www.gravatar.com/avatar/" + sBuilder.ToString() + "?s=32&d=identicon";
            }
        }

    }
}
