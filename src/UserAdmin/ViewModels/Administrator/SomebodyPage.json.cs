using Simplified.Ring3;
using Simplified.Ring6;
using Smorgasbord.PropertyMetadata;
using Starcounter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace UserAdmin {

    [SomebodyPage_json]
    partial class SomebodyPage : PropertyMetadataPage, IBound<Simplified.Ring3.SystemUser> {

        public string SelectedSystemUserGroupID_;
        public IEnumerable SystemUserGroups_ {

            get {

                List<Simplified.Ring3.SystemUserGroup> notmemberofgroups = new List<Simplified.Ring3.SystemUserGroup>();

                var groups = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o");
                foreach (var group in groups) {

                    var memberOfGroup = Db.SQL<Simplified.Ring3.SystemUserGroupMember>("SELECT o FROM Simplified.Ring3.SystemUserGroupMember o WHERE o.SystemUserGroup=? AND o.SystemUser=?", group, this.Data).First;
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

            Simplified.Ring3.SystemUserGroup group = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.ObjectID=?", this.SelectedSystemUserGroupID_).First;

            Simplified.Ring3.SystemUserGroupMember systemUserGroupMember = new Simplified.Ring3.SystemUserGroupMember();

            systemUserGroupMember.WhatIs = this.Data as Simplified.Ring3.SystemUser;
            systemUserGroupMember.ToWhat = group;

            this.SelectedSystemUserGroupID_ = null;
        }

        public bool ResetPassword_Enabled_ {

            get {
                return UserAdmin.SettingsMailServer.Settings.Enabled && Utils.IsValidEmail(this.Data.Username);
            }
        }

        /// <summary>
        /// EMailAddress
        /// </summary>
        //private Simplified.Ring3.EmailAddress EMailAddress {
        //    get {
        //        Simplified.Ring3.SystemUser user = this.Data as Simplified.Ring3.SystemUser;
        //        if (user == null) return null;
        //        return Db.SQL<Simplified.Ring3.EmailAddress>("SELECT o FROM Simplified.Ring3.EmailAddress o WHERE o.ToWhat=?", user).First;
        //    }
        //}

        #region View-model Handlers

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.ResetPassword action) {

            this.ResetUserPassword();
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Delete action) {

            var transaction = this.Transaction;

            SystemUser systemUser = Helper.GetCurrentSystemUser();
            if (systemUser.Equals(this.Data)) {
                // TODO: Show error message "Can not delete yourself"
                return;
            }

            // TODO: Warn user with Yes/No dialog
            transaction.Rollback();

            transaction.Scope(() => {
                SystemUserAdmin.DeleteSystemUser(this.Data as Simplified.Ring3.SystemUser);
            });

            transaction.Commit();

            this.RedirectUrl = "/UserAdmin/admin/users";
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Save action) {

            this.AssurePropertyFeedbacks();

            if (this.IsInvalid) {
                return;
            }

            if (this.Transaction.IsDirty) {
                this.Transaction.Commit();
            }

            this.RedirectUrl = "/UserAdmin/admin/users";
        }

        /// <summary>
        /// Close page
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Close action) {

            if (this.Transaction.IsDirty) {
                this.Transaction.Rollback();
            }

            this.RedirectUrl = "/UserAdmin/admin/users";
        }

        #endregion

        public void ResetUserPassword() {

            string link = null;
            string fullName = string.Empty;
            string email = string.Empty;
            //Simplified.Ring3.EmailAddress eMailAddress = null;

            if (UserAdmin.SettingsMailServer.Settings.Enabled == false) {
                this.Message = "Mail Server not enabled in the settings.";
                return;
            }

            if (string.IsNullOrEmpty(UserAdmin.SettingsMailServer.Settings.SiteHost)) {
                this.Message = "Invalid settings, check site host name / port";
                return;
            }

            if (!Utils.IsValidEmail(this.Data.Username)) {
                this.Message = "Username is not an email address";
                return;
            }

            email = this.Data.Username;

            var transaction = this.Transaction;
            transaction.Scope(() => {

                Simplified.Ring3.SystemUser systemUser = this.Data;
                // Generate Password Reset token
                ResetPassword resetPassword = new ResetPassword() {
                    User = systemUser,
                    Token = HttpUtility.UrlEncode(Guid.NewGuid().ToString()),
                    Expire = DateTime.UtcNow.AddMinutes(1440)
                };

                // Get FullName
                if (systemUser.WhoIs != null) {
                    fullName = systemUser.WhoIs.FullName;
                } else {
                    fullName = systemUser.Username;
                }

                // Build reset password link
                UriBuilder uri = new UriBuilder();

                uri.Host = UserAdmin.SettingsMailServer.Settings.SiteHost;
                uri.Port = (int)UserAdmin.SettingsMailServer.Settings.SitePort;

                uri.Path = "UserAdmin/user/resetpassword";
                uri.Query = "token=" + resetPassword.Token;

                link = uri.ToString();

                //eMailAddress = Db.SQL<Simplified.Ring3.EmailAddress>("SELECT o FROM Simplified.Ring3.EmailAddress o WHERE o.ToWhat=?", systemUser).First;
                //if (eMailAddress == null) {
                //    this.Message = "User dosent have any email configured";
                //    return;
                //}

            });

            transaction.Commit();

            try {
                this.Message = string.Format("Sending mail sent to {0}...", email);
                Utils.sendResetPasswordMail(fullName, email, link);
                this.Message = "Mail sent.";
            } catch (Exception e) {
                this.Message = e.Message;
            }
        }

        virtual protected void AssurePropertyFeedbacks() {

        }

    }

    // TODO: AWA
    //[SomebodyPage_json.Groups]
    //partial class SombodyGroupItem : Json {

    //    void Handle(Input.Remove action) {

    //        Simplified.Ring3.SystemUserGroup group = this.Data as Simplified.Ring3.SystemUserGroup;
    //        Simplified.Ring3.SystemUser user = this.Parent.Parent.Data as Simplified.Ring3.SystemUser;
    //        var removeGroup = Db.SQL<Simplified.Ring3.SystemUserGroupMember>("SELECT o FROM Simplified.Ring3.SystemUserGroupMember o WHERE o.WhatIs=? AND o.ToWhat=?", user, group).First;
    //        if (removeGroup != null) {
    //            removeGroup.Delete();
    //        }
    //    }
    //}
}
