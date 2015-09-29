using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Starcounter;

namespace UserAdmin {
    public class SystemUserGroupsHandlers {
        public static void Register() {

            // Create System User group
            Handle.GET("/UserAdmin/admin/createusergroup", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/createusergroup", request, "/useradmin/viewmodels/RedirectPage.html", out page)) {
                    return page;
                }

                return new CreateUserGroupPage() { Html = "/UserAdmin/viewmodels/partials/administrator/CreateUserGroupPage.html", Uri = request.Uri };
            });

            // Get System user groups
            Handle.GET("/UserAdmin/admin/usergroups", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/usergroups", request, "/useradmin/viewmodels/RedirectPage.html", out page)) {
                    return page;
                }

                return new ListUserGroupsPage() { Html = "/UserAdmin/viewmodels/partials/administrator/ListUserGroupsPage.html", Uri = request.Uri };

            });

            // Get System user group
            Handle.GET("/UserAdmin/admin/usergroups/{?}", (string usergroupid, Request request) => {
                Json page;

                if (!Helper.TryNavigateTo("/UserAdmin/admin/usergroups/{?}", request, "/useradmin/viewmodels/RedirectPage.html", out page)) {
                    return page;
                }

                Simplified.Ring3.SystemUserGroup usergroup = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.ObjectID=?", usergroupid).First;

                if (usergroup == null) {
                    // TODO: Return a "User Group not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                return Db.Scope<string, Simplified.Ring3.SystemUserGroup, Json>((uri, ug) => {
                    EditUserGroupPage editUserGroupPage = new EditUserGroupPage() {
                        Html = "/UserAdmin/viewmodels/partials/administrator/EditUserGroupPage.html",
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
