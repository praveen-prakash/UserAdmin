using Starcounter;
using System.Collections;

namespace UserAdmin {

    [EditCompanyPage_json]
    partial class EditCompanyPage : SomebodyPage {
    }

    [EditCompanyPage_json.Groups]
    partial class CompanyGroupItem : SombodyGroupItem {
        protected override void OnData() {
            base.OnData();
            this.Url = string.Format("/useradmin/admin/usergroups/{0}", this.Key);
        }
    }
}
