using Starcounter;
using System;
using System.Collections;

namespace UserAdminApp.Server.Partials.Administrator {
    partial class CreateUserPage : Page {

        void Handle(Input.Save action) {

            try {
                Db.Transaction(() => {
                    Database.SystemUserAdmin.AddPerson(this.FirstName, this.Surname, this.Username, this.EMail, this.Password);
                });

                this.RedirectUrl = Admin.LauncherWorkSpacePath + "/UserAdminApp/users";
            }
            catch (Exception e) {
                this.Message = e.Message;
            }
        }

        void Handle(Input.SendInventationMail action) {
        }

        void Handle(Input.Close action) {

            this.RedirectUrl = Admin.LauncherWorkSpacePath + "/UserAdminApp/users";
        }
    }
}
