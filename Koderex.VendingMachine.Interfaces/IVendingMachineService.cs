using Koderex.VendingMachine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Interfaces {
    public interface IVendingMachineService {
        PurchaseItemResponse PurchaseItem(PurchaseItemRequest request);
    }
}
