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
        public static void AddPerson(string firstName, string surname, string email, string password) {

            if (firstName == null) {
                throw new ArgumentNullException("firstname");
            }

            if (surname == null) {
                throw new ArgumentNullException("surname");
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

            if (!Utils.IsValidEmail(email)) {
                throw new ArgumentException("email", "Invalid email address");
            }


            Db.Transaction(() => {

                Person person = new Person() { FirstName = firstName, Surname = surname };
                Concepts.Ring3.SystemUser systemUser = new Concepts.Ring3.SystemUser(person);
                systemUser.Password = password;
                systemUser.Username = email;

                EMailAddress emailRel = new EMailAddress();
                emailRel.SetToWhat(systemUser);
                emailRel.Name = email;

                emailRel = new EMailAddress();
                emailRel.SetToWhat(person);
                emailRel.Name = email;
            });
        }

        /// <summary>
        /// Add company with a system user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        public static void AddCompany(string name, string email, string password) {

            if (name == null) {
                throw new ArgumentNullException("name");
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

            if (!Utils.IsValidEmail(email)) {
                throw new ArgumentException("email", "Invalid email address");
            }

            Db.Transaction(() => {
                Company company = new Company() { Name = name };

                Concepts.Ring3.SystemUser systemUser = new Concepts.Ring3.SystemUser(company);
                systemUser.Username = email;
                systemUser.Password = password;

                EMailAddress emailRel = new EMailAddress();
                emailRel.SetToWhat(systemUser);
                emailRel.Name = email;

                emailRel = new EMailAddress();
                emailRel.SetToWhat(company);
                emailRel.Name = email;
            });
        }

    }
}
