using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    /// <summary>
    /// Categorical lists of available <see cref="MachineItem"/> in the vending machine.
    /// </summary>
    public class VendingMachineItems {
        public IEnumerable<MachineItem> Chips { get; set; }
        public IEnumerable<MachineItem> Chocolates { get; set; }
        public IEnumerable<MachineItem> Sweets { get; set; }
        public IEnumerable<MachineItem> Drinks { get; set; }
    }
}
