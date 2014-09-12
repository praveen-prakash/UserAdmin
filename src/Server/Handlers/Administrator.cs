using Starcounter;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserAdminApp.Database;
using UserAdminApp.Server.Partials;

// http://localhost:8080/launcher/workspace/admin/
// admin/systemusers/{?}
// admin/createuser
// settings

namespace UserAdminApp.Server.Handlers {
    public class Administrator {

        public static void RegisterHandlers() {

            //
            // Settings
            //
            Starcounter.Handle.GET(Admin.Port,"/admin/settings", () => {

                // TODO: At the moment we only show MailSettings, eventually MailSettings will only be one part of all settings

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage("/launcher/workspace/admin/settings");
                }

                Partials.Administrator.SettingsPage page = new Partials.Administrator.SettingsPage() {
                    Html = "/partials/administrator/settings.html",
                    Uri = "/admin/settings"
                };

                page.Transaction = new Transaction();   // TODO: How to close this transaction if the user do a refresh in the browser?
                page.Data = SettingsMailServer.Settings;

                return page;
            });
        }
    }
}
