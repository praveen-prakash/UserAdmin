using PolyjuiceNamespace;
using Starcounter;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdmin.Partials.Launcher;

namespace UserAdmin {
    public class CommitHooks {

        internal static string Url;

        internal static void Register() {

            CommitHooks.Url = "/UserAdmin/__db/__" + StarcounterEnvironment.DatabaseNameLower + "/societyobjects/systemusersession";

            //Starcounter.Handle.GET(CommitHooks.Url, (Request request) => {
            //    return (ushort)System.Net.HttpStatusCode.OK;
            //});

            #region Sign in/out commit hooks
            // User signed in event
            Starcounter.Handle.POST(CommitHooks.Url, (Request request) => {

                string sessionID = Session.Current.SessionIdString;
                if (!Program.Sessions.ContainsKey(sessionID)) {
                    return new Response() { StatusCode = (ushort)System.Net.HttpStatusCode.InternalServerError, Body = "Failed to get the signin app Session" };
                }
                UserSessionPage admin = Program.Sessions[sessionID];

                bool isAuthorized = UserSessionPage.IsAdmin();

                // Hide or show menu choice 
                //Admin admin = Admin.AdminPage;
                if (admin.Menu != null) {
                    AdminMenu menu = admin.Menu as AdminMenu;
                    menu.IsAdministrator = isAuthorized;
                }

                return (ushort)System.Net.HttpStatusCode.OK;
            });

            // User signed out event
            Starcounter.Handle.DELETE(CommitHooks.Url, (Request request) => {

                bool isAuthorized = UserSessionPage.IsAdmin();

                string sessionID = Session.Current.SessionIdString;
                if (!Program.Sessions.ContainsKey(sessionID)) {
                    return new Response() { StatusCode = (ushort)System.Net.HttpStatusCode.InternalServerError, Body = "Failed to get the signin app Session" };
                }
                UserSessionPage admin = Program.Sessions[sessionID];

                // Hide or show menu choice 
                //Admin admin = Admin.AdminPage;
                if (admin.Menu != null) {
                    AdminMenu menu = admin.Menu as AdminMenu;
                    menu.IsAdministrator = isAuthorized;
                    menu.RedirectUrl = "/";
                }

                return (ushort)System.Net.HttpStatusCode.OK;
            });

            #endregion

            Polyjuice.Map(CommitHooks.Url, "/polyjuice/signin", "POST");
            Polyjuice.Map(CommitHooks.Url, "/polyjuice/signin", "DELETE");
        }
    }
}
