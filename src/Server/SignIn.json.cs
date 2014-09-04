using Starcounter;

namespace UserAdminApp.Server {

    [SignIn_json]
    partial class SignIn : Json
    {
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
