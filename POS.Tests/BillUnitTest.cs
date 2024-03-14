using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using POS.Model;
using POS.ViewModel;
using System.Windows.Input;

namespace POS.Tests
{
    public class BillUnitTest
    {      
        private readonly DbContextOptions<DatabaseContext> dbContextOptions;
        public BillUnitTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "PosDb")
                .Options;
        }

        [Fact]
        public void When_no_orders_placed_bill_values_must_be_zero()
        {
            var dbContext = new DatabaseContext(dbContextOptions);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var bill = new PrintBillVM(dbContext);

            bill.SubTotal.Should().Be(0);
            bill.Tax.Should().Be(0);
            bill.TotalPrice.Should().Be(0);
        }

        [Fact]
        public void When_order_placed_calculate_bill()
        {
            var dbContext = new DatabaseContext(dbContextOptions);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var p1 = new Product { Id = "1", Name = "Bun", Price = 80, Quantity = 10 };
            var p2 = new Product { Id = "2", Name = "Waffle", Price = 120, Quantity = 15 };

            dbContext.AddRange(
                new Order { Id = 1, Product = p1, Quantity = 2, Price = 160 },
                new Order { Id = 2, Product = p2, Quantity = 1, Price = 120 }
            );

            dbContext.SaveChanges();

            var bill = new PrintBillVM(dbContext);

            bill.SubTotal.Should().Be(280);
            bill.Tax.Should().Be(14);
            bill.TotalPrice.Should().Be(294);
        }
    }
}