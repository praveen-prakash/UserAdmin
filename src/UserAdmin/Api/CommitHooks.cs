using PolyjuiceNamespace;
using Starcounter;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplified.Ring5;

namespace UserAdmin {
    public class CommitHooks {
        internal static void Register() {
            Hook<SystemUserSession>.CommitInsert += (s, a) => {
                RefreshSignInState();
            };

            Hook<SystemUserSession>.CommitUpdate += (s, a) => {
                RefreshSignInState();
            };

            Hook<SystemUserSession>.CommitDelete += (s, a) => {
                RefreshSignInState();
            };
        }

        private static void RefreshSignInState() {
            bool isAuthorized = UserSessionPage.IsAdmin();
            UserSessionPage page = Session.Current.Data as UserSessionPage;

            if (page == null) {
                return;
            }

            if (page.Menu != null) {
                AdminMenu menu = page.Menu as AdminMenu;

                menu.IsAdministrator = isAuthorized;

                if (!isAuthorized) {
                    menu.RedirectUrl = "/";
                }
            }
        }
    }
}
