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
    public class SystemUserGroups {

        public static void Register() {

            //
            // Create System User group
            //
            Starcounter.Handle.GET( "/UserAdmin/admin/createusergroup", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/createusergroup", request, "/useradmin/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.CreateUserGroupPage() { Html = "/useradmin/partials/administrator/createusergroup.html", Uri = request.Uri };
            });

            //
            // Get System user groups
            //
            Starcounter.Handle.GET( "/UserAdmin/admin/usergroups", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/usergroups", request, "/useradmin/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.ListUserGroupsPage() { Html = "/useradmin/partials/administrator/listusergroups.html", Uri = request.Uri };

            });

            //
            // Get System user group
            //
            Starcounter.Handle.GET( "/UserAdmin/admin/usergroups/{?}", (string usergroupid, Request request) => {
                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/usergroups/{?}", request, "/useradmin/redirect.html", out page)) {
                    return page;
                }

                Simplified.Ring3.SystemUserGroup usergroup = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.ObjectID=?", usergroupid).First;

                if (usergroup == null) {
                    // TODO: Return a "User Group not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                return Db.Scope<string, Simplified.Ring3.SystemUserGroup, Json>((uri, ug) => {
                    Partials.Administrator.EditUserGroupPage editUserGroupPage = new Partials.Administrator.EditUserGroupPage() {
                        Html = "/useradmin/partials/administrator/editusergroup.html",
                        Uri = uri
                    };
                    editUserGroupPage.Data = ug;
                    return editUserGroupPage;
                }, 
                request.Uri, usergroup);
            });
        }
    }
}
