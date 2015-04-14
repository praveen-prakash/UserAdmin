using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdmin.Server.Partials;
using UserAdmin.Server.Partials.Launcher;
using PolyjuiceNamespace;

namespace UserAdmin.Server.Handlers {
    public class LauncherHooks {

        public static void Register() {


            // Workspace root (Launchpad)
            Starcounter.Handle.GET( "/UserAdmin", () => {
                Starcounter.Response resp;
                Starcounter.X.GET("/UserAdmin/admin/users", out resp);
                return resp;
            });

            Starcounter.Handle.GET("/UserAdmin/app-name", () => {
                return new AppName();
            });

            // App name required for Launchpad
            Starcounter.Handle.GET("/UserAdmin/app-icon", () => {
                return new UserAdmin.Server.Partials.Page() { Html = "/useradmin/launcher/app-icon.html" };
            });

            // Menu
            Starcounter.Handle.GET("/UserAdmin/menu", () => {

                UserAdmin.Server.UserSession userSessionPage = new UserAdmin.Server.UserSession();

                var menuPage = new AdminMenu() {
                    Html = "/useradmin/launcher/adminmenu.html",
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
            Starcounter.Handle.GET("/UserAdmin/search/{?}", (string query) => {
                var result = new UserAdmin.Server.Partials.Administrator.SearchResult();
                result.Html = "/useradmin/launcher/app-search.html";

                // If not authorized we don't return any results.
                if (!string.IsNullOrEmpty(query) && UserSession.IsAdmin()) {
                    result.Users = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.Username LIKE ? FETCH ?", "%" + query + "%", 5);
                    result.Groups = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name LIKE ? FETCH ?", "%" + query + "%", 5);
                }
                return result;
            });

            Polyjuice.Map("/UserAdmin/menu", "/polyjuice/menu");
            Polyjuice.Map("/UserAdmin/app-name", "/polyjuice/app-name");
            Polyjuice.Map("/UserAdmin/app-icon", "/polyjuice/app-icon");
            Polyjuice.Map("/UserAdmin/search/@w", "/polyjuice/search?query=@w");
        }
    }
}
