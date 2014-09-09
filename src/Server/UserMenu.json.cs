using Starcounter;

namespace UserAdminApp.Server {

    [UserMenu_json]
    partial class UserMenu : Page {

        void Handle(Input.Register action) {

            this.RedirectUrl = "/launcher/workspace/admin/systemuser/register";
        }

    }
}
