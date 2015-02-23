using Starcounter;
using System;
using UserAdminApp.Database;

namespace UserAdminApp.Server.Partials.User {

    [ResetPasswordPage_json]
    partial class ResetPasswordPage : Page {

        internal ResetPassword resetPassword;

        void Handle(Input.Password action) {
        }

        void Handle(Input.ConfirmPassword action) {

            CheckPasswordMatch(this.Password, action.Value);
        }

        void Handle(Input.Save action) {

            if (!CheckPasswordMatch(this.Password, this.ConfirmPassword)) {
                return;
            }

            if (this.resetPassword == null) {
                this.Message = "Reset token already used";
                return;
            }

            if (this.resetPassword.Expire < DateTime.UtcNow) {
                this.Message = "Reset token expired";
                // TODO: redirect?
                return;
            }

            if (resetPassword.User == null) {
                this.Message = "Failed to get the user"; // TODO: Better message
                return;
            }

            string userID = resetPassword.User.GetObjectID();

            Db.Transact(() => {

                SystemUserAdmin.SetPassword(resetPassword.User, this.Password);
                // Remove resetPassord instance
                resetPassword.Delete();
            });

            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/users/" + userID;

            this.Save = false;
            action.Value = false;
        }

        private bool CheckPasswordMatch(string pw1, string pw2) {
            if (pw1 != pw2) {
                this.Message = "Password missmatch";
                return false;
            }
            this.Message = string.Empty;
            return true;
        }
    }
}
