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
using UserAdminApp.Database;
using UserAdminApp.Server;


namespace UserAdminApp {
    public class Program {

        internal static string AdminGroupName = "Admin (System Users)";

        internal static Dictionary<string, UserAdminApp.Server.UserSession> Sessions = new Dictionary<string, UserAdminApp.Server.UserSession>();

        static void Main() {


            //Helper.AssureOneAdminSystemUser(Program.AdminGroupName, "System User Administrator Group");

            Program.SetupPermissions();

            UserAdminApp.Server.Handlers.Settings.Register();
            UserAdminApp.Server.Handlers.SystemUsers.Register();
            UserAdminApp.Server.Handlers.SystemUserGroups.Register();
            Database.CommitHooks.Register();
            UserAdminApp.Server.Handlers.LauncherHooks.Register();

            //Polyjuice.OntologyMap("/UserAdminApp/admin/users/@w", "/so/somebody/@w", null, null);

            Polyjuice.OntologyMap("/UserAdminApp/admin/users/@w", "/so/person/@w",
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
            Helper.AssureUriPermission("/UserAdminApp/admin/users", adminGroup);
            Helper.AssureUriPermission("/UserAdminApp/admin/users/{?}", adminGroup);
            Helper.AssureUriPermission("/UserAdminApp/admin/createuser", adminGroup);

            Helper.AssureUriPermission("/UserAdminApp/admin/usergroups", adminGroup);
            Helper.AssureUriPermission("/UserAdminApp/admin/usergroups/{?}", adminGroup);
            Helper.AssureUriPermission("/UserAdminApp/admin/createusergroup", adminGroup);

            Helper.AssureUriPermission("/UserAdminApp/admin/settings", adminGroup);
        }
    }
}
