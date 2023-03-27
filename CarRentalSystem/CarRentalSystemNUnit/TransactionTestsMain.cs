using NUnit.Framework;
using sf.systems.rentals.cars;
using System;

namespace sf.systems.rentals.cars.tests
{
    [TestFixture]
    public partial class TransactionTests
    {
        private Customer customer;
        private Car car;

        [SetUp]
        public void SetUp()
        {
            customer = new Customer("CUS1", "John Doe", "555-1234", "123 Main St", "john.doe@example.com");
            car = new Car("CAR1", "Toyota", "Corolla", 2022, 40.0, false);
        }

        [Test]
        public void TestCreateTransaction()
        {
            DateTime rentalDate = new DateTime(2023, 3, 23);
            DateTime returnDate = new DateTime(2023, 3, 24);
            double totalPrice = 80.0;
            Transaction transaction = new Transaction("TRN1", customer, car, rentalDate, returnDate, totalPrice);
            Assert.AreEqual("TRN1", transaction.Id);
            Assert.AreEqual(customer, transaction.Customer);
            Assert.AreEqual(car, transaction.Car);
            Assert.AreEqual(rentalDate, transaction.RentalDate);
            Assert.AreEqual(returnDate, transaction.ReturnDate);
            Assert.AreEqual(totalPrice, transaction.TotalPrice);
        }

        [Test]
        public void TestCalculateTotalPrice()
        {
            DateTime rentalDate = new DateTime(2023, 3, 23);
            DateTime returnDate = new DateTime(2023, 3, 26);
            double totalPrice = 120.0;
            Transaction transaction = new Transaction("TRN1", customer, car, rentalDate, returnDate, 0);
            Assert.AreEqual(totalPrice, transaction.CalculateTotalPrice());
        }
    }
}
