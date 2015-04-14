using PolyjuiceNamespace;
using Simplified.Ring1;
using Simplified.Ring3;
using Simplified.Ring5;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdmin.Database;
using UserAdmin.Server;


namespace UserAdmin {
    public class Program {

        internal static string AdminGroupName = "Admin (System Users)";

        internal static Dictionary<string, UserAdmin.Server.UserSession> Sessions = new Dictionary<string, UserAdmin.Server.UserSession>();

        static void Main() {


            //Helper.AssureOneAdminSystemUser(Program.AdminGroupName, "System User Administrator Group");

            Program.SetupPermissions();

            UserAdmin.Server.Handlers.Settings.Register();
            UserAdmin.Server.Handlers.SystemUsers.Register();
            UserAdmin.Server.Handlers.SystemUserGroups.Register();
            Database.CommitHooks.Register();
            UserAdmin.Server.Handlers.LauncherHooks.Register();

            //Polyjuice.OntologyMap("/UserAdmin/admin/users/@w", "/so/somebody/@w", null, null);

            Polyjuice.OntologyMap("/UserAdmin/admin/users/@w", "/so/person/@w",
            (String fromSo) => {
                var user = Db.SQL<SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.ObjectID=?", fromSo).First;
                if (user != null) {
                    return user.WhatIs.Key;
                }
                return null;
            },
            (String fromSo) => {
                var user = Db.SQL<SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.WhatIs.ObjectID=?", fromSo).First;
                if (user != null) {
                    return user.Key;
                }
                return null;
            });

        }

        /// <summary>
        /// Set up Uri permissions
        /// TODO: This is hardcoded, we need a gui!!
        /// TODO: Automate this
        /// </summary>
        static private void SetupPermissions() {

            SystemUserGroup adminGroup = Db.SQL<SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name=?", Program.AdminGroupName).First;
            Helper.AssureUriPermission("/UserAdmin/admin/users", adminGroup);
            Helper.AssureUriPermission("/UserAdmin/admin/users/{?}", adminGroup);
            Helper.AssureUriPermission("/UserAdmin/admin/createuser", adminGroup);

            Helper.AssureUriPermission("/UserAdmin/admin/usergroups", adminGroup);
            Helper.AssureUriPermission("/UserAdmin/admin/usergroups/{?}", adminGroup);
            Helper.AssureUriPermission("/UserAdmin/admin/createusergroup", adminGroup);

            Helper.AssureUriPermission("/UserAdmin/admin/settings", adminGroup);
        }
    }
}
