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
    partial class SystemUserGroupsItem : Json, IBound<Concepts.Ring3.SystemUserGroup> {

        public Concepts.Ring3.SystemUserGroup BasedOn_ {
            get {
                if (this.Data == null || this.Data.Parent == null) return null;

                return Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o=?",this.Data.Parent).First;
            }
        }

        void Handle(Input.Delete action) {

            // TODO: Warning user with Yes/No dialog
            Db.Transact(() => {
                SystemUserAdmin.DeleteSystemUserGroup(this.Data as Concepts.Ring3.SystemUserGroup);
            });
            // Use bellow if row is not deleted completely.
            // this.Delete = false;
            // action.Value = false;
        }
    }
}
