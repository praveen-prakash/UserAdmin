using Starcounter;
using System;
using System.Collections;

namespace UserAdminApp.Server.Partials.Administrator {
    partial class CreateUserGroupPage : Page {

        void Handle(Input.Save action) {

            try {
                Db.Transact(() => {
                    Simplified.Ring3.SystemUserGroup group = new Simplified.Ring3.SystemUserGroup();
                    group.Name = this.Name;
                });
                this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/usergroups";

            }
            catch (Exception e) {
                this.Message = e.Message;
            }
            //this.Save = false;
            //action.Value = false;
        }


        void Handle(Input.Close action) {

            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/usergroups";
            //this.Close = false;
            //action.Value = false;
        }
    }
}
