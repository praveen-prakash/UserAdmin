using Starcounter;

namespace UserAdmin {
    [SearchResultPage_json]
    partial class SearchResultPage : Page {

        [SearchResultPage_json.Users]
        partial class UserItem : Json {
            public string UserUri {
                get {
                    return "/UserAdmin/admin/users/" + Data.GetObjectID();
                }
            }
        }

        [SearchResultPage_json.Groups]
        partial class GroupItem : Json {
            public string GroupUri {
                get {
                    return "/UserAdmin/admin/usergroups/" + Data.GetObjectID();
                }
            }
        }
    }
}
