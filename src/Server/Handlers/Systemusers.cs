using Simplified.Ring2;
using Simplified.Ring6;
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
    public class SystemUsers {

        public static void Register() {

            //
            // Create System user
            //
            Starcounter.Handle.GET("/UserAdmin/admin/createuser", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/createuser", request, "/useradmin/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.CreateUserPage() { Html = "/useradmin/partials/administrator/createuser.html", Uri = request.Uri };
            });

            //
            // Get System users
            //
            Starcounter.Handle.GET("/UserAdmin/admin/users", (Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/users", request, "/useradmin/redirect.html", out page)) {
                    return page;
                }

                return new Partials.Administrator.ListUsersPage() { Html = "/useradmin/partials/administrator/listusers.html", Uri = request.Uri };
            });


            Handle.GET("/UserAdmin/admin/users/{?}", (string userid, Request request) => {

                return Db.Scope<Json>(() => {
                    return X.GET<Json>(string.Format("/UserAdmin/admin/_users/{0}", userid));
                });
            });
            //
            // Get System user
            //
            Handle.GET("/UserAdmin/admin/_users/{?}", (string userid, Request request) => {

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/users/{?}", request, "/useradmin/redirect.html", out page)) {
                    return page;
                }

                // Get system user
                Simplified.Ring3.SystemUser user = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.ObjectID=?", userid).First;
                if (user == null) {
                    // TODO: Return a "User not found" page
                    return null;
                    //return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Simplified.Ring3.SystemUser systemUser = Helper.GetCurrentSystemUser();
                Simplified.Ring3.SystemUserGroup adminGroup = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name=?", Program.AdminGroupName).First;


                // Check if current user has permission to get this user instance
                if (Helper.IsMemberOfGroup(systemUser, adminGroup)) {

                    if (user.WhoIs is Person) {
                        return Db.Scope<string, Simplified.Ring3.SystemUser, Json>((uri, personUser) => {
                            return new Partials.Administrator.EditPersonPage() {
                                Html = "/useradmin/partials/administrator/editperson.html",
                                Uri = uri,
                                Data = personUser
                            };
                        },
                        request.Uri, user);
                    }
                    else if (user.WhoIs is Organization) {
                        Db.Scope<string, Simplified.Ring3.SystemUser, Json>((uri, companyUser) => {
                            return new Partials.Administrator.EditCompanyPage() {
                                Html = "/useradmin/partials/administrator/editcompany.html",
                                Uri = uri,
                                Data = companyUser
                            };
                        },
                        request.Uri, user);
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
                        Html = "/useradmin/redirect.html",
                        RedirectUrl = "/UserAdmin"
                    };

                }


                return (ushort)System.Net.HttpStatusCode.NotFound;
            });

            //
            // Reset password
            //
            Starcounter.Handle.GET("/UserAdmin/user/resetpassword?{?}", (string query, Request request) => {

                NameValueCollection queryCollection = HttpUtility.ParseQueryString(query);
                string token = queryCollection.Get("token");
                if (token == null) {
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                // Retrive the resetPassword instance
                ResetPassword resetPassword = Db.SQL<Simplified.Ring6.ResetPassword>("SELECT o FROM Simplified.Ring6.ResetPassword o WHERE o.Token=? AND o.Expire>?", token, DateTime.UtcNow).First;

                if (resetPassword == null) {
                    // TODO: Show message "Reset token already used or expired"
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                if (resetPassword.User == null) {
                    // TODO: Show message "User deleted"
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Simplified.Ring3.SystemUser systemUser = resetPassword.User;

                Partials.User.ResetPasswordPage page = new Partials.User.ResetPasswordPage() {
                    Html = "/useradmin/partials/user/resetpassword.html",
                    Uri = "/UserAdmin/user/resetpassword"
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
