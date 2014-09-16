using Starcounter;
using System;

namespace UserAdminApp.Server {
    partial class PropertyFeedback : Json {

        [FlagsAttribute]
        public enum PropertyFeedbackType {
            None = 0,
            Message = 1,
            Warning = 2,
            Error = 3
        }
    }
}
