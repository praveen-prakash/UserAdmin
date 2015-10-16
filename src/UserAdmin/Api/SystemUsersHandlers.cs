using System;
using System.Collections.Specialized;
using System.Web;
using Starcounter;
using Simplified.Ring2;
using Simplified.Ring3;
using Simplified.Ring6;

namespace UserAdmin {
    public class SystemUsersHandlers {

        public static void Register() {
            string redirectPageHtml = "/useradmin/viewmodels/RedirectPage.html";

            Handle.GET("/useradmin/accessdenied", () => {
                return new AccessDeniedPage();
            });

            // Create System user
            Handle.GET("/useradmin/admin/createuser", (Request request) => {

                MasterPage master = LauncherHooks.GetMaster();

                Json page;
                if (!Helper.TryNavigateTo("/UserAdmin/admin/createuser", request, redirectPageHtml, out page)) {
                    master.CurrentPage = page;
                }
                else {
                    master.CurrentPage = new CreateUserPage() { Html = "/UserAdmin/viewmodels/partials/administrator/CreateUserPage.html", Uri = request.Uri };
                }
                return master;
            });

            // Get System users
            Handle.GET("/useradmin/admin/users", (Request request) => {

                MasterPage master = LauncherHooks.GetMaster();
                Json page;
                if (!Helper.TryNavigateTo("/useradmin/admin/users", request, redirectPageHtml, out page)) {
                    master.CurrentPage = page;
                }
                else {
                    master.CurrentPage = new ListUsersPage() { Html = "/UserAdmin/viewmodels/partials/administrator/ListUsersPage.html", Uri = request.Uri };
                }
                return master;
            });


            Handle.GET("/UserAdmin/admin/users/{?}", (string userid, Request request) => {
                //return Db.Scope<Json>(() => {

                Json page;

                MasterPage master = LauncherHooks.GetMaster();

                if (!Helper.TryNavigateTo("/UserAdmin/admin/users/{?}", request, redirectPageHtml, out page)) {
                    master.CurrentPage = page;
                    return master;
                }

                // Get system user
                Simplified.Ring3.SystemUser user = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.ObjectID = ?", userid).First;

                if (user == null) {
                    // TODO: Return a "User not found" page
                    return master;
                    //return (ushort)System.Net.HttpStatusCode.NotFound;
                }

                SystemUser systemUser = Helper.GetCurrentSystemUser();
                SystemUserGroup adminGroup = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name = ?", Program.AdminGroupName).First;

                // Check if current user has permission to get this user instance
                if (Helper.IsMemberOfGroup(systemUser, adminGroup)) {

                    if (user.WhoIs is Person) {



                        master.CurrentPage = Db.Scope<string, Simplified.Ring3.SystemUser, Json>((uri, personUser) => {
                            return new EditPersonPage() {
                                Html = "/UserAdmin/viewmodels/partials/administrator/EditPersonPage.html",
                                Uri = uri,
                                Data = personUser
                            };
                        }, request.Uri, user);
                        return master;
                    }
                    else if (user.WhoIs is Organization) {
                        Db.Scope<string, Simplified.Ring3.SystemUser, Json>((uri, companyUser) => {
                            return new EditCompanyPage() {
                                Html = "/UserAdmin/viewmodels/partials/administrator/EditCompanyPage.html",
                                Uri = uri,
                                Data = companyUser
                            };
                        },
                        request.Uri, user);
                    }
                }
                else if (user == systemUser) {
                    // User can edit it's self
                }
                else {
                    // No rights
                    // User trying to view another's users data

                    // User has no permission, redirect to app's root page
                    master.CurrentPage = new RedirectPage() {
                        Html = redirectPageHtml,
                        RedirectUrl = "/useradmin"
                    };
                    return master;
                }

                return master;
            });

            // Get System user
            //Handle.GET("/useradmin/admin/_users/{?}", (string userid, Request request) => {
            //    Json page;

            //    MasterPage master = LauncherHooks.GetMaster();

            //    if (!Helper.TryNavigateTo("/UserAdmin/admin/users/{?}", request, redirectPageHtml, out page)) {
            //        master.CurrentPage = page;
            //        return master;
            //    }

            //    // Get system user
            //    Simplified.Ring3.SystemUser user = Db.SQL<Simplified.Ring3.SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.ObjectID = ?", userid).First;

            //    if (user == null) {
            //        // TODO: Return a "User not found" page
            //        return master;
            //        //return (ushort)System.Net.HttpStatusCode.NotFound;
            //    }

            //    SystemUser systemUser = Helper.GetCurrentSystemUser();
            //    SystemUserGroup adminGroup = Db.SQL<Simplified.Ring3.SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name = ?", Program.AdminGroupName).First;

            //    // Check if current user has permission to get this user instance
            //    if (Helper.IsMemberOfGroup(systemUser, adminGroup)) {

            //        if (user.WhoIs is Person) {

            //            master.CurrentPage = Db.Scope<string, Simplified.Ring3.SystemUser, Json>((uri, personUser) => {
            //                return new EditPersonPage() {
            //                    Html = "/UserAdmin/viewmodels/partials/administrator/EditPersonPage.html",
            //                    Uri = uri,
            //                    Data = personUser
            //                };
            //            }, request.Uri, user);
            //            return master;



            //        }
            //        else if (user.WhoIs is Organization) {
            //            Db.Scope<string, Simplified.Ring3.SystemUser, Json>((uri, companyUser) => {
            //                return new EditCompanyPage() {
            //                    Html = "/UserAdmin/viewmodels/partials/administrator/EditCompanyPage.html",
            //                    Uri = uri,
            //                    Data = companyUser
            //                };
            //            },
            //            request.Uri, user);
            //        }
            //    }
            //    else if (user == systemUser) {
            //        // User can edit it's self
            //    }
            //    else {
            //        // No rights
            //        // User trying to view another's users data

            //        // User has no permission, redirect to app's root page
            //        master.CurrentPage = new RedirectPage() {
            //            Html = redirectPageHtml,
            //            RedirectUrl = "/useradmin"
            //        };
            //        return master;
            //    }

            //    return (ushort)System.Net.HttpStatusCode.NotFound;
            //});

            // Reset password
            Handle.GET("/useradmin/user/resetpassword?{?}", (string query, Request request) => {
                NameValueCollection queryCollection = HttpUtility.ParseQueryString(query);
                string token = queryCollection.Get("token");

                MasterPage master = LauncherHooks.GetMaster();

                if (token == null) {
                    // TODO:
                    master.CurrentPage = null; // (ushort)System.Net.HttpStatusCode.NotFound;
                    return master;
                }

                // Retrive the resetPassword instance
                ResetPassword resetPassword = Db.SQL<Simplified.Ring6.ResetPassword>("SELECT o FROM Simplified.Ring6.ResetPassword o WHERE o.Token=? AND o.Expire>?", token, DateTime.UtcNow).First;

                if (resetPassword == null) {
                    // TODO: Show message "Reset token already used or expired"
                    master.CurrentPage = null; // (ushort)System.Net.HttpStatusCode.NotFound;
                    return master;
                }

                if (resetPassword.User == null) {
                    // TODO: Show message "User deleted"
                    master.CurrentPage = null; // (ushort)System.Net.HttpStatusCode.NotFound;
                    return master;
                }

                Simplified.Ring3.SystemUser systemUser = resetPassword.User;

                ResetPasswordPage page = new ResetPasswordPage() {
                    Html = "/UserAdmin/viewmodels/partials/user/ResetPasswordPage.html",
                    Uri = "/useradmin/user/resetpassword"
                    //Uri = request.Uri // TODO:
                };

                page.resetPassword = resetPassword;

                if (systemUser.WhoIs != null) {
                    page.FullName = systemUser.WhoIs.FullName;
                }
                else {
                    page.FullName = systemUser.Username;
                }

                master.CurrentPage = page;

                return master;
            });
        }
    }
}
