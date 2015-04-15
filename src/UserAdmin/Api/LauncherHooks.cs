using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using PolyjuiceNamespace;

namespace UserAdmin {
    public class LauncherHooks {
        public static void Register() {

            // Workspace root (Launchpad)
            Starcounter.Handle.GET("/useradmin", () => {
                Starcounter.Response resp;
                Starcounter.X.GET("/useradmin/admin/users", out resp);
                return resp;
            });

            Starcounter.Handle.GET("/useradmin/app-name", () => {
                return new AppName();
            });

            // App name required for Launchpad
            Starcounter.Handle.GET("/useradmin/app-icon", () => {
                return new Page() { Html = "/UserAdmin/viewmodels/launcher/AppIconPage.html" };
            });

            // Menu
            Starcounter.Handle.GET("/useradmin/menu", () => {

                UserSessionPage userSessionPage = new UserSessionPage();

                var menuPage = new AdminMenu() {
                    Html = "/UserAdmin/viewmodels/launcher/AppMenuPage.html",
                    IsAdministrator = UserSessionPage.IsAdmin()
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
            Starcounter.Handle.GET("/useradmin/search/{?}", (string query) => {
                var result = new SearchResultPage();
                result.Html = "/UserAdmin/viewmodels/launcher/AppSearchPage.html";

                // If not authorized we don't return any results.
                if (!string.IsNullOrEmpty(query) && UserSessionPage.IsAdmin()) {
                    result.Users = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.Username LIKE ? FETCH ?", "%" + query + "%", 5);
                    result.Groups = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name LIKE ? FETCH ?", "%" + query + "%", 5);
                }

                return result;
            });

            Polyjuice.Map("/useradmin/menu", "/polyjuice/menu");
            Polyjuice.Map("/useradmin/app-name", "/polyjuice/app-name");
            Polyjuice.Map("/useradmin/app-icon", "/polyjuice/app-icon");
            Polyjuice.Map("/useradmin/search/@w", "/polyjuice/search?query=@w");
        }
    }
}
