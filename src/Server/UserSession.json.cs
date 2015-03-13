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

            Simplified.Ring3.SystemUserGroup adminGroup = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name=?", Program.AdminGroupName).First;
            Simplified.Ring3.SystemUser user = Helper.GetCurrentSystemUser();
            return Helper.IsMemberOfGroup(user, adminGroup);
        }
    }
}
