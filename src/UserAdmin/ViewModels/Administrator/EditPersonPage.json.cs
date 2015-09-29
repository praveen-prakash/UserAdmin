using Starcounter;
using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Web;

// FORGOT PASSWORD:
// http://www.asp.net/identity/overview/features-api/account-confirmation-and-password-recovery-with-aspnet-identity

namespace UserAdmin {

    [EditPersonPage_json]
    partial class EditPersonPage : SomebodyPage, IBound<Simplified.Ring3.SystemUser> {

        void Handle(Input.FirstName action) {

            this.AssurePropertyFeedback_Name("FirstName$", action.Value);
        }

        void Handle(Input.LastName action) {

            this.AssurePropertyFeedback_Name("LastName$", action.Value);
        }

        #region Validate Properties (Create Property Feedbacks)

        protected void AssurePropertyFeedback_Name(string propertyName, string value) {

            string message;
            this.Validate_Name(value, out message);
            if (message != null) {
                PropertyMetadataItem item = new PropertyMetadataItem();
                item.Message = message;
                item.ErrorLevel = 1;
                item.PropertyName = propertyName;
                this.AddPropertyFeedback(item);
            }
            else {
                this.RemovePropertyFeedback(propertyName);
            }
        }

        #endregion

        #region Validate Properties

        private bool Validate_Name(string value, out string message) {

            message = null;
            if (value == null || string.IsNullOrEmpty(value.Trim())) {
                message = "Field can not be empty";
                return false;
            }
            return true;
        }

        #endregion

        protected override void AssurePropertyFeedbacks() {

            if ( this.Data.WhoIs == null || !(this.Data.WhoIs is Simplified.Ring2.Person)) return;

            Simplified.Ring2.Person person = this.Data.WhoIs as Simplified.Ring2.Person;

            AssurePropertyFeedback_Name("FirstName$", person.FirstName);
            AssurePropertyFeedback_Name("LastName$", person.LastName);
            
            base.AssurePropertyFeedbacks();
        }
    }

    [EditPersonPage_json.Groups]
    partial class PersonGroupItem : SombodyGroupItem {
        protected override void OnData() {
            base.OnData();
            this.Url = string.Format("/useradmin/admin/usergroups/{0}", this.Key);
        }
    }
}
