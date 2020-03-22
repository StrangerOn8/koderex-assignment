using Koderex.VendingMachine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Koderex.VendingMachine.Implementation {
    /// <summary>
    /// A representation of a vending machine.
    /// </summary>
    public class Machine {
        private readonly VendingMachineItems _vendingMachineItems;
        private readonly List<int> _coinDenominations = new List<int>();
        private readonly string _currencySymbol;
        public Machine(VendingMachineItems vendingMachineItems, Currency currency) {
            _vendingMachineItems = vendingMachineItems;
            foreach (Tender tender in currency.Coins) {
                _coinDenominations.Add(tender.Value);
            }
            _coinDenominations = _coinDenominations.OrderByDescending(x => x).ToList();
            _currencySymbol = currency.Symbol;
        }
        /// <summary>
        /// get the item from the machine based on the position in the machine it is in.
        /// </summary>
        /// <param name="position">The position of the item in the vending machine,</param>
        /// <param name="tenderAmount">The amount an individual has put in the vending machine.</param>
        /// <returns>decimal</returns>
        public decimal ProcessPurchase(MachineItemPosition position, decimal tenderAmount) {
            MachineItem item = TryFindMachineItem(position);
            if (item == null) {
                throw new ApplicationException($"Unable to find the vending machine item at position: {position.ToString()}.");
            }
            if (tenderAmount < item.Price) {
                throw new ApplicationException($"The amount provided: {_currencySymbol} {tenderAmount}, is less than the required amount of: {_currencySymbol} {item.Price} for the {item.Item}");
            }
            return calculateChange(item.Price, tenderAmount);
        }
        /// <summary>
        /// call all machine item categories to try and find the item you are looking for.
        /// </summary>
        /// <param name="position">The position in the vending macine of the item.</param>
        /// <returns><see cref="MachineItem"/></returns>
        private MachineItem TryFindMachineItem(MachineItemPosition position) {
            var chipsTask = Task.Run(() => tryChips(position));
            var chocolatesTask = Task.Run(() => tryChocolates(position));
            var sweetsTask = Task.Run(() => trySweets(position));
            var drinksTask = Task.Run(() => tryDrinks(position));
            Task.WaitAll(chipsTask, chocolatesTask, sweetsTask, drinksTask);
            if (chipsTask.Result != null) {
                return chipsTask.Result;
            }
            if (chocolatesTask.Result != null) {
                return chocolatesTask.Result;
            }
            if (sweetsTask.Result != null) {
                return sweetsTask.Result;
            }
            if (drinksTask.Result != null) {
                return drinksTask.Result;
            }
            return null;
        }
        private MachineItem tryChips(MachineItemPosition position) {
            var chipsItem = _vendingMachineItems.Chips.ToList().Find(x => x.Position.Equals(position));
            if (chipsItem == null) {
                return null;
            }
            return chipsItem;
        }
        private MachineItem tryChocolates(MachineItemPosition position) {
            var chipsItem = _vendingMachineItems.Chocolates.ToList().Find(x => x.Position.Equals(position));
            if (chipsItem == null) {
                return null;
            }
            return chipsItem;
        }
        private MachineItem trySweets(MachineItemPosition position) {
            var chipsItem = _vendingMachineItems.Sweets.ToList().Find(x => x.Position.Equals(position));
            if (chipsItem == null) {
                return null;
            }
            return chipsItem;
        }
        private MachineItem tryDrinks(MachineItemPosition position) {
            var chipsItem = _vendingMachineItems.Drinks.ToList().Find(x => x.Position.Equals(position));
            if (chipsItem == null) {
                return null;
            }
            return chipsItem;
        }
        /// <summary>
        /// get the amount of change due, by subtracting the price from the tender amount
        /// </summary>
        /// <param name="price">Thie price of the item.</param>
        /// <param name="tenderAmount">The amount an individual has put in the vending machine.</param>
        /// <returns>decimal</returns>
        private decimal calculateChange(decimal price, decimal tenderAmount) {
            if (price.Equals(tenderAmount)) {
                return 0.0M;
            }
            return tenderAmount - price;
        }
        /// <summary>
        /// this function iterates through the provided coin denominations
        /// </summary>
        /// <param name="changeDue">the amount of change due.
        /// used to calculate how many of each coin must be provided as change.
        /// </param>
        /// <returns>List<<see cref="Change"/>></returns>
        public List<Change> GiveCoins(decimal changeDue) {
            List<Change> denominations = new List<Change>();
            foreach (int coin in _coinDenominations) {
                int total = 0;
                Change change = calculateOwedCoins(ref changeDue, ref total, coin);
                if (change != null) {
                    denominations.Add(change);
                }
            }
            return denominations;
        }
        /// <summary>
        /// recursively calculate the change due according to the current coin, by converting the coin to a decimal and subtracting it from the the change due.
        /// if the change due is greater than, or equal to the coin value then the function will execute again.
        /// if the change due is a zero decimal value the function will return null.
        /// if the change due is less than zero the function will return null. If this happens the change due will be set back to original value, to ensure the next iteration can try and calculate
        /// </summary>
        /// <param name="changeDue">a by ref value to represent the change due, which will be used to determine how many times a coin can go into the value.</param>
        /// <param name="total">a by ref value used to get the count of how many times a perticular coin goes into the change due amount.</param>
        /// <param name="coin">the current coin.</param>
        /// <returns>null || Change <see cref="Change"/></returns>
        private Change calculateOwedCoins(ref decimal changeDue, ref int total, int coin) {
            decimal originalValue = changeDue;
            if (changeDue == 0.0M) {
                return null;
            }
            string stringDecimal = (coin < 10) ? $"0.0{coin}" : $"0.{coin}";
            decimal coinDecimal = Convert.ToDecimal(stringDecimal);
            changeDue = changeDue - coinDecimal;
            if (changeDue < 0.0M) {
                // This is here incase it is only half way through the coins and goes under the 0.0M, so the next iteration on the calling function can try resolve it with a smaller coin.
                changeDue = originalValue;
                return null;
            }
            total++;
            if (changeDue >= coinDecimal) {
                RuntimeHelpers.EnsureSufficientExecutionStack();
                calculateOwedCoins(ref changeDue, ref total, coin);
            }
            return new Change {
                Coin = coin,
                Total = total
            };
        }
    }
}
