using Starcounter;
using System;
using System.Collections;
using System.Net;
using UserAdminApp.Database;

namespace UserAdminApp.Server.Partials.Administrator {

    [SomebodyPage_json]
    partial class SomebodyPage : Page {


        public bool ResetPassword_Enabled_ {
            get {
                return SettingsMailServer.Settings.Enabled;
            }
        }

        /// <summary>
        /// EMailAddress
        /// </summary>
        private Concepts.Ring2.EMailAddress EMailAddress {
            get {
                Concepts.Ring3.SystemUser user = this.Data as Concepts.Ring3.SystemUser;
                if (user == null) return null;
                return Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.ToWhat=?", user).First;
            }
        }

        /// <summary>
        /// UserEmail
        /// </summary>
        public string UserEmail {
            get {

                Concepts.Ring3.SystemUser systemUser = this.Data as Concepts.Ring3.SystemUser;
                if (systemUser == null) return string.Empty;

                if (this.EMailAddress == null) return string.Empty;
                return this.EMailAddress.EMail;
            }
            set {
                Concepts.Ring3.SystemUser systemUser = this.Data as Concepts.Ring3.SystemUser;
                if (systemUser == null) return;

                Concepts.Ring2.EMailAddress eMailAddress = this.EMailAddress;
                if (eMailAddress == null) {
                    if (!string.IsNullOrEmpty(value)) {
                        // Create email
                        this.Transaction.Add(() => {
                            Concepts.Ring2.EMailAddress emailRel = new Concepts.Ring2.EMailAddress();
                            emailRel.SetToWhat(systemUser);
                            emailRel.EMail = value.ToLowerInvariant();

                            // Update Gravatar image
                            if (systemUser.WhoIs is Concepts.Ring1.Somebody) {
                                ((Concepts.Ring1.Somebody)systemUser.WhoIs).ImageURL = UserAdminApp.Database.Utils.GetGravatarUrl(emailRel.EMail);
                                //this.ImageURL = ((Concepts.Ring1.Somebody)systemUser.WhoIs).ImageURL;
                            }

                        });
                    }
                }
                else {
                    if (string.IsNullOrEmpty(value)) {
                        // Delete email
                        this.Transaction.Add(() => {
                            eMailAddress.Delete();
                        });
                    }
                    else {
                        // Update email
                        this.Transaction.Add(() => {
                            eMailAddress.EMail = value.ToLowerInvariant();

                            // Update Gravatar image
                            if (systemUser.WhoIs is Concepts.Ring1.Somebody) {
                                ((Concepts.Ring1.Somebody)systemUser.WhoIs).ImageURL = UserAdminApp.Database.Utils.GetGravatarUrl(eMailAddress.EMail);
                                //this.ImageURL = ((Concepts.Ring1.Somebody)systemUser.WhoIs).ImageURL;
                            }
                        });
                    }
                }
            }
        }

        #region View-model Handlers

        void Handle(Input.EMail action) {

            if (!UserAdminApp.Database.Utils.IsValidEmail(action.Value)) {
                this.AddPropertyFeedback("EMail_Feedback", PropertyFeedback.PropertyFeedbackType.Error, "Invalid e-mail adress");
            }
            else {
                this.RemovePropertyFeedback("EMail_Feedback");
            }
        }


        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.ResetPassword action) {

            this.ResetPassword();
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Delete action) {

            // TODO: Warn user with Yes/No dialog
            this.Transaction.Rollback();

            SystemUserAdmin.DeleteSystemUser(this.Data as Concepts.Ring3.SystemUser);

            this.RedirectUrl = "/launcher/workspace/admin/users";
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = "/launcher/workspace/admin/users";
        }

        /// <summary>
        /// Close page
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = "/launcher/workspace/admin/users";
        }

        #endregion

        public void ResetPassword() {

            string link = null;
            string fullName = string.Empty;
            Concepts.Ring2.EMailAddress eMailAddress = null;

            if (SettingsMailServer.Settings.Enabled == false) {
                this.Message = "Mail Server not enabled in the settings.";
                return;
            }

            this.Transaction.Commit(); // TODO: Make this action disabled it user data has been changed.

            Db.Transaction(() => {

                Concepts.Ring3.SystemUser systemUser = (Concepts.Ring3.SystemUser)this.Data;
                // Generate Password Reset token
                ResetPassword resetPassword = new ResetPassword(systemUser);

                // Get FullName
                if (systemUser.WhoIs != null) {
                    fullName = systemUser.WhoIs.FullName;
                }
                else {
                    fullName = systemUser.Username;
                }

                // Build reset password link
                UriBuilder uri = new UriBuilder();
                uri.Host = Dns.GetHostEntry(String.Empty).HostName;
                uri.Port = Admin.Port;
                uri.Path = "launcher/workspace/admin/user/resetpassword";
                uri.Query = "token=" + resetPassword.Token;

                link = uri.ToString();

                eMailAddress = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.ToWhat=?", systemUser).First;
                if (eMailAddress == null) {
                    this.Message = "User dosent have any email configured";
                    return;
                }

            });

            try {
                this.Message = string.Format("Sending mail sent to {0}...", eMailAddress.EMail);
                Utils.sendResetPasswordMail(fullName, eMailAddress.EMail, link);
                this.Message = "Mail sent.";
            }
            catch (Exception e) {
                this.Message = e.Message;
            }
        }

    }
}
