using NUnit.Framework;
using System.Collections.Generic;
using sf.systems.rentals.cars;

namespace sf.systems.rentals.cars.tests
{
    public class CustomerTests
    {
        [Test]
        public void Serialize_ReturnsCorrectCSVFormat()
        {
            // Arrange
            ICustomer customer = new Customer("1", "John Doe", "555-1234", "123 Main St", "johndoe@example.com");
            string expected = "1,John Doe,555-1234,123 Main St,johndoe@example.com";

            // Act
            string result = customer.Serialize(DataType.CSV);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void DeserializeHandler_ReturnsCustomerInstance()
        {
            // Arrange
            string csvData = "1,John Doe,555-1234,123 Main St,johndoe@example.com";

            // Act
            Customer result = new Customer().DeserializeHandler(csvData, DataType.CSV);

            // Assert
            Assert.IsInstanceOf<Customer>(result);
            Assert.AreEqual("1", result.Id);
            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual("555-1234", result.PhoneNumber);
            Assert.AreEqual("123 Main St", result.Address);
            Assert.AreEqual("johndoe@example.com", result.Email);
        }

        [Test]
        public void RentedCarsPoolExtend_AddsNewCarsToList()
        {
            // Arrange
            ICustomer customer = new Customer("1", "John Doe", "555-1234", "123 Main St", "johndoe@example.com");
            List<Car> cars = new List<Car>() { new Car("1", "Honda", "Civic", 2020, 30, false) };

            // Act
            customer.RentedCarsPoolExtend(cars);

            // Assert
            Assert.AreEqual(1, customer.RentedCarsCopy.Count);
            Assert.AreEqual("Honda", customer.RentedCarsCopy[0].Make);
        }

        [Test]
        public void RentedCarsPoolNew_ReplacesOldCarsWithNewCars()
        {
            // Arrange
            ICustomer customer = new Customer("1", "John Doe", "555-1234", "123 Main St", "johndoe@example.com");
            List<Car> oldCars = new List<Car>() { new Car("1", "Honda", "Civic", 2020, 30, false) };
            List<Car> newCars = new List<Car>() { new Car("2", "Toyota", "Corolla", 2021, 35, false) };
            customer.RentedCarsPoolExtend(oldCars);

            // Act
            customer.RentedCarsPoolNew(newCars);

            // Assert
            Assert.AreEqual(1, customer.RentedCarsCopy.Count);
            Assert.AreEqual("Toyota", customer.RentedCarsCopy[0].Make);
        }
    }
}
