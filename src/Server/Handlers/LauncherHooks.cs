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
    public class LauncherHooks {

        public static void RegisterLauncherHooks() {

            #region Launcher hooks
            //Starcounter.Handle.GET("/user", () => {

            //    //                Admin admin = Session.Current.Data as Admin;
            //    Admin admin = Admin.AdminPage;

            //    var userMenu = new UserMenu() {
            //        Html = "/usermenu.html",
            //    };

            //    userMenu.IsSignedIn = Admin.IsAuthorized();

            //    admin.User = userMenu;


            //    return userMenu;
            //});


            // Menu
            Starcounter.Handle.GET(Admin.Port, "/menu", () => {

                Admin adminPage = new Admin();

                var menuPage = new SystemUserMenu() {
                    Html = "/adminmenu.html",
                    IsSignedIn = Admin.IsAuthorized()
                };

                adminPage.Menu = menuPage;

                Admin.AdminPage = adminPage;

                return menuPage;
            });

            #endregion

        }
    }
}
