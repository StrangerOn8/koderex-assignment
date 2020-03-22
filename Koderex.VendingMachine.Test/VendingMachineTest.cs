using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Koderex.VendingMachine.Implementation;
using Koderex.VendingMachine.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Koderex.VendingMachine.Test {
    public class VendingMachineTest {
        public readonly IServiceCollection ServiceCollection = new ServiceCollection();
        public IContainer Container { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public ContainerBuilder Builder = new ContainerBuilder();
        private readonly VendingMachineService _service;
        private readonly ITestOutputHelper _output;
        public VendingMachineTest(ITestOutputHelper output) {
            _output = output;
            _output.WriteLine("VendingMachineTest Constructor running");
            configureServices(ServiceCollection);
            _service = (VendingMachineService)ServiceProvider.GetService(typeof(IVendingMachineService));
        }
        private void configureServices(IServiceCollection services) {
            buildAutofacContainer(services);
            ServiceProvider = new AutofacServiceProvider(Container);
        }

        private void buildAutofacContainer(IServiceCollection services) {
            Builder.RegisterType<VendingMachineService>().As<IVendingMachineService>();
            Builder.Populate(services);
            this.Container = Builder.Build();
        }

        [Fact]
        public void Pos_PurchaseItemTest() {
            _output.WriteLine("Executing a positive test where all parameters provided are valid.");
            var result = _service.PurchaseItem(new Models.PurchaseItemRequest {
                Alpha2Code = "us",
                TenderAmount = 1.59M,
                Position = new Models.MachineItemPosition {
                    Row = "A",
                    Column = 1
                }
            });
            _output.WriteLine(result.Message);
            Assert.True(result.IsSuccessful);
        }
        [Fact]
        public void Neg_Alpha2_PurchaseItemTest() {
            _output.WriteLine("Executing a negative test where the alpha 2 code provided is unsupported.");
            var result = _service.PurchaseItem(new Models.PurchaseItemRequest {
                Alpha2Code = "zar",
                TenderAmount = 1.59M,
                Position = new Models.MachineItemPosition {
                    Row = "A",
                    Column = 1
                }
            });
            _output.WriteLine(result.Message);
            Assert.True(!result.IsSuccessful);
        }
        [Fact]
        public void Neg_RowExistence_PurchaseItemTest() {
            _output.WriteLine("Executing a negative test where the row provided does not exist.");
            var result = _service.PurchaseItem(new Models.PurchaseItemRequest {
                Alpha2Code = "gb",
                TenderAmount = 1.59M,
                Position = new Models.MachineItemPosition {
                    Row = "x",
                    Column = 1
                }
            });
            _output.WriteLine(result.Message);
            Assert.True(!result.IsSuccessful);
        }
        [Fact]
        public void Neg_ColumnExistance_PurchaseItemTest() {
            _output.WriteLine("Executing a negative test where the column provided does not exist.");
            var result = _service.PurchaseItem(new Models.PurchaseItemRequest {
                Alpha2Code = "us",
                TenderAmount = 1.59M,
                Position = new Models.MachineItemPosition {
                    Row = "A",
                    Column = 9
                }
            });
            _output.WriteLine(result.Message);
            Assert.True(!result.IsSuccessful);
        }
        [Fact]
        public void Neg_TenderAmountNotEnough_PurchaseItemTest() {
            _output.WriteLine("Executing a negative test where the tender amount provided isn't enough for the product.");
            var result = _service.PurchaseItem(new Models.PurchaseItemRequest {
                Alpha2Code = "us",
                TenderAmount = 0.02M,
                Position = new Models.MachineItemPosition {
                    Row = "A",
                    Column = 1
                }
            });
            _output.WriteLine(result.Message);
            Assert.True(!result.IsSuccessful);
        }
        [Fact]
        public void Neg_ForceStackOverFlow_PurchaseItemTest() {
            // The correct way to manage this would be to limit the amount of tender a user can provide. 
            _output.WriteLine("Executing a negative test where the application will crash, due to stack overflow.");
            var result = _service.PurchaseItem(new Models.PurchaseItemRequest {
                Alpha2Code = "gb",
                TenderAmount = 10000.59M,
                Position = new Models.MachineItemPosition {
                    Row = "A",
                    Column = 1
                }
            });
            _output.WriteLine(result.Message);
            Assert.True(!result.IsSuccessful);
        }
    }
}
