using Concepts.Ring3;
using Starcounter;

namespace UserAdminApp.Server {

    [Admin_json]
    partial class Admin : Json {

        static void Main() {

            HandlerOptions opt = new HandlerOptions() { HandlerLevel = 0 };

            // User signed in event
            Starcounter.Handle.POST("/__db/__default/societyobjects/systemusersession", (Request request) => {

                Admin admin = Session.Current.Data as Admin;
                SystemUserMenu menu = admin.Menu as SystemUserMenu;
                menu.IsSignedIn = Admin.IsAuthorized();

                return (ushort)System.Net.HttpStatusCode.OK;
            }, opt);

            // User signed out event
            Starcounter.Handle.DELETE("/__db/__default/societyobjects/systemusersession", (Request request) => {

                Admin admin = Session.Current.Data as Admin;
                SystemUserMenu menu = admin.Menu as SystemUserMenu;
                menu.IsSignedIn = Admin.IsAuthorized();

                return (ushort)System.Net.HttpStatusCode.OK;
            }, opt);

            // Menu
            Starcounter.Handle.GET("/menu", () => {

                Admin a = new Admin();

                var menuPage = new SystemUserMenu() {
                    Html = "/adminmenu.html",
                    IsSignedIn = Admin.IsAuthorized()
                };

                a.Menu = menuPage;
                Session.Current.Data = a;

                return menuPage;
            });

            #region System Users page
            Starcounter.Handle.GET("/admin/systemusers", () => {

                if (!Admin.IsAuthorized()) {
                    var signinPage = new SignIn()
                    {
                        Html = "/useradmin-signin.html"
                    };
                    return signinPage;
                    // Response response = new Response() { StatusCode = (ushort)System.Net.HttpStatusCode.Redirect };
                    // response["Location"] = "/";
                    // return response;
                }

                SystemUsers page = new SystemUsers() {
                    Html = "/systemusers.html",
                    Uri = "/admin/systemusers"
                };
                return page;
            });

            Starcounter.Handle.GET("/admin/systemuser/{?}", (Request request, string userid) => {

                if (!Admin.IsAuthorized()) {
                    Response response = new Response() { StatusCode = (ushort)System.Net.HttpStatusCode.Redirect };
                    response["Location"] = "/";
                    return response;
                }

                Concepts.Ring3.SystemUser user = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM SystemUser o WHERE Username=?", userid).First;

                if (user == null) {
                    return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                Server.SystemUser page = new SystemUser() {
                    Html = "/systemuser.html",
                    Uri = "/admin/systemusers/"+user.Username
                };

                page.Transaction = new Transaction();
                page.Data = user;

                return page;
            });
            #endregion
        }

        static private bool IsAuthorized() {

            SignInApp.Database.SystemUserSession userSession = Db.SQL<SignInApp.Database.SystemUserSession>("SELECT o FROM SignInApp.Database.SystemUserSession o WHERE o.SessionIdString=?", Session.Current.SessionIdString).First;
            return userSession != null;
        }

        #region Base
        // Browsers will ask for "text/html" and we will give it to them
        // by loading the contents of the URI in our Html property
        public override string AsMimeType(MimeType type) {
            if (type == MimeType.Text_Html) {
                return X.GET<string>(Html);
            }
            return base.AsMimeType(type);
        }

        /// <summary>
        /// The way to get a URL for HTML partial if any.
        /// </summary>
        /// <returns></returns>
        public override string GetHtmlPartialUrl() {
            return Html;
        }

        /// <summary>
        /// Whenever we set a bound data object to this page, we update the
        /// URI property on this page.
        /// </summary>
        protected override void OnData() {
            base.OnData();
            var str = "";
            Json x = this;
            while (x != null) {
                if (x is Admin)
                    str = (x as Admin).UriFragment + str;
                x = x.Parent;
            }
            Uri = str;
        }

        /// <summary>
        /// Override to provide an URI fragment
        /// </summary>
        /// <returns></returns>
        protected virtual string UriFragment {
            get {
                return "";
            }
        }
        #endregion
    }
}
