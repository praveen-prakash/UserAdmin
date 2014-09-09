using Starcounter;
using System.Web;

namespace UserAdminApp.Server {

    [SystemUserRegister_json]
    partial class SystemUserRegister : Page {

  
        void Handle(Input.Save action) {

            Concepts.Ring3.SystemUser user = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", this.Username).First;
            if (user != null) {
                // Username taken
                this.Message = "Username taken";
                return;
            }

            // TODO Check for duplicated users
            Db.Transaction(() => {
                user = new Concepts.Ring3.SystemUser();
                user.Username = this.Username;
                user.Password = this.Password;
            });


            // TODO: Get Current url
            string query = HttpUtility.UrlEncode("originurl" + "=" + "/");
            this.RedirectUrl = "/launcher/workspace/signinapp/signinuser?" + query;
        }

        void Handle(Input.Cancel action) {

            // TODO: Get Current url
            string query = HttpUtility.UrlEncode("originurl" + "=" + "/");
            this.RedirectUrl = "/launcher/workspace/signinapp/signinuser?" + query;
        }

    }
}
