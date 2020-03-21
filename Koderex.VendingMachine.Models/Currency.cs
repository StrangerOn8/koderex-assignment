using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    public class Currency {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public IEnumerable<Tender> Coins { get; set; }
    }
}
