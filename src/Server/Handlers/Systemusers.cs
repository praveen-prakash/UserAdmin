using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server.Partials;

namespace UserAdminApp.Server.Handlers {
    public class Systemusers {

        public static void RegisterHandlers() {

            #region System Users Pages/Partials



            //
            // System users
            //
            Starcounter.Handle.GET("/admin/systemusers", () => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage("/launcher/workspace/admin/systemusers");
                }

                SystemUsersPage page = new SystemUsersPage() {
                    Html = "/partials/systemusers.html",
                    Uri = "/admin/systemusers"
                };
                return page;
            });

            //
            // System user
            //
            Starcounter.Handle.GET("/admin/systemusers/{?}", (Request request, string userid) => {

                if (!Admin.IsAuthorized()) {
                    return Admin.GetSignInPage("/launcher/workspace/admin/systemusers/" + userid);
                }

                Concepts.Ring3.SystemUser user = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", userid).First;

                if (user == null) {
                    // TODO: Return a "User not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Partials.SystemUserPage page = new Partials.SystemUserPage() {
                    Html = "/partials/systemuser.html",
                    Uri = "/admin/systemusers/" + user.Username
                };

                page.Transaction = new Transaction();   // TODO: How to close this transaction if the user do a refresh in the browser?
                page.Data = user;

                return page;
            });

            Starcounter.Handle.GET("/admin/systemuser/register", () => {

                SystemUserRegister page = new SystemUserRegister() {
                    Html = "/systemuserregister.html",
                    Uri = "/admin/systemuser/register"
                };

                return page;
            });
            #endregion


        }

    }
}
