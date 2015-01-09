using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server.Partials;
using UserAdminApp.Server.Partials.Launcher;

namespace UserAdminApp.Server.Handlers {
    public class LauncherHooks {

        public static void Register() {


            // Workspace root (Launchpad)
            Starcounter.Handle.GET( "/UserAdminApp", () => {
                Starcounter.Response resp;
                Starcounter.X.GET("/UserAdminApp/admin/users", out resp);
                return resp;
            });

            Starcounter.Handle.GET("/launcher/app-name", () => {
                return new AppName();
            }, HandlerOptions.ApplicationLevel);

            // App name required for Launchpad
            Starcounter.Handle.GET("/launcher/app-icon", () => {
                return new UserAdminApp.Server.Partials.Page() { Html = "/useradminapp/launcher/app-icon.html" };
            }, HandlerOptions.ApplicationLevel);

            // Menu
            Starcounter.Handle.GET( "/launcher/menu", () => {

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
            }, HandlerOptions.ApplicationLevel);

            Starcounter.Handle.GET("/launcher/search?query={?}", (string query) => {
                return X.GET<Json>("/UserAdminApp/search=" + query);
            }, HandlerOptions.ApplicationLevel);

            // TODO:
            // Not sure where to put this.
            Starcounter.Handle.GET("/UserAdminApp/search={?}", (string query) => {
                var result = new UserAdminApp.Server.Partials.Administrator.SearchResult();
                result.Html = "/useradminapp/launcher/app-search.html";

                // If not authorized we don't return any results.
                if (!string.IsNullOrEmpty(query) && UserSession.IsAdmin()) {
                    result.Users = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username LIKE ? FETCH ?", "%" + query + "%", 5);
                    result.Groups = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.Name LIKE ? FETCH ?", "%" + query + "%", 5);
                }
                return result;
            });
        }
    }
}
