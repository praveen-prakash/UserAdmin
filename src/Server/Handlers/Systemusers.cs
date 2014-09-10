using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server.Partials;

// http://localhost:8080/launcher/workspace/admin/
// admin/systemusers/{?}
// admin/createuser
// settings

namespace UserAdminApp.Server.Handlers {
    public class Systemusers {

        public static void RegisterHandlers() {

            #region System Users Pages/Partials

            //
            // System users
            //
            Starcounter.Handle.GET("/admin/createuser", () => {

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
            Starcounter.Handle.GET("/admin/users", () => {

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
            Starcounter.Handle.GET("/admin/users/{?}", (Request request, string userid) => {

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


            //Starcounter.Handle.GET("/admin/systemuser/register", () => {

            //    SystemUserRegister page = new SystemUserRegister() {
            //        Html = "/systemuserregister.html",
            //        Uri = "/admin/systemuser/register"
            //    };

            //    return page;
            //});
            #endregion
        }
    }
}
