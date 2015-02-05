using Starcounter;
using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Web;
using UserAdminApp.Database;

// FORGOT PASSWORD:
// http://www.asp.net/identity/overview/features-api/account-confirmation-and-password-recovery-with-aspnet-identity

namespace UserAdminApp.Server.Partials.Administrator {

    [EditUserGroupPage_json]
    partial class EditUserGroupPage : Page, IBound<Concepts.Ring3.SystemUserGroup> {

        #region Properties

        #region Set Based On System User group

        /// <summary>
        /// Selected System User group (dropdown)
        /// </summary>
        public string SelectedBasedOnGroupID_ {
            get {

                if (this.Data.Parent == null) return null;

                Concepts.Ring3.SystemUserGroup group = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.ObjectID=?", this.Data.Parent.GetObjectID()).First;
                if (group == null) return null;

                return group.GetObjectID();
            }
            set {

                if (string.IsNullOrEmpty(value)) {
                    this.Data.Parent = null;
                }

                Concepts.Ring3.SystemUserGroup group = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.ObjectID=?", value).First;

                this.Data.Parent = group;

            }
        }

        /// <summary>
        /// System User Groups (exept the current one)
        /// </summary>
        public IEnumerable Groups {
            get {

                // TODO: do not show groups that this group is inhereted

                return Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.ObjectID<>? ORDER BY o.Name", ((Concepts.Ring3.SystemUserGroup)this.Data).GetObjectID());
            }
        }

        #endregion

        #endregion


        /// <summary>
        /// Validate username, Show Feedback to user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="systemUser"></param>
        /// <returns></returns>
        private bool ValidateUserToAdd(string username, out Concepts.Ring3.SystemUser systemUser) {

            systemUser = null;

            if (string.IsNullOrEmpty(username)) {
                this.RemovePropertyFeedback("AddUser_Feedback");
                return false;
            }

            systemUser = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", username).First;
            if (systemUser == null) {
                this.AddPropertyFeedback("AddUser_Feedback", PropertyFeedback.PropertyFeedbackType.Error, "User not found");
                return false;
            }

            // Check if user is alread a member of this sytem user group
            Concepts.Ring3.SystemUserGroup group = this.Data as Concepts.Ring3.SystemUserGroup;

            foreach (var member in group.Members) {
                if (member.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase)) {
                    this.AddPropertyFeedback("AddUser_Feedback", PropertyFeedback.PropertyFeedbackType.Error, "User is already member of the group");
                    return false;
                }
            }

            this.RemovePropertyFeedback("AddUser_Feedback");
            return true;
        }

        #region View-model Handlers

        /// <summary>
        /// BasedOn changed
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.SelectedBasedOnGroupID action) {

            Concepts.Ring3.SystemUserGroup group = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.ObjectID=?", action.Value).First;

            // Check for circular references?
            while (group != null) {

                if (group.Equals(this.Data)) {
                    this.Message = "Circular referenced system groups are not allowd";
                    action.Cancel();
                    action.Cancelled = true;
                    this.SelectedBasedOnGroupID = action.OldValue;
                    return;
                }

                group = group.Parent;
            }

            this.Message = string.Empty;
        }

        /// <summary>
        /// System Group name changed
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Name action) {

            var result = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.Name=? AND o<>?", action.Value, this.Data).First;
            if (result != null) {
                this.AddPropertyFeedback("Name_Feedback", PropertyFeedback.PropertyFeedbackType.Error, "System group with that name already exists");
            }
            else {
                this.RemovePropertyFeedback("Name_Feedback");
            }
        }

        /// <summary>
        /// AddUser operation invoked
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.AddUser action) {

            this.AddUser = false;
            action.Value = false;
            Concepts.Ring3.SystemUser systemUser;
            if (!this.ValidateUserToAdd(this.AddUserName, out systemUser)) {
                return;
            }

            Concepts.Ring3.SystemUserGroup group = this.Data as Concepts.Ring3.SystemUserGroup;

            SystemUserAdmin.AddSystemUserToSystemUserGroup(systemUser, group);

            this.AddUserName = string.Empty;
        }

        /// <summary>
        /// AddUsername was changed
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.AddUserName action) {

            Concepts.Ring3.SystemUser systemUser;
            this.ValidateUserToAdd(action.Value, out systemUser);
        }

        /// <summary>
        /// Delete System User Group
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Delete action) {

            // TODO: Warn user with Yes/No dialog

            this.Transaction.Add(() => {
                SystemUserAdmin.DeleteSystemUserGroup(this.Data as Concepts.Ring3.SystemUserGroup);
            });
            this.Transaction.Commit();

            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/usergroups";

            this.Delete = false;
            action.Value = false;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/usergroups";

            this.Save = false;
            action.Value = false;
        }

        /// <summary>
        /// Close page
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = Program.LauncherWorkSpacePath + "/UserAdminApp/admin/usergroups";

            this.Close = false;
            action.Value = false;
        }

        #endregion
    }

    [EditUserGroupPage_json.Members]
    partial class UserGroupMember : Json {

        /// <summary>
        /// Remove a SystemUser from the group
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Remove action) {
            Concepts.Ring3.SystemUserGroup group = this.Parent.Parent.Data as Concepts.Ring3.SystemUserGroup;
            Concepts.Ring3.SystemUser systemUser = this.Data as Concepts.Ring3.SystemUser;
            SystemUserAdmin.RemoveSystemUserFromSystemUserGroup(systemUser, group);

            this.Remove = false;
            action.Value = false;
        }
    }
}
