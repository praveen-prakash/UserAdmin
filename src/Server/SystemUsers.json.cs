using Concepts.Ring1;
using Concepts.Ring3;
using Starcounter;
using System.Collections;

namespace UserAdministrationApp.Server {

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
        // Browsers will ask for "text/html" and we will give it to them
        // by loading the contents of the URI in our Html property
        public override string AsMimeType(MimeType type) {
            if (type == MimeType.Text_Html) {
                return X.GET<string>(Html);
            }
            return base.AsMimeType(type);
        }

        /// <summary>
        /// The way to get a URL for HTML partial if any.
        /// </summary>
        /// <returns></returns>
        public override string GetHtmlPartialUrl() {
            return Html;
        }

        /// <summary>
        /// Whenever we set a bound data object to this page, we update the
        /// URI property on this page.
        /// </summary>
        protected override void OnData() {
            base.OnData();
            var str = "";
            Json x = this;
            while (x != null) {
                if (x is SystemUsers)
                    str = (x as SystemUsers).UriFragment + str;
                x = x.Parent;
            }
            Uri = str;
        }

        /// <summary>
        /// Override to provide an URI fragment
        /// </summary>
        /// <returns></returns>
        protected virtual string UriFragment {
            get {
                return "";
            }
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
