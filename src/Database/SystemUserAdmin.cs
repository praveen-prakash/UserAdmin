using Concepts.Ring1;
using Concepts.Ring2;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminApp.Server;

namespace UserAdminApp.Database {
    public class SystemUserAdmin {

        /// <summary>
        /// Add Person with a system user
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="surname"></param>
        /// <param name="email"></param>
        public static Concepts.Ring3.SystemUser AddPerson(string firstName, string surname, string username, string email, string password) {

            if (firstName == null) {
                throw new ArgumentNullException("firstname");
            }

            if (surname == null) {
                throw new ArgumentNullException("surname");
            }

            if (username == null) {
                throw new ArgumentNullException("username");
            }

            if (email == null) {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(firstName)) {
                throw new ArgumentException("firstname");
            }

            if (string.IsNullOrEmpty(surname)) {
                throw new ArgumentException("surname");
            }

            if (string.IsNullOrEmpty(email)) {
                throw new ArgumentException("email");
            }

            // Validation

            // Check for duplicated email
            string emailLow = email.ToLowerInvariant();

            if (!Utils.IsValidEmail(email)) {
                throw new ArgumentException("email", "Invalid email address");
            }

            // Check if there is any system users that has this email.
            var dupEmail = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.EMail=? AND o.ToWhat IS Concepts.Ring3.SystemUser", emailLow).First;
            if (dupEmail != null) {
                throw new ArgumentException("email", "Duplicated email");
            }

            // Check for duplicated username
            var dupUserName = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", username).First;
            if (dupUserName != null) {
                throw new ArgumentException("username", "Duplicated username");
            }

            Person person = new Person() { FirstName = firstName, Surname = surname };
            Concepts.Ring3.SystemUser systemUser = new Concepts.Ring3.SystemUser(person);
            systemUser.Username = username;
            SetPassword(systemUser, password);

            // Add ability to also sign in with email
            EMailAddress emailRel = new EMailAddress();
            emailRel.SetToWhat(systemUser);
            emailRel.EMail = emailLow.ToLowerInvariant();

            person.ImageURL = Utils.GetGravatarUrl(emailRel.EMail);

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
            var dupEmail = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.EMail=? AND o.ToWhat IS Concepts.Ring3.SystemUser", emailLow).First;
            if (dupEmail != null) {
                throw new ArgumentException("email", "Duplicated email");
            }

            // Check for duplicated username
            var dupUserName = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", username).First;
            if (dupUserName != null) {
                throw new ArgumentException("username", "Duplicated username");
            }

            Company company = new Company() { Name = name };

            Concepts.Ring3.SystemUser systemUser = new Concepts.Ring3.SystemUser(company);
            systemUser.Username = username;
            SetPassword(systemUser, password);

            // Add ability to also sign in with email
            EMailAddress emailRel = new EMailAddress();
            emailRel.SetToWhat(systemUser);
            emailRel.EMail = emailLow;

            //emailRel = new EMailAddress();
            //emailRel.SetToWhat(company);
            //emailRel.EMail = emailLow;
            company.ImageURL = Utils.GetGravatarUrl(emailRel.EMail);
        }

        /// <summary>
        /// Delete System user
        /// </summary>
        /// <param name="user"></param>
        public static void DeleteSystemUser(Concepts.Ring3.SystemUser user) {

            if (user == null) {
                throw new ArgumentNullException("user");
            }

            // Remove Email adresses associated to the system user
            Db.SlowSQL("DELETE FROM Concepts.Ring2.EMailAddress WHERE ToWhat=?", user);

            // Remove ResetPassword associated to the system user Sombody
            Db.SlowSQL("DELETE FROM UserAdminApp.Database.ResetPassword WHERE User=?", user);

            // TODO: Should we also delete the Somebody (Person/Company)?

            // Remove system user group member (If system user is member of a system user group)
            Db.SlowSQL("DELETE FROM Concepts.Ring3.SystemUserGroupMember WHERE SystemUser=?", user);

            user.Delete();
        }

        /// <summary>
        /// Delete System User Group and it's relationships
        /// </summary>
        /// <param name="group"></param>
        public static void DeleteSystemUserGroup(Concepts.Ring3.SystemUserGroup group) {


            // Remove System user member's
            Db.SlowSQL("DELETE FROM Concepts.Ring3.SystemUserGroupMember WHERE SystemUserGroup=?", group);

            group.Delete();
        }

        /// <summary>
        /// Add System User as a Member of a SystemUserGroup
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public static void AddSystemUserToSystemUserGroup(Concepts.Ring3.SystemUser user, Concepts.Ring3.SystemUserGroup group) {

            Concepts.Ring3.SystemUserGroupMember systemUserGroupMember = new Concepts.Ring3.SystemUserGroupMember();
            systemUserGroupMember.SetSystemUser(user);
            systemUserGroupMember.SetToWhat(group);
            //group.AddMember(systemUser);
        }

        /// <summary>
        /// Remove System User as a Member of a SystemUserGroup
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public static void RemoveSystemUserFromSystemUserGroup(Concepts.Ring3.SystemUser user, Concepts.Ring3.SystemUserGroup group) {

            group.RemoveMember(user);
        }

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public static void SetPassword(Concepts.Ring3.SystemUser user, string password) {

            string hashedPassword;
            Concepts.Ring8.Polyjuice.SystemUserPassword.GeneratePasswordHash(user.Username.ToLower(), password, out hashedPassword);
            user.Password = hashedPassword;
        }
    }
}
