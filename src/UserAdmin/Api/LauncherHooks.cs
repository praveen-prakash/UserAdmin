﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Starcounter;

namespace UserAdmin {
    public class LauncherHooks {

        static public MasterPage GetMaster() {

            return Self.GET<MasterPage>("/useradmin/standalone");
        }

        public static void Register() {

            // Workspace root (Launchpad)
            Handle.GET("/useradmin", () => {

                return Self.GET("/useradmin/admin/users");
            });

            Handle.GET("/useradmin/standalone", () => {

                Session session = Session.Current;

                if (session != null && session.Data != null) {
                    return session.Data;
                }

                MasterPage masterPage = new MasterPage();

                if (session == null) {
                    session = new Session(SessionOptions.PatchVersioning);
                    masterPage.Html = "/useradmin/viewmodels/launcher/StandalonePage.html";
                }
                else {
                    masterPage.Html = "/useradmin/viewmodels/launcher/LauncherWrapperPage.html";
                }

                masterPage.Session = session;
                return masterPage;

            });


            Handle.GET("/useradmin/app-name", () => {
                return new AppName();
            });

            // App name required for Launchpad
            Handle.GET("/useradmin/app-icon", () => {
                return new Page() { Html = "/UserAdmin/viewmodels/launcher/AppIconPage.html" };
            });

            // Menu
            Handle.GET("/useradmin/menu", () => {

                MasterPage master = GetMaster();

                master.Menu = new AdminMenu() { Html = "/UserAdmin/viewmodels/launcher/AppMenuPage.html", IsAdministrator = MasterPage.IsAdmin() };
                return master.Menu;
                //return new UserSessionPage() { Html = "/UserAdmin/viewmodels/launcher/AppMenuPage.html" };

                //return new Page() { Html = "/UserAdmin/viewmodels/launcher/AppMenuPage.html" };
                //UserSessionPage userSessionPage = new UserSessionPage();

                //var menuPage = new AdminMenu() {
                //    Html = "/UserAdmin/viewmodels/launcher/AppMenuPage.html",
                //    IsAdministrator = UserSessionPage.IsAdmin()
                //};

                //userSessionPage.Menu = menuPage;
                //userSessionPage.Session = Session.Current;

                //return menuPage;
            });

            // TODO:
            // Not sure where to put this.
            Handle.GET("/useradmin/search/{?}", (string query) => {
                var result = new SearchResultPage();
                result.Html = "/UserAdmin/viewmodels/launcher/AppSearchPage.html";

                // If not authorized we don't return any results.
                if (!string.IsNullOrEmpty(query) && MasterPage.IsAdmin()) {
                    result.Users = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.Username LIKE ? FETCH ?", "%" + query + "%", 5);
                    result.Groups = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name LIKE ? FETCH ?", "%" + query + "%", 5);
                }

                return result;
            });

            UriMapping.Map("/useradmin/menu", "/sc/mapping/menu");
            UriMapping.Map("/useradmin/app-name", "/sc/mapping/app-name");
            UriMapping.Map("/useradmin/app-icon", "/sc/mapping/app-icon");
            UriMapping.Map("/useradmin/search/@w", "/sc/mapping/search?query=@w");
        }
    }
}
