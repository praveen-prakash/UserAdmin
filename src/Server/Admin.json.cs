using Concepts.Ring3;
using Starcounter;
using System.Web;
using UserAdminApp.Database;
using UserAdminApp.Server.Partials;

namespace UserAdminApp.Server {

    [Admin_json]
    partial class Admin : Page {

        // TODO: How to remove items from this list
        internal static Admin AdminPage;

        internal object Menu;

        static public ushort Port = 8080;

        static void Main() {

            Admin.AssureOneSystemUser();

            Handlers.Administrator.RegisterHandlers();
            Handlers.Systemusers.RegisterHandlers();
            Database.CommitHooks.RegisterCommitHooks();
            Handlers.LauncherHooks.RegisterLauncherHooks();
        }

        static private void AssureOneSystemUser() {

            Concepts.Ring3.SystemUser systemUser = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o").First;
            if (systemUser == null) {
                Db.Transaction(() => {
                    SystemUserAdmin.AddPerson("admin", "admin", "admin", "change@this.email", "admin");
                });
            }
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
