using Starcounter;
using System;
using System.Collections;

namespace UserAdminApp.Server.Partials.Administrator {
    partial class CreateUserGroupPage : Page {

        void Handle(Input.Save action) {

            try {
                Db.Transaction(() => {
                    Concepts.Ring3.SystemUserGroup group = new Concepts.Ring3.SystemUserGroup();
                    group.Name = this.Name;
                });
                this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/usergroups";

            }
            catch (Exception e) {
                this.Message = e.Message;
            }
        }


        void Handle(Input.Close action) {

            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/usergroups";
        }
    }
}
