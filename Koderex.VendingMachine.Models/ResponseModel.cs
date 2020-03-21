using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    /// <summary>
    /// A base response, used to uniformly handle all responses with a base type.
    /// This helps if you have a logging interceptor, or want to have a standardized way you want you frontend to handle responses.
    /// </summary>
    public class ResponseModel {
        public bool IsSuccessful { get; set; }
        public bool IsClientFriendlyMessage { get; set; }
        public string Message { get; set; }
    }
}
