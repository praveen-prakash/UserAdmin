using Starcounter;

namespace UserAdminApp.Server {
    partial class SystemUser : Json {

        void Handle(Input.Delete action) {

            // TODO: Warn user with Yes/No dialog

            Db.Transaction(() => {
                this.Data.Delete();
            });

            this.RedirectUrl = "/launcher/workspace/admin/systemusers";

        }

        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = "/launcher/workspace/admin/systemusers";
        }

        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = "/launcher/workspace/admin/systemusers";
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
}
