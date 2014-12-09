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

        public Concepts.Ring8.Polyjuice.SystemUserGroupBasedOn BasedOn_ {
            get {
                return Db.SQL<Concepts.Ring8.Polyjuice.SystemUserGroupBasedOn>("SELECT o FROM Concepts.Ring8.Polyjuice.SystemUserGroupBasedOn o WHERE o.SystemUserGroup=?", this.Data).First;
            }
        }

        //public string BasedOn_ {
        //    get {

        //        if (this.Data == null) return null;
        //        if (this.BasedOnGroup != null) {
        //            return this.BasedOnGroup.SystemUserGroupBaseOn.Name;
        //        }
        //        return string.Empty;
        //    }
        //}

        //public string BasedOnID_ {
        //    get {
        //        Concepts.Ring8.Polyjuice.SystemUserGroupBasedOn basedOn = this.BasedOn_;
        //        if (basedOn != null) {
        //            basedOn.GetObjectID();
        //        }
                    
        //            return string.Empty;



        //    }
        //}


        void Handle(Input.Delete action) {

            // TODO: Warning user with Yes/No dialog
            Db.Transaction(() => {
                SystemUserAdmin.DeleteSystemUserGroup(this.Data as Concepts.Ring3.SystemUserGroup);
            });
        }
    }
}
