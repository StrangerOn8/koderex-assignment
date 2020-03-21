using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    /// <summary>
    /// The item in the vending machine with the price, position, and item description.
    /// </summary>
    public class MachineItem {
        public MachineItemPosition Position { get; set; }
        public string Item { get; set; }
        public decimal Price { get; set; }
    }
}
