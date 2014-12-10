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
        }
    }
}
