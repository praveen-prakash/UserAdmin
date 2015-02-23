using Starcounter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UserAdminApp.Database;

namespace UserAdminApp.Server.Partials.Administrator {

    [SomebodyPage_json]
    partial class SomebodyPage : Page {

        public string SelectedSystemUserGroupID_;
        public IEnumerable SystemUserGroups_ {
            get {

                List<Concepts.Ring3.SystemUserGroup> notmemberofgroups = new List<Concepts.Ring3.SystemUserGroup>();

                var groups = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o");
                foreach (var group in groups) {

                    var memberOfGroup = Db.SQL<Concepts.Ring3.SystemUserGroupMember>("SELECT o FROM Concepts.Ring3.SystemUserGroupMember o WHERE o.SystemUserGroup=? AND o.SystemUser=?", group, this.Data).First;
                    if (memberOfGroup == null) {
                        notmemberofgroups.Add(group);
                    }

                }
                return notmemberofgroups;
            }
        }

        void Handle(Input.AddUserToGroup action) {

            if (string.IsNullOrEmpty(this.SelectedSystemUserGroupID_)) {
                action.Cancel();
                // TODO: Feedback!
                return;
            }

            Concepts.Ring3.SystemUserGroup group = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.ObjectID=?", this.SelectedSystemUserGroupID_).First;

            Concepts.Ring3.SystemUserGroupMember systemUserGroupMember = new Concepts.Ring3.SystemUserGroupMember();
            systemUserGroupMember.SetSystemUser(this.Data as Concepts.Ring3.SystemUser);
            systemUserGroupMember.SetToWhat(group);

            this.SelectedSystemUserGroupID_ = null;

            this.AddUserToGroup = false;
            action.Value = false;
        }

        public bool ResetPassword_Enabled_ {
            get {
                return UserAdminApp.Database.SettingsMailServer.Settings.Enabled;
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
                        this.Transaction.Scope(() => {
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
                        this.Transaction.Scope(() => {
                            eMailAddress.Delete();
                        });
                    }
                    else {
                        // Update email
                        this.Transaction.Scope(() => {
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
            var transaction = this.Transaction;

            // TODO: Warn user with Yes/No dialog
            transaction.Rollback();

            transaction.Scope(() => {
                SystemUserAdmin.DeleteSystemUser(this.Data as Concepts.Ring3.SystemUser);
            });

            transaction.Commit();


            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/users";

            this.Delete = false;
            action.Value = false;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/users";

            this.Save = false;
            action.Value = false;
        }

        /// <summary>
        /// Close page
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/users";

            this.Close = false;
            action.Value = false;
        }

        #endregion

        public void ResetPassword() {

            string link = null;
            string fullName = string.Empty;
            Concepts.Ring2.EMailAddress eMailAddress = null;

            if (UserAdminApp.Database.SettingsMailServer.Settings.Enabled == false) {
                this.Message = "Mail Server not enabled in the settings.";
                return;
            }

            if (string.IsNullOrEmpty(UserAdminApp.Database.SettingsMailServer.Settings.SiteHost)) {
                this.Message = "Invalid settings, check site host name / port";
                return;
            }

            var transaction = this.Transaction;
            transaction.Scope(() => {

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

                uri.Host = UserAdminApp.Database.SettingsMailServer.Settings.SiteHost;
                uri.Port = (int)UserAdminApp.Database.SettingsMailServer.Settings.SitePort;

                uri.Path = "launcher/workspace/UserAdminApp/user/resetpassword";
                uri.Query = "token=" + resetPassword.Token;

                link = uri.ToString();

                eMailAddress = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.ToWhat=?", systemUser).First;
                if (eMailAddress == null) {
                    this.Message = "User dosent have any email configured";
                    return;
                }

            });

            transaction.Commit();

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

    [SomebodyPage_json.Groups]
    partial class SombodyGroupItem : Json {

        void Handle(Input.Remove action) {

            Concepts.Ring3.SystemUserGroup group = this.Data as Concepts.Ring3.SystemUserGroup;
            group.RemoveMember(this.Parent.Parent.Data as Concepts.Ring3.SystemUser);

            // Use bellow if row is not deleted completely.
            // this.Remove = false;
            // action.Value = false;
        }
    }
}
