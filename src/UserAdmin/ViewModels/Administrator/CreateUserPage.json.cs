using Smorgasbord.PropertyMetadata;
using Starcounter;
using System;
using System.Collections;

namespace UserAdmin {
    partial class CreateUserPage : PropertyMetadataPage {

        void Handle(Input.Username action) {
            this.AssurePropertyMetadataName(action.Template.TemplateName, action.Value);
        }

        void Handle(Input.FirstName action) {
            this.AssurePropertyMetadataName(action.Template.TemplateName, action.Value);
        }

        void Handle(Input.LastName action) {
            this.AssurePropertyMetadataName(action.Template.TemplateName, action.Value);
        }

        /// <summary>
        /// Save event
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Save action) {
            this.AssurePropertiesMetadata();

            if (this.IsInvalid) {
                return;
            }

            try {
                Db.Transact(() => {
                    SystemUserAdmin.AddPerson(this.FirstName, this.LastName, this.Username, this.Password);
                });

                this.RedirectUrl = "/UserAdmin/admin/users";
            } catch (Exception e) {
                this.Message = e.Message;
            }
        }

        void Handle(Input.Close action) {
            this.RedirectUrl = "/UserAdmin/admin/users";
        }

        #region Validate Properties (Create Property metadata)

        /// <summary>
        /// Assure metadata for all fields
        /// </summary>
        virtual protected void AssurePropertiesMetadata() {
            AssurePropertyMetadataName("Username$", this.Username);
            AssurePropertyMetadataName("FirstName$", this.FirstName);
            AssurePropertyMetadataName("LastName$", this.LastName);
        }

        protected void AssurePropertyMetadataName(string propertyName, string value) {
            string message;

            this.ValidateName(value, out message);
            
            if (message != null) {
                PropertyMetadataItem item = new PropertyMetadataItem();
                item.Message = message;
                item.ErrorLevel = 1;
                item.PropertyName = propertyName;
                this.AddPropertyFeedback(item);
            } else {
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
        private bool ValidateName(string value, out string message) {
            message = null;

            if (value == null || string.IsNullOrEmpty(value.Trim())) {
                message = "Field can not be empty";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate email
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ValidateEmail(string value, out string message) {
            message = null;

            if (!Utils.IsValidEmail(value)) {
                message = "Invalid email address";
                return false;
            }

            return true;
        }
        #endregion
    }
}
