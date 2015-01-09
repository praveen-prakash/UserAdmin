﻿using Starcounter;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server;
using UserAdminApp.Server.Partials.Launcher;

namespace UserAdminApp.Database {
    public class CommitHooks {

        internal static void Register() {

            #region Sign in/out commit hooks
            // User signed in event
            Starcounter.Handle.POST( "/__db/__" + StarcounterEnvironment.DatabaseNameLower + "/societyobjects/systemusersession", (Request request) => {

                string sessionID = Session.Current.SessionIdString;
                if (!Program.Sessions.ContainsKey(sessionID)) {
                    return new Response() { StatusCode = (ushort)System.Net.HttpStatusCode.InternalServerError, Body = "Failed to get the signin app Session" };
                }
                UserSession admin = Program.Sessions[sessionID];

                bool isAuthorized = UserSession.IsAdmin();

                // Hide or show menu choice 
                //Admin admin = Admin.AdminPage;
                if (admin.Menu != null) {
                    AdminMenu menu = admin.Menu as AdminMenu;
                    menu.IsAdministrator = isAuthorized;
                }

                return (ushort)System.Net.HttpStatusCode.OK;
            });

            // User signed out event
            Starcounter.Handle.DELETE( "/__db/__" + StarcounterEnvironment.DatabaseNameLower + "/societyobjects/systemusersession", (Request request) => {

                bool isAuthorized = UserSession.IsAdmin();

                string sessionID = Session.Current.SessionIdString;
                if (!Program.Sessions.ContainsKey(sessionID)) {
                    return new Response() { StatusCode = (ushort)System.Net.HttpStatusCode.InternalServerError, Body = "Failed to get the signin app Session" };
                }
                UserSession admin = Program.Sessions[sessionID];

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
  
        }
    }
}
