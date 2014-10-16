using Starcounter;
using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Web;
using UserAdminApp.Database;

// FORGOT PASSWORD:
// http://www.asp.net/identity/overview/features-api/account-confirmation-and-password-recovery-with-aspnet-identity

namespace UserAdminApp.Server.Partials.Administrator {

    [EditPersonPage_json]
    partial class EditPersonPage : SomebodyPage {
    }

    [EditPersonPage_json.Groups]
    partial class PersonGroupItem : SombodyGroupItem {
    }
}
