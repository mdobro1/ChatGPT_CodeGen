using NUnit.Framework;
using sf.systems.rentals.cars;

namespace sf.systems.rentals.cars.tests
{
    [TestFixture]
    public class CarTests
    {
        private const string CarCsvData = "CAR1,Toyota,Corolla,2022,40,False";
        private const string CarJsonData = "{\"Id\":\"CAR1\",\"Make\":\"Toyota\"," 
            + "\"Model\":\"Corolla\",\"Year\":2022,\"DailyPrice\":40.0,\"Rented\":false}";

        [Test]
        public void TestCreateCarFromCsv()
        {
            Car car = Car.Deserialize(CarCsvData, DataType.CSV);
            Assert.AreEqual("CAR1", car.Id);
            Assert.AreEqual("Toyota", car.Make);
            Assert.AreEqual("Corolla", car.Model);
            Assert.AreEqual(2022, car.Year);
            Assert.AreEqual(40, car.DailyPrice);
            Assert.AreEqual(false, car.Rented);
        }

        [Test]
        public void TestCreateCarFromJson()
        {
            Car car = Car.Deserialize(CarJsonData, DataType.JSON);
            Assert.AreEqual("CAR1", car.Id);
            Assert.AreEqual("Toyota", car.Make);
            Assert.AreEqual("Corolla", car.Model);
            Assert.AreEqual(2022, car.Year);
            Assert.AreEqual(40.0, car.DailyPrice);
            Assert.AreEqual(false, car.Rented);
        }

        [Test]
        public void TestSerializeCarToCsv()
        {
            Car car = new Car("CAR1", "Toyota", "Corolla", 2022, 40, false);
            string csvData = car.Serialize(DataType.CSV);
            Assert.AreEqual(CarCsvData, csvData);
        }

        [Test]
        public void TestSerializeCarToJson()
        {
            Car car = new Car("CAR1", "Toyota", "Corolla", 2022, 40, false);
            string jsonData = car.Serialize(DataType.JSON);
            Assert.AreEqual(CarJsonData, jsonData);
        }

        [Test]
        public void TestRentCar()
        {
            Car car = new Car("CAR1", "Toyota", "Corolla", 2022, 40.0, false);
            car.Rent();
            Assert.AreEqual(true, car.Rented);
        }

        [Test]
        public void TestReturnCar()
        {
            Car car = new Car("CAR1", "Toyota", "Corolla", 2022, 40.0, true);
            car.Return();
            Assert.AreEqual(false, car.Rented);
        }
    }
}