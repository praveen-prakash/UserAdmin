using Starcounter;

namespace UserAdminApp.Server.Partials.Administrator {
    [SearchResult_json]
    partial class SearchResult : Page {

        [SearchResult_json.Users]
        partial class UserItem : Json {
            public string UserUri {
                get {
                    return "/launcher/workspace/UserAdminApp/users/" + Data.GetObjectID();
                }
            }
        }

        [SearchResult_json.Groups]
        partial class GroupItem : Json {
            public string GroupUri {
                get {
                    return "/launcher/workspace/UserAdminApp/usergroups/" + Data.GetObjectID();
                }
            }
        }
    }
}
