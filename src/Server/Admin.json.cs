using Concepts.Ring3;
using Starcounter;
using System.Web;

namespace UserAdminApp.Server {

    [Admin_json]
    partial class Admin : Json {

        static void Main() {

            HandlerOptions opt = new HandlerOptions() { HandlerLevel = 0 };

            #region Sign in/out event
            // User signed in event
            Starcounter.Handle.POST("/__db/__default/societyobjects/systemusersession", (Request request) => {

                // Hide or show menu choice 
                Admin admin = Session.Current.Data as Admin;
                SystemUserMenu menu = admin.Menu as SystemUserMenu;
                menu.IsSignedIn = Admin.IsAuthorized();

                return (ushort)System.Net.HttpStatusCode.OK;
            }, opt);

            // User signed out event
            Starcounter.Handle.DELETE("/__db/__default/societyobjects/systemusersession", (Request request) => {

                // Hide or show menu choice 
                Admin admin = Session.Current.Data as Admin;
                SystemUserMenu menu = admin.Menu as SystemUserMenu;
                menu.IsSignedIn = Admin.IsAuthorized();

                return (ushort)System.Net.HttpStatusCode.OK;
            }, opt);

            #endregion

            // Menu
            Starcounter.Handle.GET("/menu", () => {

                Admin adminPage = new Admin();

                var menuPage = new SystemUserMenu() {
                    Html = "/adminmenu.html",
                    IsSignedIn = Admin.IsAuthorized()
                };

                adminPage.Menu = menuPage;
                Session.Current.Data = adminPage;
                return menuPage;
            });

            #region System Users page

            Starcounter.Handle.GET("/admin/systemusers", () => {

                if (!Admin.IsAuthorized()) {
                    return GetSignInPage("/launcher/workspace/admin/systemusers");
                }

                SystemUsers page = new SystemUsers() {
                    Html = "/systemusers.html",
                    Uri = "/admin/systemusers"
                };
                return page;
            });

            Starcounter.Handle.GET("/admin/systemusers/{?}", (Request request, string userid) => {

                if (!Admin.IsAuthorized()) {
                    return GetSignInPage("/launcher/workspace/admin/systemusers/" + userid);
                }

                Concepts.Ring3.SystemUser user = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", userid).First;

                if (user == null) {
                    // TODO: Return a "User not found" page
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Server.SystemUser page = new SystemUser() {
                    Html = "/systemuser.html",
                    Uri = "/admin/systemusers/" + user.Username
                };

                page.Transaction = new Transaction();   // TODO: How to close this transaction if the user do a refresh in the browser?
                page.Data = user;

                return page;
            });
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        static private SignIn GetSignInPage(string referer) {

            string query = HttpUtility.UrlEncode("originurl" + "=" + referer);

            var signinPage = new SignIn() {
                Html = "/useradmin-signin.html",
                RedirectUrl = "/launcher/workspace/signinapp/signinuser?" + query
            };
            return signinPage;
        }

        /// <summary>
        /// Check if user is Authorized
        /// </summary>
        /// <returns></returns>
        static private bool IsAuthorized() {

            Concepts.Ring5.SystemUserSession userSession = Db.SQL<Concepts.Ring5.SystemUserSession>("SELECT o FROM Concepts.Ring5.SystemUserSession o WHERE o.SessionIdString=?", Session.Current.SessionIdString).First;

            if (userSession != null && userSession.User == null) {
                // system user has been deleted
                return false;
            }

            // Here we can do more permission checking..

            return userSession != null;
        }

       #region Base

        /// <summary>
        /// The way to get a URL for HTML partial if any.
        /// </summary>
        /// <returns></returns>
        public override string GetHtmlPartialUrl() {
            return Html;
        }

        #endregion
    }
}
