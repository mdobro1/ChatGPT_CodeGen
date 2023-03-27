using NUnit.Framework;
using sf.systems.rentals.cars;
using System.Linq;

namespace sf.systems.rentals.cars.tests
{
    [TestFixture]
    public class CustomerTests
    {
        private const string CustomerCsvData = "CUS1,John Doe,555-1234,123 Main St,john.doe@example.com";
        private const string CustomerJsonData = "{\"Id\":\"CUS1\",\"Name\":\"John Doe\",\"PhoneNumber\":\"555-1234\",\"Address\":\"123 Main St\",\"Email\":\"john.doe@example.com\",\"RentedCars\":[]}";

        [Test]
        public void TestCreateCustomerFromCsv()
        {
            Customer customer = Customer.Deserialize(CustomerCsvData, DataType.CSV);
            Assert.AreEqual("CUS1", customer.Id);
            Assert.AreEqual("John Doe", customer.Name);
            Assert.AreEqual("555-1234", customer.PhoneNumber);
            Assert.AreEqual("123 Main St", customer.Address);
            Assert.AreEqual("john.doe@example.com", customer.Email);
            Assert.IsEmpty(customer.RentedCars);
        }

        [Test]
        public void TestCreateCustomerFromJson()
        {
            Customer customer = Customer.Deserialize(CustomerJsonData, DataType.JSON);
            Assert.AreEqual("CUS1", customer.Id);
            Assert.AreEqual("John Doe", customer.Name);
            Assert.AreEqual("555-1234", customer.PhoneNumber);
            Assert.AreEqual("123 Main St", customer.Address);
            Assert.AreEqual("john.doe@example.com", customer.Email);
            Assert.IsEmpty(customer.RentedCars);
        }

        [Test]
        public void TestSerializeCustomerToCsv()
        {
            Customer customer = new Customer("CUS1", "John Doe", "555-1234", "123 Main St", "john.doe@example.com");
            string csvData = customer.Serialize(DataType.CSV);
            Assert.AreEqual(CustomerCsvData, csvData);
        }

        [Test]
        public void TestSerializeCustomerToJson()
        {
            Customer customer = new Customer("CUS1", "John Doe", "555-1234", "123 Main St", "john.doe@example.com");
            string jsonData = customer.Serialize(DataType.JSON);
            Assert.AreEqual(CustomerJsonData, jsonData);
        }

        [Test]
        public void TestRentCar()
        {
            Customer customer = new Customer("CUS1", "John Doe", "555-1234", "123 Main St", "john.doe@example.com");
            Car car = new Car("CAR1", "Toyota", "Corolla", 2022, 40.0, false);
            customer.RentCar(car);
            Assert.IsTrue(customer.RentedCars.Contains(car));
            Assert.IsTrue(car.Rented);
        }

        [Test]
        public void TestReturnCar()
        {
            Customer customer = new Customer("CUS1", "John Doe", "555-1234", "123 Main St", "john.doe@example.com");
            Car car = new Car("CAR1", "Toyota", "Corolla", 2022, 40.0, true);
            customer.RentCar(car); // Rent the car first
            customer.ReturnCar(car); // Return the car
            Assert.IsFalse(customer.RentedCars.Contains(car));
            Assert.IsFalse(car.Rented);
        }
    }
}
