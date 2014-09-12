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
    public class Systemusers {

        public static void RegisterHandlers() {


            //
            // System users
            //
            Starcounter.Handle.GET(Admin.Port, "/admin/createuser", () => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage("/launcher/workspace/admin/createuser");
                }

                Partials.Administrator.CreateUserPage page = new Partials.Administrator.CreateUserPage() {
                    Html = "/partials/administrator/createuser.html",
                    Uri = "/admin/createuser"
                };
                return page;
            });

            //
            // List users
            //
            Starcounter.Handle.GET(Admin.Port, "/admin/users", () => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage("/launcher/workspace/admin/users");
                }

                Partials.Administrator.ListUsersPage page = new Partials.Administrator.ListUsersPage() {
                    Html = "/partials/administrator/listusers.html",
                    Uri = "/admin/users"
                };
                return page;
            });

            //
            // System user
            //
            Starcounter.Handle.GET(Admin.Port, "/admin/users/{?}", (Request request, string userid) => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage("/launcher/workspace/admin/users/" + userid);
                }

                Concepts.Ring3.SystemUser user = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", userid).First;

                if (user == null) {
                    // TODO: Return a "User not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                if (user.WhoIs is Concepts.Ring1.Person) {
                    Partials.Administrator.EditPersonPage page = new Partials.Administrator.EditPersonPage() {
                        Html = "/partials/administrator/editperson.html",
                        Uri = "/admin/users/" + user.Username
                    };
                    page.Transaction = new Transaction();   // TODO: How to close this transaction if the user do a refresh in the browser?
                    page.Data = user;
                    return page;
                }
                else if (user.WhoIs is Concepts.Ring2.Company) {
                    Partials.Administrator.EditCompanyPage page = new Partials.Administrator.EditCompanyPage() {
                        Html = "/partials/administrator/editcompany.html",
                        Uri = "/admin/users/" + user.Username
                    };
                    page.Transaction = new Transaction();   // TODO: How to close this transaction if the user do a refresh in the browser?
                    page.Data = user;
                    return page;
                }

                return (ushort)System.Net.HttpStatusCode.NotFound;
            });

            //
            // Reset password
            //
            Starcounter.Handle.GET(Admin.Port, "/admin/user/resetpassword?{?}", (string query, Request request) => {

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
                    Html = "/partials/user/resetpassword.html",
                    Uri = "/admin/user/resetpassword"
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
