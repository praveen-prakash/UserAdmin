﻿using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server.Partials;

namespace UserAdminApp.Server.Handlers {
    public class LauncherHooks {

        public static void RegisterLauncherHooks() {

            // Menu
            Starcounter.Handle.GET(Admin.Port, "/menu", () => {

                UserAdminApp.Server.Admin adminPage = new UserAdminApp.Server.Admin();

                var menuPage = new UserAdminApp.Server.SystemUserMenu() {
                    Html = "/adminmenu.html",
                    IsSignedIn = Admin.IsAuthorized()
                };

                adminPage.Menu = menuPage;
                //adminPage.Menu = menuPage;
                Admin.AdminPage = adminPage;
                return menuPage;
            });
        }
    }
}
