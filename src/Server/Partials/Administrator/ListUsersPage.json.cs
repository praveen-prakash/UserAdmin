using Concepts.Ring1;
using Concepts.Ring3;
using Starcounter;
using System.Collections;
using UserAdminApp.Database;

namespace UserAdminApp.Server.Partials.Administrator {

    [ListUsersPage_json]
    partial class ListUsersPage : Page {

        public IEnumerable Users {

            get {
                return Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o");
            }
        }


        void Handle(Input.AddSystemUser action) {

            this.RedirectUrl = "/launcher/workspace/admin/createuser";
        }

    }

    [ListUsersPage_json.Items]
    partial class SystemUsersItem : Json {

        protected override void OnData() {
            base.OnData();
        }

        public string Kind {
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

            // TODO: Warning user with Yes/No dialog
            SystemUserAdmin.DeleteSystemUser(this.Data as Concepts.Ring3.SystemUser);
        }
    }
}
