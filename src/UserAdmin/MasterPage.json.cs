using Simplified.Ring3;
using Starcounter;

namespace UserAdmin {
    partial class MasterPage : Page {

        static public bool IsAdmin() {

            SystemUserGroup adminGroup = Db.SQL<SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name = ?", Program.AdminGroupName).First;
            SystemUser user = Helper.GetCurrentSystemUser();

            return Helper.IsMemberOfGroup(user, adminGroup);
        }
    }
}
