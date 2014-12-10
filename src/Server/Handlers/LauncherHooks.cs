using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server.Partials;

namespace UserAdminApp.Server.Handlers {
    public class LauncherHooks {

        public static void RegisterLauncherHooks() {


            // Workspace root (Launchpad)
            Starcounter.Handle.GET(8080, "/UserAdminApp", () => {
                Starcounter.Response resp;
                Starcounter.X.GET("/UserAdminApp/users", out resp);
                return resp;
            });

            Starcounter.Handle.GET("/launcher/app-name", () => {
                var json = new AppName();
                //json
                return json;
            }, HandlerOptions.ApplicationLevel);

            // App name required for Launchpad
            Starcounter.Handle.GET("/launcher/app-icon", () => {
                var iconpage = new Page() { Html = "/useradminapp/app-icon.html" };
                //json
                return iconpage;
            }, HandlerOptions.ApplicationLevel);

            // Menu
            Starcounter.Handle.GET(Admin.Port, "/launcher/menu", () => {

                UserAdminApp.Server.Admin adminPage = new UserAdminApp.Server.Admin();

                var menuPage = new UserAdminApp.Server.SystemUserMenu() {
                    Html = "/adminmenu.html",
                    IsSignedIn = Admin.IsAuthorized()
                };

                adminPage.Menu = menuPage;
                Admin.AdminPage = adminPage;
                return menuPage;
            }, HandlerOptions.ApplicationLevel);

            Starcounter.Handle.GET("/launcher/search?query={?}", (string query) => {
                return X.GET<Json>("/UserAdminApp/search=" + query);
            }, HandlerOptions.ApplicationLevel);

            // TODO:
            // Not sure where to put this.
            Starcounter.Handle.GET("/UserAdminApp/search={?}", (string query) => {
                var result = new UserAdminApp.Server.Partials.Administrator.SearchResult();
                result.Html = "/useradminapp/app-search.html";

                // If not authorized we don't return any results.
                if (!string.IsNullOrEmpty(query) && Admin.IsAuthorized()) {
                    result.Users = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username LIKE ? FETCH ?", "%" + query + "%", 5);
                    result.Groups = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.Name LIKE ? FETCH ?", "%" + query + "%", 5);
                }
                return result;
            });
        }
    }
}
