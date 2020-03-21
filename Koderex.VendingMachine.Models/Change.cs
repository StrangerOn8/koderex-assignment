using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    /// <summary>
    /// An object that represents the amount of a perticular coin must be given by the machine.
    /// </summary>
    public class Change {
        public int Coin { get; set; }
        public int Total { get; set; }
    }
}
