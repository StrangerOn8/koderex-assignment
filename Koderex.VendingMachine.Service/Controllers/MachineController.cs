using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Koderex.VendingMachine.Interfaces;
using Koderex.VendingMachine.Models;
using Microsoft.AspNetCore.Mvc;
namespace Koderex.VendingMachine.Service.Controllers {
    [Route("api/[controller]")]
    public class MachineController : Controller {
        private readonly IVendingMachineService _vendingMachineService;
        public MachineController(IVendingMachineService vendingMachineService) {
            _vendingMachineService = vendingMachineService;
        }
        [HttpPost("PurchaseItem")]
        public IActionResult PurchaseItem([FromBody] PurchaseItemRequest purchaseItemRequest) {
            var result = _vendingMachineService.PurchaseItem(purchaseItemRequest);
            if (result.IsSuccessful) {
                if (result.Coins != null && result.Coins.Count != 0) {
                    return Ok(result.Coins);
                } else {
                    return NoContent();
                }
            } else {
                return BadRequest(result);
            }
        }
    }
}
