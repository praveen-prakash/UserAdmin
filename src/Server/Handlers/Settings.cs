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


namespace UserAdminApp.Server.Handlers {
    public class Settings {

        public static void Register() {

            //
            // Settings
            //
            Starcounter.Handle.GET( "/UserAdminApp/admin/settings", (Request request) => {

                // TODO: At the moment we only show MailSettings, eventually MailSettings will only be one part of all settings

                Json page;
                if (!Helper.TryNavigateTo("/UserAdminApp/admin/settings", request, "/useradminapp/redirect.html", out page)) {
                    return page;
                }

                return Db.Scope<string, Json>((string uri) => {
                    Partials.Administrator.SettingsPage settingsPage = new Partials.Administrator.SettingsPage() {
                        Html = "/useradminapp/partials/administrator/settings.html",
                        Uri = uri
                    };
                    settingsPage.Data = UserAdminApp.Database.SettingsMailServer.Settings;
                    return settingsPage;
                },
                request.Uri);
            });

            //
            // Settings
            //
            //Starcounter.Handle.GET(Program.Port, "/UserAdminApp/clean", (Request request) => {

            //    if (!UserSession.IsAuthorized()) {
            //        return UserSession.GetSignInPage(Program.LauncherWorkSpacePath + request.Uri);
            //    }

            //    Db.Transaction(() => {

            //        Db.SlowSQL("DELETE FROM Concepts.Ring1.Person");

            //        Db.SlowSQL("DELETE FROM Concepts.Ring2.Company");
            //        Db.SlowSQL("DELETE FROM Simplified.Ring3.EmailAddress");

            //        Db.SlowSQL("DELETE FROM Simplified.Ring3.SystemUserGroup");
            //        Db.SlowSQL("DELETE FROM Simplified.Ring3.SystemUserGroupMember");
            //        Db.SlowSQL("DELETE FROM Simplified.Ring3.SystemUser");

            //        Db.SlowSQL("DELETE FROM Simplified.Ring6.ResetPassword");
            //        Db.SlowSQL("DELETE FROM UserAdminApp.Database.SettingsMailServer");
            //    });


            //    return 200;
            //});
        }
    }
}
