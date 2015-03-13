﻿using Starcounter;
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
    public class SystemUserGroups {

        public static void Register() {

            //
            // Create System User group
            //
            Starcounter.Handle.GET( "/UserAdminApp/admin/createusergroup", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdminApp/admin/createusergroup", request, "/useradminapp/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.CreateUserGroupPage() { Html = "/useradminapp/partials/administrator/createusergroup.html", Uri = request.Uri };
            });

            //
            // Get System user groups
            //
            Starcounter.Handle.GET( "/UserAdminApp/admin/usergroups", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdminApp/admin/usergroups", request, "/useradminapp/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.ListUserGroupsPage() { Html = "/useradminapp/partials/administrator/listusergroups.html", Uri = request.Uri };

            });

            //
            // Get System user group
            //
            Starcounter.Handle.GET( "/UserAdminApp/admin/usergroups/{?}", (string usergroupid, Request request) => {
                Json page;
                if (!Helper.TryNavigateTo("/UserAdminApp/admin/usergroups/{?}", request, "/useradminapp/redirect.html", out page)) {
                    return page;
                }

                Simplified.Ring3.SystemUserGroup usergroup = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.ObjectID=?", usergroupid).First;

                if (usergroup == null) {
                    // TODO: Return a "User Group not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                return Db.Scope<string, Simplified.Ring3.SystemUserGroup, Json>((uri, ug) => {
                    Partials.Administrator.EditUserGroupPage editUserGroupPage = new Partials.Administrator.EditUserGroupPage() {
                        Html = "/useradminapp/partials/administrator/editusergroup.html",
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
