using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    /// <summary>
    /// Used to determine the country you are working in, and which item in the vending machine is being requested, as well as providing a tender amount for the selected vending machine item.
    /// </summary>
    public class PurchaseItemRequest {
        /// <summary>
        /// This is used to identify the country system is transacting on.
        /// </summary>
        public string Alpha2Code { get; set; }
        /// <summary>
        /// This is used to get the price of the item, by using the row and column position to get the specific item's price.
        /// </summary>
        public MachineItemPosition Position { get; set; }
        public decimal TenderAmount { get; set; }
    }
}
