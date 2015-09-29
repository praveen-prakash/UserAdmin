using System;
using Starcounter;

namespace UserAdmin {
    public class SettingsHandlers {
        public static void Register() {
            Handle.GET("/UserAdmin/admin/settings", (Request request) => {

                // TODO: At the moment we only show MailSettings, eventually MailSettings will only be one part of all settings
                Json page;

                if (!Helper.TryNavigateTo("/UserAdmin/admin/settings", request, "/useradmin/viewmodels/RedirectPage.html", out page)) {
                    return page;
                }

                return Db.Scope<string, Json>((string uri) => {
                    SettingsPage settingsPage = new SettingsPage() {
                        Html = "/UserAdmin/viewmodels/partials/administrator/SettingsPage.html",
                        Uri = uri
                    };
                    settingsPage.Data = UserAdmin.SettingsMailServer.Settings;
                    return settingsPage;
                },
                request.Uri);
            });
        }
    }
}
