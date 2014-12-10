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

// http://localhost:8080/launcher/workspace/UserAdminApp/
// admin/systemusers/{?}
// admin/createuser
// settings

namespace UserAdminApp.Server.Handlers {
    public class SystemUserGroups {

        public static void RegisterHandlers() {


            //
            // System users
            //
            Starcounter.Handle.GET(Admin.Port, "/UserAdminApp/createusergroup", (Request request) => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage(Admin.LauncherWorkSpacePath + "/UserAdminApp/createusergroup");
                }

                Partials.Administrator.CreateUserGroupPage page = new Partials.Administrator.CreateUserGroupPage() {
                    Html = "/partials/administrator/createusergroup.html",
                    Uri = request.Uri
                };
                return page;
            });

            //
            // List user groups
            //
            Starcounter.Handle.GET(Admin.Port, "/UserAdminApp/usergroups", (Request request) => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage(Admin.LauncherWorkSpacePath + "/UserAdminApp/usergroups");
                }

                Partials.Administrator.ListUserGroupsPage page = new Partials.Administrator.ListUserGroupsPage() {
                    Html = "/partials/administrator/listusergroups.html",
                    Uri = request.Uri
                };
                return page;
            });

            //
            // System user group
            //
            Starcounter.Handle.GET(Admin.Port, "/UserAdminApp/usergroups/{?}", (string usergroupid, Request request) => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage(Admin.LauncherWorkSpacePath + "/UserAdminApp/usergroups/" + usergroupid);
                }

                Concepts.Ring3.SystemUserGroup usergroup = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.ObjectID=?", usergroupid).First;

                if (usergroup == null) {
                    // TODO: Return a "User Group not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Partials.Administrator.EditUserGroupPage page = new Partials.Administrator.EditUserGroupPage() {
                    Html = "/partials/administrator/editusergroup.html",
                    Uri = request.Uri
                };
                page.Transaction = new Transaction();
                page.Data = usergroup;
                return page;
            });
        }
    }
}
