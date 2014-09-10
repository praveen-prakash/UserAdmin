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

            // TODO: Check for duplicates

            this.RedirectUrl = "/launcher/workspace/admin/createuser";

            //Database.SystemUserAdmin.AddPerson(this.FirstName, this.Surname, this.EMail, this.Password);

            //Db.Transaction(() => {

                
            //    Concepts.Ring3.SystemUser user = new Concepts.Ring3.SystemUser();
            //    user.Username = this.EMail;
            //    user.Password = this.Password;
            //});

            //this.EMail = this.Password = string.Empty;
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

            Db.Transaction(() => {

                this.Data.Delete();
            });
        }
    }
}
