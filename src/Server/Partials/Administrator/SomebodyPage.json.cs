using Simplified.Ring3;
using Simplified.Ring6;
using Starcounter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using UserAdminApp.Database;

namespace UserAdminApp.Server.Partials.Administrator {

    [SomebodyPage_json]
    partial class SomebodyPage : Page {

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
            //systemUserGroupMember.SetSystemUser(this.Data as Simplified.Ring3.SystemUser);
            //systemUserGroupMember.SetToWhat(group);

            this.SelectedSystemUserGroupID_ = null;

//            this.AddUserToGroup = false;
//            action.Value = false;
        }

        public bool ResetPassword_Enabled_ {
            get {
                return UserAdminApp.Database.SettingsMailServer.Settings.Enabled;
            }
        }

        /// <summary>
        /// EMailAddress
        /// </summary>
        private Simplified.Ring3.EmailAddress EMailAddress {
            get {
                Simplified.Ring3.SystemUser user = this.Data as Simplified.Ring3.SystemUser;
                if (user == null) return null;
                return Db.SQL<Simplified.Ring3.EmailAddress>("SELECT o FROM Simplified.Ring3.EmailAddress o WHERE o.ToWhat=?", user).First;
            }
        }

        /// <summary>
        /// UserEmail
        /// 
        ///"$EMail$" : { "Bind" : "UserEmail" },
        ///"EMail$" : "",
        ///"EMail$_Feedback" : {},
        /// </summary>
        //public string UserEmail {
        //    get {
        //        Simplified.Ring3.SystemUser systemUser = this.Data as Simplified.Ring3.SystemUser;
        //        if (systemUser == null) return string.Empty;

        //        if (this.EMailAddress == null) return string.Empty;
        //        return this.EMailAddress.EMail;
        //    }
        //    set {
        //        Simplified.Ring3.SystemUser systemUser = this.Data as Simplified.Ring3.SystemUser;
        //        if (systemUser == null) return;

        //        Simplified.Ring3.EmailAddress eMailAddress = this.EMailAddress;
        //        if (eMailAddress == null) {
        //            if (!string.IsNullOrEmpty(value)) {
        //                // Create email
        //                this.Transaction.Scope(() => {
        //                    Simplified.Ring3.EmailAddress emailRel = new Simplified.Ring3.EmailAddress();
        //                    emailRel.SetToWhat(systemUser);
        //                    emailRel.EMail = value.ToLowerInvariant();

        //                    // Update Gravatar image
        //                    if (systemUser.WhoIs is Simplified.Ring1.Somebody) {
        //                        ((Simplified.Ring1.Somebody)systemUser.WhoIs).ImageURL = UserAdminApp.Database.Utils.GetGravatarUrl(emailRel.EMail);
        //                        //this.ImageURL = ((Simplified.Ring1.Somebody)systemUser.WhoIs).ImageURL;
        //                    }

        //                });
        //            }
        //        }
        //        else {
        //            if (string.IsNullOrEmpty(value)) {
        //                // Delete email
        //                this.Transaction.Scope(() => {
        //                    eMailAddress.Delete();
        //                });
        //            }
        //            else {
        //                // Update email
        //                this.Transaction.Scope(() => {
        //                    eMailAddress.EMail = value.ToLowerInvariant();

        //                    // Update Gravatar image
        //                    if (systemUser.WhoIs is Simplified.Ring1.Somebody) {
        //                        ((Simplified.Ring1.Somebody)systemUser.WhoIs).ImageURL = UserAdminApp.Database.Utils.GetGravatarUrl(eMailAddress.EMail);
        //                        //this.ImageURL = ((Simplified.Ring1.Somebody)systemUser.WhoIs).ImageURL;
        //                    }
        //                });
        //            }
        //        }
        //    }
        //}

        #region View-model Handlers

        //void Handle(Input.EMail action) {

        //    if (!UserAdminApp.Database.Utils.IsValidEmail(action.Value)) {
        //        this.AddPropertyFeedback("EMail_Feedback", PropertyFeedback.PropertyFeedbackType.Error, "Invalid e-mail adress");
        //    }
        //    else {
        //        this.RemovePropertyFeedback("EMail_Feedback");
        //    }
        //}


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

            SystemUser systemUser = Helper.GetCurrentSystemUser();
            if (systemUser.Equals (this.Data)) {
                // TODO: Show error message "Can not delete yourself"
                return;
            }

            // TODO: Warn user with Yes/No dialog
            transaction.Rollback();

            transaction.Scope(() => {
                SystemUserAdmin.DeleteSystemUser(this.Data as Simplified.Ring3.SystemUser);
            });

            transaction.Commit();


            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/users";

//            this.Delete = false;
//            action.Value = false;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/users";

            //this.Save = false;
            //action.Value = false;
        }

        /// <summary>
        /// Close page
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/users";

            //this.Close = false;
            //action.Value = false;
        }

        #endregion

        public void ResetPassword() {

            string link = null;
            string fullName = string.Empty;
            Simplified.Ring3.EmailAddress eMailAddress = null;

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

                Simplified.Ring3.SystemUser systemUser = (Simplified.Ring3.SystemUser)this.Data;
                // Generate Password Reset token
                ResetPassword resetPassword = new ResetPassword() {
                    User = systemUser,
                    Token = HttpUtility.UrlEncode(Guid.NewGuid().ToString()),
                    Expire = DateTime.UtcNow.AddMinutes(1440)
                };





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

                eMailAddress = Db.SQL<Simplified.Ring3.EmailAddress>("SELECT o FROM Simplified.Ring3.EmailAddress o WHERE o.ToWhat=?", systemUser).First;
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

            Simplified.Ring3.SystemUserGroup group = this.Data as Simplified.Ring3.SystemUserGroup;
            Simplified.Ring3.SystemUser user = this.Parent.Parent.Data as Simplified.Ring3.SystemUser;
            var removeGroup = Db.SQL<Simplified.Ring3.SystemUserGroupMember>("SELECT o FROM Simplified.Ring3.SystemUserGroupMember o WHERE o.WhatIs=? AND o.ToWhat=?", user, group).First;
            if (removeGroup != null) {
                removeGroup.Delete();
            }
            //group.RemoveMember(this.Parent.Parent.Data as Simplified.Ring3.SystemUser);

            // Use bellow if row is not deleted completely.
            // this.Remove = false;
            // action.Value = false;
        }
    }
}
