using Starcounter;

namespace UserAdminApp.Server.Partials.Administrator {
    [SearchResult_json]
    partial class SearchResult : Page {

        [SearchResult_json.Users]
        partial class UserItem : Json {
            public string UserUri {

                get {
                    return "/UserAdminApp/admin/users/" + Data.GetObjectID();
                }
            }
        }

        [SearchResult_json.Groups]
        partial class GroupItem : Json {
            public string GroupUri {

                get {
                    return "/UserAdminApp/admin/usergroups/" + Data.GetObjectID();
                }
            }
        }
    }
}
