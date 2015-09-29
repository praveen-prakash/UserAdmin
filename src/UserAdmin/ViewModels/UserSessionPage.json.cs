using System.Web;
using Starcounter;
using Simplified.Ring3;

namespace UserAdmin {

    [UserSessionPage_json]
    partial class UserSessionPage : Page {

        internal object Menu;

        /// <summary>
        /// Check if user is Authorized
        /// </summary>
        /// <returns></returns>
        static public bool IsAdmin() {

            SystemUserGroup adminGroup = Db.SQL<SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name = ?", Program.AdminGroupName).First;
            SystemUser user = Helper.GetCurrentSystemUser();

            return Helper.IsMemberOfGroup(user, adminGroup);
        }
    }
}
