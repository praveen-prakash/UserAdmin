using Starcounter;
using System;

namespace UserAdmin.Server.Partials {
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
