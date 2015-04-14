using Starcounter;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserAdmin.Database;
using UserAdmin.Server.Partials;


namespace UserAdmin.Server.Handlers {
    public class Settings {

        public static void Register() {

            //
            // Settings
            //
            Starcounter.Handle.GET( "/UserAdmin/admin/settings", (Request request) => {

                // TODO: At the moment we only show MailSettings, eventually MailSettings will only be one part of all settings

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/settings", request, "/useradmin/redirect.html", out page)) {
                    return page;
                }

                return Db.Scope<string, Json>((string uri) => {
                    Partials.Administrator.SettingsPage settingsPage = new Partials.Administrator.SettingsPage() {
                        Html = "/useradmin/partials/administrator/settings.html",
                        Uri = uri
                    };
                    settingsPage.Data = UserAdmin.Database.SettingsMailServer.Settings;
                    return settingsPage;
                },
                request.Uri);
            });
        }
    }
}
