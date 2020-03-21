using Koderex.VendingMachine.Interfaces;
using Koderex.VendingMachine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Koderex.VendingMachine.Implementation {
    public class VendingMachineService : IVendingMachineService {
        private const string MOCK_VENDING_MACHINE = "vending-machine-items.json";
        private readonly string[] supportedAlpha3Codes = new string[2] { "usd", "gbp" };
        private List<Country> countries = new List<Country>();
        private string directoryPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public VendingMachineService() {
            GetCountryConfig();
        }
        /// <summary>
        /// Get the country config which contains the coin denominations.
        /// </summary>
        private void GetCountryConfig() {
            foreach (string code in supportedAlpha3Codes) {
                var path = Path.Combine(directoryPath, "Config", $"{code}.json");
                using (StreamReader streamReader = new StreamReader(path)) {
                    string json = streamReader.ReadToEnd();
                    countries.Add(JsonConvert.DeserializeObject<Country>(json));
                }
            }
        }
        /// <summary>
        /// Get the vending machine items from configuration.
        /// </summary>
        /// <returns><see cref="VendingMachineItems"/></returns>
        public VendingMachineItems GetVendingMachineItems() {
            var path = Path.Combine(directoryPath, "Mock", MOCK_VENDING_MACHINE);
            using (StreamReader streamReader = new StreamReader(path)) {
                string json = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<Models.VendingMachineItems>(json);
            }
        }
        /// <summary>
        /// get the machine items, and determine which country you are working in to pass to the <see cref="Machine"/> to handle the logic.
        /// </summary>
        /// <param name="request"><see cref="PurchaseItemRequest"/></param>
        /// <returns><see cref="PurchaseItemResponse"/></returns>
        public PurchaseItemResponse PurchaseItem(PurchaseItemRequest request) {
            List<Change> denominations = null;
            List<int> coinsDue = null;
            try {
                VendingMachineItems items = GetVendingMachineItems();
                Country country = countries.Find(x => x.Alpha2Code.ToLower() == request.Alpha2Code.ToLower());
                if (country == null) {
                    throw new ApplicationException($"Unable to find a config set for the country with the Alpha2Code: {request.Alpha2Code}");
                }
                Machine machine = new Machine(items, country.Currency);
                decimal changeDue = machine.ProcessPurchase(request.Position, request.TenderAmount);
                if (changeDue != 0.0M) {
                    denominations = machine.GiveCoins(changeDue);
                    var ordered = denominations.OrderByDescending(x => x.Coin).ToList();
                    coinsDue = new List<int>();
                    foreach (Change change in ordered) {
                        int count = 0;
                        while (count < change.Total) {
                            count++;
                            coinsDue.Add(change.Coin);
                        }
                    }
                }
                return new PurchaseItemResponse {
                    IsSuccessful = true,
                    IsClientFriendlyMessage = true,
                    Message = $"Successfully purchased: {request.Position.ToString()}.{Environment.NewLine}Change due: {country.Currency.Symbol}{changeDue.ToString()}",
                    TotalChangeDue = changeDue,
                    Denominations = denominations,
                    Coins = coinsDue
                };
            } catch (ApplicationException exception) {
                return new PurchaseItemResponse {
                    IsSuccessful = false,
                    IsClientFriendlyMessage = true,
                    Message = exception.Message
                };
            } catch (Exception exception) {
                return new PurchaseItemResponse {
                    IsSuccessful = false,
                    IsClientFriendlyMessage = false,
                    Message = exception.ToString()
                };
            }
        }

    }
}
