using Concepts.Ring3;
using Starcounter;
using System.Web;
using UserAdminApp.Server.Partials;

namespace UserAdminApp.Server {

    [Admin_json]
    partial class Admin : Page {

        // TODO: How to remove items from this list
        static Admin AdminPage;

        static void Main() {

            HandlerOptions opt = new HandlerOptions() { HandlerLevel = 0 };

            #region Sign in/out commit hooks
            // User signed in event
            Starcounter.Handle.POST("/__db/__default/societyobjects/systemusersession", (Request request) => {

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
            Starcounter.Handle.DELETE("/__db/__default/societyobjects/systemusersession", (Request request) => {

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
            Starcounter.Handle.GET("/menu", () => {

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

            Handlers.Systemusers.RegisterHandlers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        static public UserAdminSignInPage GetSignInPage(string referer) {

            string query = HttpUtility.UrlEncode("originurl" + "=" + referer);

            var signinPage = new UserAdminSignInPage() {
                Html = "/useradmin-signin.html",
                RedirectUrl = "/launcher/workspace/signinapp/signinuser?" + query
            };
            return signinPage;
        }

        /// <summary>
        /// Check if user is Authorized
        /// </summary>
        /// <returns></returns>
        static public bool IsAuthorized() {

            Concepts.Ring5.SystemUserSession userSession = Db.SQL<Concepts.Ring5.SystemUserSession>("SELECT o FROM Concepts.Ring5.SystemUserSession o WHERE o.SessionIdString=?", Session.Current.SessionIdString).First;

            if (userSession != null && userSession.Token.User == null) {

                // system user has been deleted
                return false;
            }

            // Here we can do more permission checking..

            return userSession != null;
        }
    }
}
