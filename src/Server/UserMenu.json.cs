using Starcounter;

namespace UserAdminApp.Server {

    [UserMenu_json]
    partial class UserMenu : Json {

        void Handle(Input.Register action) {

            // TODO: Warn user with Yes/No dialog

            //Db.Transaction(() => {
            //    this.Data.Delete();
            //});

            this.RedirectUrl = "/launcher/workspace/admin/systemuser/register";

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
