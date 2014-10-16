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
            Starcounter.Handle.GET(Admin.Port, "/admin/settings", (Request request) => {

                // TODO: At the moment we only show MailSettings, eventually MailSettings will only be one part of all settings

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage(Admin.LauncherWorkSpacePath + request.Uri);
                }

                Partials.Administrator.SettingsPage page = new Partials.Administrator.SettingsPage() {
                    Html = "/partials/administrator/settings.html",
                    Uri = request.Uri
                };

                page.Transaction = new Transaction();
                page.Data = SettingsMailServer.Settings;

                return page;
            });

            //
            // Settings
            //
            Starcounter.Handle.GET(Admin.Port, "/admin/clean", (Request request) => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage(Admin.LauncherWorkSpacePath + request.Uri);
                }

                Db.Transaction(() => {

                    Db.SlowSQL("DELETE FROM Concepts.Ring1.Person");

                    Db.SlowSQL("DELETE FROM Concepts.Ring2.Company");
                    Db.SlowSQL("DELETE FROM Concepts.Ring2.EMailAddress");

                    Db.SlowSQL("DELETE FROM Concepts.Ring3.SystemUserGroup");
                    Db.SlowSQL("DELETE FROM Concepts.Ring3.SystemUserGroupMember");
                    Db.SlowSQL("DELETE FROM Concepts.Ring3.SystemUser");
                    Db.SlowSQL("DELETE FROM Concepts.Ring5.SystemUserGroupBasedOn");

                    Db.SlowSQL("DELETE FROM UserAdminApp.Database.ResetPassword");
                    Db.SlowSQL("DELETE FROM UserAdminApp.Database.SettingsMailServer");
                });


                return 200;
            });
        }
    }
}
