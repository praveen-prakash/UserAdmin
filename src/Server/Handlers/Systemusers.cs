using Concepts.Ring8.Polyjuice.App;
using Concepts.Ring8.Polyjuice.Permissions;
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
    public class SystemUsers {

        public static void Register() {

            //
            // Create System user
            //
            Starcounter.Handle.GET( "/UserAdminApp/admin/createuser", (Request request) => {

                Json page;
                if (!UriPermissionHelper.TryNavigateTo("/UserAdminApp/admin/createuser", request, "/useradminapp/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.CreateUserPage() { Html = "/useradminapp/partials/administrator/createuser.html", Uri = request.Uri };
            });

            //
            // Get System users
            //
            Starcounter.Handle.GET( "/UserAdminApp/admin/users", (Request request) => {

                Json page;
                if (!UriPermissionHelper.TryNavigateTo("/UserAdminApp/admin/users", request, "/useradminapp/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.ListUsersPage() { Html = "/useradminapp/partials/administrator/listusers.html", Uri = request.Uri };
            });

            //
            // Get System user
            //
            Starcounter.Handle.GET( "/UserAdminApp/admin/users/{?}", (string userid, Request request) => {

                Json page;
                if (!UriPermissionHelper.TryNavigateTo("/UserAdminApp/admin/users/{?}", request, "/useradminapp/redirect.html", out page)) {
                    return page;
                }

                // Get system user
                Concepts.Ring3.SystemUser user = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.ObjectID=?", userid).First;
                if (user == null) {
                    // TODO: Return a "User not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Concepts.Ring3.SystemUser systemUser = UriPermissionHelper.GetCurrentSystemUser();
                Concepts.Ring3.SystemUserGroup adminGroup = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.Name=?", Program.AdminGroupName).First;


                // Check if current user has permission to get this user instance
                if (UriPermission.IsMemberOfGroup(systemUser, adminGroup)) {

                    if (user.WhoIs is Concepts.Ring1.Person) {
                        Partials.Administrator.EditPersonPage editPersonPage = new Partials.Administrator.EditPersonPage() {
                            Html = "/useradminapp/partials/administrator/editperson.html",
                            Uri = request.Uri
                        };
                        Db.Scope(() => {
                            editPersonPage.Data = user;
                        });
                        return editPersonPage;
                    }
                    else if (user.WhoIs is Concepts.Ring2.Company) {
                        Partials.Administrator.EditCompanyPage editCompanyPage = new Partials.Administrator.EditCompanyPage() {
                            Html = "/useradminapp/partials/administrator/editcompany.html",
                            Uri = request.Uri
                        };
                        Db.Scope(() => {
                            editCompanyPage.Data = user;
                        });
                        return editCompanyPage;
                    }
                }
                else if (user == systemUser) {
                    // User can edit it's self
                }
                else {
                    // No rights
                    // User trying to view another's users data

                    // User has no permission, redirect to app's root page
                    return new RedirectPage() {
                        Html = "/useradminapp/redirect.html",
                        RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp"
                    };

                }


                return (ushort)System.Net.HttpStatusCode.NotFound;
            });

            //
            // Reset password
            //
            Starcounter.Handle.GET( "/UserAdminApp/user/resetpassword?{?}", (string query, Request request) => {

                NameValueCollection queryCollection = HttpUtility.ParseQueryString(query);
                string token = queryCollection.Get("token");
                if (token == null) {
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                // Retrive the resetPassword instance
                ResetPassword resetPassword = Db.SQL<UserAdminApp.Database.ResetPassword>("SELECT o FROM UserAdminApp.Database.ResetPassword o WHERE o.Token=? AND o.Expire>?", token, DateTime.UtcNow).First;

                if (resetPassword == null) {
                    // TODO: Show message "Reset token already used or expired"
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                if (resetPassword.User == null) {
                    // TODO: Show message "User deleted"
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Concepts.Ring3.SystemUser systemUser = resetPassword.User;

                Partials.User.ResetPasswordPage page = new Partials.User.ResetPasswordPage() {
                    Html = "/useradminapp/partials/user/resetpassword.html",
                    Uri = "/UserAdminApp/user/resetpassword"
                    //Uri = request.Uri // TODO:
                };

                page.resetPassword = resetPassword;

                if (systemUser.WhoIs != null) {
                    page.FullName = systemUser.WhoIs.FullName;
                }
                else {
                    page.FullName = systemUser.Username;
                }

                return page;
            });
        }
    }
}
