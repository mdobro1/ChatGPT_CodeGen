using System;
using NUnit.Framework;

namespace sf.systems.rentals.cars.tests
{
    [TestFixture]
    public class TransactionTests
    {
        private CarRentalSystem carRentalSystem;
        private Customer customer;
        private Car car;

        [SetUp]
        public void Setup()
        {
            carRentalSystem = new CarRentalSystem();
            customer = carRentalSystem.RegisterCustomer("1", "John Doe", "555-1234", "123 Main St", "john.doe@example.com");
            car = new Car("1", "Toyota", "Corolla", 2020, 50, false);
            carRentalSystem.AddCar("1", "Toyota", "Corolla", 2020, 50);
        }

        [Test]
        public void TestTransactionProperties()
        {
            var rentalDate = DateTime.Now;
            var returnDate = rentalDate.AddDays(1);
            var transaction = Transaction.OpenTransaction(carRentalSystem, customer.Id, car.Id, rentalDate, returnDate);

            Assert.AreEqual(customer, transaction.Customer);
            Assert.AreEqual(car.Id, transaction.Car.Id);
            Assert.AreEqual(rentalDate, transaction.RentalDate);
            Assert.AreEqual(returnDate, transaction.ReturnDate);
            Assert.IsFalse(transaction.IsClosed);
            Assert.AreEqual(50, transaction.TotalPrice);
        }

        [Test]
        public void TestCalculateTotalPrice()
        {
            var rentalDate = DateTime.Now;
            var returnDate = rentalDate.AddDays(1);
            var transaction = Transaction.OpenTransaction(carRentalSystem, customer.Id, car.Id, rentalDate, returnDate);

            var expectedPrice = car.DailyPrice * (returnDate - rentalDate).TotalDays;
            var actualPrice = transaction.CalculateTotalPrice();

            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [Test]
        public void TestCloseTransaction()
        {
            var rentalDate = DateTime.Now;
            var returnDate = rentalDate.AddDays(1);
            var transaction = Transaction.OpenTransaction(carRentalSystem, customer.Id, car.Id, rentalDate, returnDate);
            var totalPrice = transaction.CalculateTotalPrice();
            transaction.CloseTransaction(carRentalSystem);

            Assert.IsTrue(transaction.IsClosed);
            Assert.AreEqual(DateTime.Now.Date, transaction.ClosedDate.Date);
            Assert.AreEqual(totalPrice, transaction.TotalPrice);
            Assert.IsFalse(carRentalSystem.ListRentedCars().Find(c => c.Id  == car.Id) != null);
            Assert.IsTrue(carRentalSystem.ListAvailableCars().Find(c => c.Id == car.Id) != null);
        }

        [Test]
        public void TestSerializeDeserialize()
        {
            var rentalDate = DateTime.Now;
            var returnDate = rentalDate.AddDays(1);
            var transaction = Transaction.OpenTransaction(carRentalSystem, customer.Id, car.Id, rentalDate, returnDate);
            var serializedData = transaction.Serialize(DataType.CSV);
            var deserializedTransaction = new Transaction().DeserializeHandler(serializedData, DataType.CSV, carRentalSystem);

            Assert.AreEqual(transaction.Id, deserializedTransaction.Id);
            Assert.AreEqual(transaction.Customer.Id, deserializedTransaction.Customer.Id);
            Assert.AreEqual(transaction.Car.Id, deserializedTransaction.Car.Id);
            Assert.AreEqual(transaction.RentalDate.Date, deserializedTransaction.RentalDate.Date);
            Assert.AreEqual(transaction.ReturnDate.Date, deserializedTransaction.ReturnDate.Date);
            Assert.AreEqual(transaction.IsClosed, deserializedTransaction.IsClosed);
            Assert.AreEqual(transaction.TotalPrice, deserializedTransaction.TotalPrice);
        }
    }
}
