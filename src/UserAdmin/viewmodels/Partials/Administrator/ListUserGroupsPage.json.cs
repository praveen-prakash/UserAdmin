using Starcounter;
using System.Collections;

namespace UserAdmin.Partials.Administrator {

    [ListUserGroupsPage_json]
    partial class ListUserGroupsPage : Page {

        public IEnumerable Groups {

            get {
                return Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o");
            }
        }
    }

    [ListUserGroupsPage_json.Items]
    partial class SystemUserGroupsItem : Json, IBound<Simplified.Ring3.SystemUserGroup> {

        public Simplified.Ring3.SystemUserGroup BasedOn_ {
            get {
                if (this.Data == null || this.Data.Parent == null) return null;

                return Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o=?",this.Data.Parent).First;
            }
        }

        void Handle(Input.Delete action) {

            // TODO: Warning user with Yes/No dialog
            Db.Transact(() => {
                SystemUserAdmin.DeleteSystemUserGroup(this.Data as Simplified.Ring3.SystemUserGroup);
            });
        }
    }
}
