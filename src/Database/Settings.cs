using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.Web;
using System.Net;
namespace UserAdminApp.Database {

    //[Database]
    //public class Settings {

    //    public SettingsMailServer SettingsMailServer { get; private set; }


    //}


    [Database]
    public class SettingsMailServer {
        public bool Enabled;
        public string Host;
        public int Port;
        public bool EnableSsl;
        public string Username;
        public string Password;

        /// <summary>
        /// Settings
        /// </summary>
        static public SettingsMailServer Settings {

            get {

                SettingsMailServer settings = Db.SQL<UserAdminApp.Database.SettingsMailServer>("SELECT o FROM UserAdminApp.Database.SettingsMailServer o").First;
                if (settings == null) {

                    Db.Transaction(() => {
                        settings = new SettingsMailServer();
                        settings.Enabled = false;
                        settings.Host = "";
                        settings.Port = 587;
                        settings.EnableSsl = true;
                        settings.Username = "";
                        settings.Password = "";
                    });
                }

                return settings;
            }
        }

        static public bool IsValidSettings() {

            SettingsMailServer settings = SettingsMailServer.Settings;

            if (string.IsNullOrEmpty(settings.Host)) {
                return false;
            }

            if (settings.Port > IPEndPoint.MaxPort || (settings.Port < IPEndPoint.MinPort)) {
                return false;
            }

            return true;
        }
    }
}
