using Starcounter;
using System.Collections;

namespace UserAdminApp.Server.Partials {
    partial class SystemUserPage : Page {

        protected override void OnData() {
            base.OnData();
        }

        public string Type_ {
            get {

                Concepts.Ring3.SystemUser user = this.Data as Concepts.Ring3.SystemUser;
                if (user != null) {
                    if (user.WhoIs != null) {
                        return user.WhoIs.GetType().Name;
                    }
                    else {
                        return user.GetType().Name;
                    }
                }
                return null;
            }
        }

        void Handle(Input.Delete action) {

            // TODO: Warn user with Yes/No dialog

            Db.Transaction(() => {
                this.Data.Delete();
            });

            this.RedirectUrl = "/launcher/workspace/admin/systemusers";

        }

        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = "/launcher/workspace/admin/systemusers";
        }

        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = "/launcher/workspace/admin/systemusers";
        }
    }
}
