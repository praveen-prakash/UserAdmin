using System;
using Starcounter;
using Simplified.Ring5;

namespace UserAdmin {
    public class CommitHooks {
        private static bool oldIsAdmin = false;

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
            bool isAdmin = UserSessionPage.IsAdmin();
            UserSessionPage page = Session.Current.Data as UserSessionPage;

            if (page == null) {
                return;
            }

            if (page.Menu != null) {
                AdminMenu menu = page.Menu as AdminMenu;

                menu.IsAdministrator = isAdmin;

                if (!isAdmin && oldIsAdmin) {
                    menu.RedirectUrl = "/";
                }

                oldIsAdmin = isAdmin;
            }
        }
    }
}
