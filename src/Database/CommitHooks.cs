using Starcounter;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server;

namespace UserAdminApp.Database {
    public class CommitHooks {

        internal static void RegisterCommitHooks() {

            HandlerOptions opt = new HandlerOptions() { HandlerLevel = 0 };

            #region Sign in/out commit hooks
            // User signed in event
            Starcounter.Handle.POST(Admin.Port, "/__db/__" + StarcounterEnvironment.DatabaseNameLower + "/societyobjects/systemusersession", (Request request) => {

                bool isSignedIn = Admin.IsAuthorized();

                // Hide or show menu choice 
                Admin admin = Admin.AdminPage;
                if (admin.Menu != null) {
                    SystemUserMenu menu = admin.Menu as SystemUserMenu;
                    menu.IsSignedIn = isSignedIn;
                }

                //if (admin.User != null) {
                //    UserMenu user = admin.User as UserMenu;
                //    user.IsSignedIn = isSignedIn;
                //}

                return (ushort)System.Net.HttpStatusCode.OK;
            }, opt);

            // User signed out event
            Starcounter.Handle.DELETE(Admin.Port, "/__db/__" + StarcounterEnvironment.DatabaseNameLower + "/societyobjects/systemusersession", (Request request) => {

                bool isSignedIn = Admin.IsAuthorized();

                // Hide or show menu choice 
                Admin admin = Admin.AdminPage;
                if (admin.Menu != null) {
                    SystemUserMenu menu = admin.Menu as SystemUserMenu;
                    menu.IsSignedIn = isSignedIn;
                }

                //if (admin.User != null) {
                //    UserMenu user = admin.User as UserMenu;
                //    user.IsSignedIn = isSignedIn;
                //}

                return (ushort)System.Net.HttpStatusCode.OK;
            }, opt);

            #endregion

   
        }
    }
}
