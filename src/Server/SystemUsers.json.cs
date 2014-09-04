using Concepts.Ring1;
using Concepts.Ring3;
using Starcounter;
using System.Collections;

namespace UserAdminApp.Server {

    [SystemUsers_json]
    partial class SystemUsers : Json {

        public IEnumerable Users {

            get {
                return Db.SQL<Concepts.Ring3.SystemUser>("SELECT o FROM Concepts.Ring3.SystemUser o");
            }
        }

        void Handle(Input.AddSystemUser action) {

            // TODO: Check for duplicates

            Db.Transaction(() => {

                Concepts.Ring3.SystemUser user = new Concepts.Ring3.SystemUser();
                user.Username = this.Username;
                user.Password = this.Password;
            });

            this.Username = this.Password = string.Empty;
        }

        #region Base

        /// <summary>
        /// The way to get a URL for HTML partial if any.
        /// </summary>
        /// <returns></returns>
        public override string GetHtmlPartialUrl() {
            return Html;
        }
     
        #endregion
    }

    [SystemUsers_json.Items]
    partial class SystemUsersItems : Json {

        void Handle(Input.Delete action) {

            // TODO: Warning user with Yes/No dialog

            Db.Transaction(() => {

                this.Data.Delete();
            });
        }
    }
}
