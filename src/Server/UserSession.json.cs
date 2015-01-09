using Concepts.Ring3;
using Concepts.Ring8.Polyjuice.App;
using Concepts.Ring8.Polyjuice.Permissions;
using PolyjuiceNamespace;
using Starcounter;
using System.Web;
using UserAdminApp.Database;
using UserAdminApp.Server.Partials;

namespace UserAdminApp.Server {

    [UserSession_json]
    partial class UserSession : UserAdminApp.Server.Partials.Page {

        internal object Menu;

        /// <summary>
        /// Check if user is Authorized
        /// </summary>
        /// <returns></returns>
        static public bool IsAdmin() {

            Concepts.Ring3.SystemUserGroup adminGroup = Db.SQL<Concepts.Ring3.SystemUserGroup>("SELECT o FROM Concepts.Ring3.SystemUserGroup o WHERE o.Name=?", Program.AdminGroupName).First;
            Concepts.Ring3.SystemUser user = UriPermissionHelper.GetCurrentSystemUser();
            return UriPermission.IsMemberOfGroup(user, adminGroup);
        }
    }
}
