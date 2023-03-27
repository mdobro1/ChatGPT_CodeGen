using Newtonsoft.Json;
using NUnit.Framework;
using sf.systems.rentals.cars;
using System;

namespace sf.systems.rentals.cars.tests
{
    [TestFixture]
    public partial class TransactionTests
    {
        private const string CsvData = "TRN1,CUS1,CAR1,23.03.2023,26.03.2023,160";
        private const string JsonData = "{\"Id\":\"TRN1\",\"Customer\":{\"Id\":\"CUS1\",\"Name\":\"John Doe\",\"PhoneNumber\":\"555-1234\",\"Address\":\"123 Main St\",\"Email\":\"john.doe@example.com\",\"RentedCars\":[]},\"Car\":{\"Id\":\"CAR1\",\"Make\":\"Toyota\",\"Model\":\"Corolla\",\"Year\":2022,\"DailyPrice\":40.0,\"Rented\":false},\"RentalDate\":\"2023-03-23T00:00:00\",\"ReturnDate\":\"2023-03-26T00:00:00\",\"TotalPrice\":160.0}";

        [Test]
        public void TestSerializeToCsv()
        {
            DateTime rentalDate = new DateTime(2023, 3, 23);
            DateTime returnDate = new DateTime(2023, 3, 26);
            double totalPrice = 160;
            Transaction transaction = new Transaction("TRN1", customer, car, rentalDate, returnDate, totalPrice);
            string csvData = transaction.Serialize(DataType.CSV);
            Assert.AreEqual(CsvData, csvData);
        }

        [Test]
        public void TestSerializeToJson()
        {
            DateTime rentalDate = new DateTime(2023, 3, 23);
            DateTime returnDate = new DateTime(2023, 3, 26);
            double totalPrice = 160;
            Transaction transaction = new Transaction("TRN1", customer, car, rentalDate, returnDate, totalPrice);
            string jsonData = transaction.Serialize(DataType.JSON);
            Assert.AreEqual(JsonData, jsonData);
        }

        [Test]
        public void TestDeserializeFromCsv()
        {
            Transaction transaction = Transaction.Deserialize(CsvData, DataType.CSV);
            Assert.AreEqual("TRN1", transaction.Id);
            Assert.AreEqual("CUS1", transaction.Customer.Id);
            Assert.AreEqual("CAR1", transaction.Car.Id);
            Assert.AreEqual(new DateTime(2023, 3, 23), transaction.RentalDate);
            Assert.AreEqual(new DateTime(2023, 3, 26), transaction.ReturnDate);
            Assert.AreEqual(160, transaction.TotalPrice);
        }

        [Test]
        public void TestDeserializeFromJson()
        {
            Transaction transaction = Transaction.Deserialize(JsonData, DataType.JSON);
            Assert.AreEqual("TRN1", transaction.Id);
            Assert.AreEqual("CUS1", transaction.Customer.Id);
            Assert.AreEqual("CAR1", transaction.Car.Id);
            Assert.AreEqual(new DateTime(2023, 3, 23), transaction.RentalDate);
            Assert.AreEqual(new DateTime(2023, 3, 26), transaction.ReturnDate);
            Assert.AreEqual(160, transaction.TotalPrice);
        }
    }
}
