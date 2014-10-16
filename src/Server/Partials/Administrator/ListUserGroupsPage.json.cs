using Concepts.Ring1;
using Concepts.Ring3;
using Starcounter;
using System.Collections;
using UserAdminApp.Database;

namespace UserAdminApp.Server.Partials.Administrator {

    [ListUserGroupsPage_json]
    partial class ListUserGroupsPage : Page {

        public IEnumerable Groups {

            get {
                return Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o");
            }
        }
    }

    [ListUserGroupsPage_json.Items]
    partial class SystemUserGroupsItem : Json {

        public string BasedOn_ {
            get {

                if (this.Data == null) return null;

                Concepts.Ring5.SystemUserGroupBasedOn basedOn = Db.SQL<Concepts.Ring5.SystemUserGroupBasedOn>("SELECT o FROM Concepts.Ring5.SystemUserGroupBasedOn o WHERE o.SystemUserGroup=?", this.Data).First;
                if (basedOn != null) {
                    return basedOn.SystemUserGroupBaseOn.Name;
                }
                return string.Empty;
            }
        }

        void Handle(Input.Delete action) {

            // TODO: Warning user with Yes/No dialog
            Db.Transaction(() => {
                SystemUserAdmin.DeleteSystemUserGroup(this.Data as Concepts.Ring3.SystemUserGroup);
            });
        }
    }
}
