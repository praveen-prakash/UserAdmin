using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server.Partials;
using UserAdminApp.Server.Partials.Launcher;
using PolyjuiceNamespace;

namespace UserAdminApp.Server.Handlers {
    public class LauncherHooks {

        public static void Register() {


            // Workspace root (Launchpad)
            Starcounter.Handle.GET( "/UserAdminApp", () => {
                Starcounter.Response resp;
                Starcounter.X.GET("/UserAdminApp/admin/users", out resp);
                return resp;
            });

            Starcounter.Handle.GET("/UserAdminApp/app-name", () => {
                return new AppName();
            });

            // App name required for Launchpad
            Starcounter.Handle.GET("/UserAdminApp/app-icon", () => {
                return new UserAdminApp.Server.Partials.Page() { Html = "/useradminapp/launcher/app-icon.html" };
            });

            // Menu
            Starcounter.Handle.GET("/UserAdminApp/menu", () => {

                UserAdminApp.Server.UserSession userSessionPage = new UserAdminApp.Server.UserSession();

                var menuPage = new AdminMenu() {
                    Html = "/useradminapp/launcher/adminmenu.html",
                    IsAdministrator = UserSession.IsAdmin()
                };

                userSessionPage.Menu = menuPage;

                string sessionID = Session.Current.SessionIdString;
                if (Program.Sessions.ContainsKey(sessionID)) {
                    Program.Sessions.Remove(sessionID);
                }
                Program.Sessions.Add(sessionID, userSessionPage);

                return menuPage;
            });

            // TODO:
            // Not sure where to put this.
            Starcounter.Handle.GET("/UserAdminApp/search/{?}", (string query) => {
                var result = new UserAdminApp.Server.Partials.Administrator.SearchResult();
                result.Html = "/useradminapp/launcher/app-search.html";

                // If not authorized we don't return any results.
                if (!string.IsNullOrEmpty(query) && UserSession.IsAdmin()) {
                    result.Users = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username LIKE ? FETCH ?", "%" + query + "%", 5);
                    result.Groups = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.Name LIKE ? FETCH ?", "%" + query + "%", 5);
                }
                return result;
            });

            Polyjuice.Map("/UserAdminApp/menu", "/polyjuice/menu");
            Polyjuice.Map("/UserAdminApp/app-name", "/polyjuice/app-name");
            Polyjuice.Map("/UserAdminApp/app-icon", "/polyjuice/app-icon");
            Polyjuice.Map("/UserAdminApp/search/@w", "/polyjuice/search?query=@w");
        }
    }
}
