using Smorgasbord.PropertyMetadata;
using Starcounter;
using System;
using System.Collections;

namespace UserAdmin.Partials.Administrator {
    partial class CreateUserGroupPage : PropertyMetadataPage {

        void Handle(Input.Name action) {

            this.AssurePropertyMetadata_Name(action.Template.TemplateName, action.Value);
        }

        void Handle(Input.Save action) {

            this.AssurePropertiesMetadata();

            if (this.IsInvalid) {
                return;
            }

            try {
                Db.Transact(() => {
                    Simplified.Ring3.SystemUserGroup group = new Simplified.Ring3.SystemUserGroup();
                    group.Name = this.Name;
                });
                this.RedirectUrl = "/UserAdmin/admin/usergroups";

            }
            catch (Exception e) {
                this.Message = e.Message;
            }
        }

        void Handle(Input.Close action) {

            this.RedirectUrl = "/UserAdmin/admin/usergroups";
        }

        #region Validate Properties (Create Property metadata)

        /// <summary>
        /// Assure metadata for all fields
        /// </summary>
        virtual protected void AssurePropertiesMetadata() {

            AssurePropertyMetadata_Name("Name$", this.Name);
        }

        protected void AssurePropertyMetadata_Name(string propertyName, string value) {

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

        /// <summary>
        /// Validate name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool Validate_Name(string value, out string message) {

            message = null;
            if (value == null || string.IsNullOrEmpty(value.Trim())) {
                message = "Field can not be empty";
                return false;
            }
            return true;
        }
        #endregion
    }
}
