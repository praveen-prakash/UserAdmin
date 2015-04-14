using Simplified.Ring2;
using Simplified.Ring3;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdmin.Server;

namespace UserAdmin.Database {
    public class SystemUserAdmin {

        /// <summary>
        /// Add Person with a system user
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        public static Simplified.Ring3.SystemUser AddPerson(string firstName, string lastname, string username, string password) {

            if (firstName == null) {
                throw new ArgumentNullException("firstname");
            }

            if (lastname == null) {
                throw new ArgumentNullException("lastname");
            }

            if (username == null) {
                throw new ArgumentNullException("username");
            }

            //if (email == null) {
            //    throw new ArgumentNullException("email");
            //}

            if (string.IsNullOrEmpty(firstName)) {
                throw new ArgumentException("firstname");
            }

            if (string.IsNullOrEmpty(lastname)) {
                throw new ArgumentException("lastname");
            }

            //if (string.IsNullOrEmpty(email)) {
            //    throw new ArgumentException("email");
            //}

            // Validation

            // Check for duplicated email
            string usernameLow = username.ToLowerInvariant();

            //if (!Utils.IsValidEmail(email)) {
            //    throw new ArgumentException("email", "Invalid email address");
            //}

            // Check if there is any system users that has this email.
            //var dupEmail = Db.SQL<Simplified.Ring3.EmailAddress>("SELECT o FROM Simplified.Ring3.EmailAddress o WHERE o.EMail=? AND o.ToWhat IS Simplified.Ring3.SystemUser", emailLow).First;
            //if (dupEmail != null) {
            //    throw new ArgumentException("email", "Duplicated email");
            //}

            // Check for duplicated username
            var dupUserName = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.Username=?", username).First;
            if (dupUserName != null) {
                throw new ArgumentException("username", "Duplicated username");
            }

            Person person = new Person() { FirstName = firstName, LastName = lastname };
            Simplified.Ring3.SystemUser systemUser = new Simplified.Ring3.SystemUser();
            systemUser.WhatIs = person;
            systemUser.Username = username;
            SetPassword(systemUser, password);

            // Add ability to also sign in with email
            //EmailAddress emailRel = new EmailAddress();
            //emailRel.SetToWhat(systemUser);
            //emailRel.EMail = emailLow.ToLowerInvariant();

            person.ImageURL = Utils.GetGravatarUrl(usernameLow);

            return systemUser;
        }

        /// <summary>
        /// Add company with a system user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        public static void AddCompany(string name, string username, string email, string password) {

            if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (username == null) {
                throw new ArgumentNullException("username");
            }

            if (email == null) {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentException("name");
            }

            if (string.IsNullOrEmpty(email)) {
                throw new ArgumentException("email");
            }

            // Check for duplicated email
            string emailLow = email.ToLowerInvariant();

            if (!Utils.IsValidEmail(email)) {
                throw new ArgumentException("email", "Invalid email address");
            }

            // Check if there is any system users that has this email.
            var dupEmail = Db.SQL<Simplified.Ring3.EmailAddress>("SELECT o FROM Simplified.Ring3.EmailAddress o WHERE o.EMail=? AND o.ToWhat IS Simplified.Ring3.SystemUser", emailLow).First;
            if (dupEmail != null) {
                throw new ArgumentException("email", "Duplicated email");
            }

            // Check for duplicated username
            var dupUserName = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.Username=?", username).First;
            if (dupUserName != null) {
                throw new ArgumentException("username", "Duplicated username");
            }

            Organization company = new Organization() { Name = name };

            Simplified.Ring3.SystemUser systemUser = new Simplified.Ring3.SystemUser();
            systemUser.WhatIs = company;
            systemUser.Username = username;
            SetPassword(systemUser, password);

            //// Add ability to also sign in with email
            //EmailAddress emailRel = new EmailAddress();
            //emailRel.SetToWhat(systemUser);
            //emailRel.EMail = emailLow;

            //emailRel = new EMailAddress();
            //emailRel.SetToWhat(company);
            //emailRel.EMail = emailLow;
            company.ImageURL = Utils.GetGravatarUrl(emailLow);
        }

        /// <summary>
        /// Delete System user
        /// </summary>
        /// <param name="user"></param>
        public static void DeleteSystemUser(Simplified.Ring3.SystemUser user) {

            if (user == null) {
                throw new ArgumentNullException("user");
            }

            // Remove Email adresses associated to the system user
//            Db.SlowSQL("DELETE FROM Simplified.Ring3.EmailAddress WHERE ToWhat=?", user);

            // Remove ResetPassword associated to the system user Sombody
            Db.SlowSQL("DELETE FROM Simplified.Ring6.ResetPassword WHERE User=?", user);

            // TODO: Should we also delete the Somebody (Person/Company)?

            // Remove system user group member (If system user is member of a system user group)
            Db.SlowSQL("DELETE FROM Simplified.Ring3.SystemUserGroupMember WHERE SystemUser=?", user);

            user.Delete();
        }

        /// <summary>
        /// Delete System User Group and it's relationships
        /// </summary>
        /// <param name="group"></param>
        public static void DeleteSystemUserGroup(Simplified.Ring3.SystemUserGroup group) {


            // Remove System user member's
            Db.SlowSQL("DELETE FROM Simplified.Ring3.SystemUserGroupMember WHERE SystemUserGroup=?", group);

            group.Delete();
        }

        /// <summary>
        /// Add System User as a Member of a SystemUserGroup
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public static void AddSystemUserToSystemUserGroup(Simplified.Ring3.SystemUser user, Simplified.Ring3.SystemUserGroup group) {

            Simplified.Ring3.SystemUserGroupMember systemUserGroupMember = new Simplified.Ring3.SystemUserGroupMember();
            systemUserGroupMember.WhatIs = user;
            systemUserGroupMember.ToWhat = group;
            //systemUserGroupMember.SetSystemUser(user);
            //systemUserGroupMember.SetToWhat(group);
            //group.AddMember(systemUser);
        }

        /// <summary>
        /// Remove System User as a Member of a SystemUserGroup
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public static void RemoveSystemUserFromSystemUserGroup(Simplified.Ring3.SystemUser user, Simplified.Ring3.SystemUserGroup group) {

            var removeGroup = Db.SQL<Simplified.Ring3.SystemUserGroupMember>("SELECT o FROM Simplified.Ring3.SystemUserGroupMember o WHERE o.WhatIs=? AND o.ToWhat=?", user, group).First;
            if (removeGroup != null) {
                removeGroup.Delete();
            }

            //group.RemoveMember(user);
        }

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public static void SetPassword(Simplified.Ring3.SystemUser user, string password) {

            string hashedPassword;
            Helper.GeneratePasswordHash(user.Username.ToLower(), password, out hashedPassword);
            user.Password = hashedPassword;
        }
    }
}
