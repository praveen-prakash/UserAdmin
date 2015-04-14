using Starcounter;

namespace UserAdmin.Server.Partials.Administrator {
    [SearchResult_json]
    partial class SearchResult : Page {

        [SearchResult_json.Users]
        partial class UserItem : Json {
            public string UserUri {

                get {
                    return "/UserAdmin/admin/users/" + Data.GetObjectID();
                }
            }
        }

        [SearchResult_json.Groups]
        partial class GroupItem : Json {
            public string GroupUri {

                get {
                    return "/UserAdmin/admin/usergroups/" + Data.GetObjectID();
                }
            }
        }
    }
}
