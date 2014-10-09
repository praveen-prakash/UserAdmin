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

    [SettingsPage_json]
    partial class SettingsPage : Page {

        void Handle(Input.Port action) {

            if (action.Value > IPEndPoint.MaxPort || (action.Value < IPEndPoint.MinPort)) {
                this.AddPropertyFeedback("Port_Feedback", PropertyFeedback.PropertyFeedbackType.Error, "Invalid port number");
            }
            else {
                this.RemovePropertyFeedback("Port_Feedback");
            }
        }


        void Handle(Input.Save action) {

            this.Transaction.Commit();
            this.RedirectUrl = "/launcher/workspace/admin/users";
        }

        void Handle(Input.Close action) {

            this.Transaction.Rollback();
            this.RedirectUrl = "/launcher/workspace/admin/users";
        }
    }
}