using Starcounter;
using System.Collections;
using System.Collections.Generic;

namespace UserAdminApp.Server {
    partial class Page : Json {

        public bool IsDirtyTest_ {
            get {
                if (this.Transaction == null) return false;
                return this.Transaction.IsDirty;
            }
        }

        public bool IsDirty_ {
            get {
                return this.GetIsDiryFlag();
            }
        }

        public bool IsPristine_ {
            get {
                return !this.IsDirty_;
            }
        }

        public bool IsValid_ {
            get {
                return this.Feedbacks.Count == 0;
            }
        }

        public bool IsInvalid_ {
            get {
                return !this.IsValid_;
            }
        }

        protected override void HasChanged(Starcounter.Templates.TValue property) {

            this.RefreshDirtyFlags();
            base.HasChanged(property);
        }

        protected override void InternalSetData(object data, Starcounter.Templates.TObject template, bool readOperation) {
            base.InternalSetData(data, template, readOperation);
        }

        /// <summary>
        /// Refresh Dirty flags
        /// </summary>
        private void RefreshDirtyFlags() {

            //bool isDirtyFlag = this.GetIsDiryFlag();

            //if (this.IsDirty != isDirtyFlag) {
            //    this.IsDirty = isDirtyFlag;
            //}

            //if (this.IsPristine != !isDirtyFlag) {
            //    this.IsPristine = !isDirtyFlag;
            //}
        }

        /// <summary>
        /// Refresh Valid flags
        /// </summary>
        private void RefreshValidFlags() {

            //this.IsValid = this.Feedbacks.Count == 0;
            //this.IsInvalid = !this.IsValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool GetIsDiryFlag() {

            if (this.Transaction != null) {
                return this.Transaction.IsDirty;
            }
            return false;
        }


        /// <summary>
        /// Sets the data to its pristine state.
        /// </summary>
        public void SetPristine() {

            if (this.Transaction != null && this.Transaction.IsDirty) {
                this.Transaction.Rollback();
                this.RefreshDirtyFlags();
                this.ClearAllPropertyFeedback();
            }
        }


        System.Collections.Hashtable Feedbacks = new System.Collections.Hashtable();

        /// <summary>
        /// Add property feedback
        /// </summary>
        /// <param name="feedback"></param>
        public void AddPropertyFeedback(string propertyName, PropertyFeedback.PropertyFeedbackType typeNo, string message) {

            PropertyFeedback pf = new PropertyFeedback() { Type = typeNo.ToString(), TypeNo = (int)typeNo, Message = message };

            if (this.Feedbacks.ContainsKey(propertyName)) return;
            this.Feedbacks.Add(propertyName, pf);

            // Set property value
            System.Reflection.PropertyInfo pi = this.GetType().GetProperty(propertyName);
            pi.SetValue(this, pf);
            this.RefreshValidFlags();
        }

        /// <summary>
        /// Remove property feedback
        /// </summary>
        /// <param name="feedback"></param>
        public void RemovePropertyFeedback(string propertyName) {

            if (!this.Feedbacks.ContainsKey(propertyName)) return;

            System.Reflection.PropertyInfo pi = this.GetType().GetProperty(propertyName);
            pi.SetValue(this, null);

            this.Feedbacks.Remove(propertyName);
            this.RefreshValidFlags();
        }

        private void ClearAllPropertyFeedback() {

            List<string> propNames = new List<string>();

            foreach (DictionaryEntry pair in this.Feedbacks) {
                propNames.Add((string)pair.Key);
            }

            foreach (string propName in propNames) {
                this.RemovePropertyFeedback(propName);
            }

            this.Feedbacks.Clear();
            this.RefreshValidFlags();
        }

        #region View-model Handlers

        /// <summary>
        /// Sets the data to its pristine state.
        /// </summary>
        /// <param name="action"></param>
        void Handle(Input.Rollback action) {
            this.SetPristine();
        }

        #endregion

        #region Base

        /// <summary>
        /// The way to get a URL for HTML partial if any.
        /// </summary>
        /// <returns></returns>
        public override string GetHtmlPartialUrl() {
            return Html;
        }

        #endregion
    }
}
