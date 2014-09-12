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
        public static void AddPerson(string firstName, string surname, string username, string email, string password) {

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
            var dupEmail = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.EMail=?", emailLow).First;
            if (dupEmail != null) {
                throw new ArgumentException("username", "Duplicated email");
            }

            // Check for duplicated username
            var dupUserName = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", username).First;
            if (dupUserName != null) {
                throw new ArgumentException("username", "Duplicated username");
            }

            Db.Transaction(() => {

                Person person = new Person() { FirstName = firstName, Surname = surname };
                Concepts.Ring3.SystemUser systemUser = new Concepts.Ring3.SystemUser(person);
                systemUser.Username = username;
                SetPassword(systemUser, password);

                // Add ability to also sign in with email
                EMailAddress emailRel = new EMailAddress();
                emailRel.SetToWhat(systemUser);
                emailRel.EMail = emailLow.ToLowerInvariant();

                emailRel = new EMailAddress();
                emailRel.SetToWhat(person);
                emailRel.EMail = emailLow.ToLowerInvariant();
                person.ImageURL = Utils.GetGravatarUrl(emailRel.EMail);
            });
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
            var dupEmail = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.EMail=?", emailLow).First;
            if (dupEmail != null) {
                throw new ArgumentException("username", "Duplicated email");
            }

            // Check for duplicated username
            var dupUserName = Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o WHERE o.Username=?", username).First;
            if (dupUserName != null) {
                throw new ArgumentException("username", "Duplicated username");
            }


            Db.Transaction(() => {
                Company company = new Company() { Name = name };

                Concepts.Ring3.SystemUser systemUser = new Concepts.Ring3.SystemUser(company);
                systemUser.Username = username;
                SetPassword(systemUser, password);

                // Add ability to also sign in with email
                EMailAddress emailRel = new EMailAddress();
                emailRel.SetToWhat(systemUser);
                emailRel.EMail = emailLow;

                emailRel = new EMailAddress();
                emailRel.SetToWhat(company);
                emailRel.EMail = emailLow;
                company.ImageURL = Utils.GetGravatarUrl(emailRel.EMail);
            });
        }

        /// <summary>
        /// Delete System user
        /// </summary>
        /// <param name="user"></param>
        public static void DeleteSystemUser(Concepts.Ring3.SystemUser user) {

            if (user == null) {
                throw new ArgumentNullException("user");
            }

            Db.Transaction(() => {

                // Remove Email adresses associated to the system user
                var result = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.ToWhat=?", user);
                foreach (var addr in result) {
                    addr.Delete();
                }

                // Remove Email adresses associated to the system user Sombody
                if (user.WhatIs != null) {
                    result = Db.SQL<Concepts.Ring2.EMailAddress>("SELECT o FROM Concepts.Ring2.EMailAddress o WHERE o.ToWhat=?", user.WhatIs);
                    foreach (var addr in result) {
                        addr.Delete();
                    }
                }

                // Remove ResetPassword associated to the system user Sombody
                if (user.WhatIs != null) {
                    var res = Db.SQL<UserAdminApp.Database.ResetPassword>("SELECT o FROM UserAdminApp.Database.ResetPassword o WHERE o.User=?", user);
                    foreach (var addr in res) {
                        addr.Delete();
                    }
                }


                user.Delete();
            });

        }

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public static void SetPassword(Concepts.Ring3.SystemUser user, string password) {

            Db.Transaction(() => {
                string hashedPassword;
                Concepts.Ring5.SystemUserPassword.GeneratePasswordHash(user.Username, password, out hashedPassword);
                user.Password = hashedPassword;
            });

        }

    }
}
