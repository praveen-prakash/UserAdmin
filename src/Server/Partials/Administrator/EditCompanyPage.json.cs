using Starcounter;
using System.Collections;
using UserAdminApp.Database;

namespace UserAdminApp.Server.Partials.Administrator {

    [EditCompanyPage_json]
    partial class EditCompanyPage : Page {


        //public string Type_ {
        //    get {

        //        Concepts.Ring3.SystemUser user = this.Data as Concepts.Ring3.SystemUser;
        //        if (user != null) {
        //            if (user.WhoIs != null) {
        //                return user.WhoIs.GetType().Name;
        //            }
        //            else {
        //                return user.GetType().Name;
        //            }
        //        }
        //        return null;
        //    }
        //}

        void Handle(Input.Delete action) {

            // TODO: Warn user with Yes/No dialog

            SystemUserAdmin.DeleteSystemUser(this.Data as Concepts.Ring3.SystemUser);
            this.RedirectUrl = "/launcher/workspace/admin/users";
        }

        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = "/launcher/workspace/admin/users";
        }

        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = "/launcher/workspace/admin/users";
        }
    }
}
