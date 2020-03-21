using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    /// <summary>
    /// The response showing the total change, the segmented coin denomination, and a list in descending order of all coins due.
    /// <see cref="ResponseModel"/>
    /// <seealso cref="Change"/>
    /// </summary>
    public class PurchaseItemResponse : ResponseModel {
        public decimal TotalChangeDue { get; set; }
        public List<Change> Denominations { get; set; }
        public List<int> Coins { get; set; }
    }
}
