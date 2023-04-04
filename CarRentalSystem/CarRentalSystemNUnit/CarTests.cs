using NUnit.Framework;

namespace sf.systems.rentals.cars.tests
{
    [TestFixture]
    public class CarTests
    {
        private Car car;

        [SetUp]
        public void Setup()
        {
            // Create a new instance of a Car object for each test
            car = new Car("1234", "Toyota", "Camry", 2022, 50, true);
        }

        [Test]
        public void Serialize_ReturnsValidString()
        {
            // Arrange
            string expected = "1234,Toyota,Camry,2022,50,True";

            // Act
            string result = car.Serialize(DataType.CSV);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void DeserializeHandler_ReturnsValidCarObject()
        {
            // Arrange
            string data = "1234,Toyota,Camry,2022,50,True";

            // Act
            Car result = car.DeserializeHandler(data, DataType.CSV);

            // Assert
            Assert.AreEqual(car.Id, result.Id);
            Assert.AreEqual(car.Make, result.Make);
            Assert.AreEqual(car.Model, result.Model);
            Assert.AreEqual(car.Year, result.Year);
            Assert.AreEqual(car.DailyPrice, result.DailyPrice);
            Assert.AreEqual(car.Rented, result.Rented);
        }
    }
}
