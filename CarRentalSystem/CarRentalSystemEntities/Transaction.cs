using Newtonsoft.Json;
using System;

namespace sf.systems.rentals.cars
{
    public class Transaction : ISerializedEntity<Transaction>
    {
        public string Id { get; set; }
        public Customer Customer { get; set; }
        public Car Car { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public double TotalPrice { get; set; }

        public Transaction() { }

        public Transaction(string id, Customer customer, Car car, DateTime rentalDate, DateTime returnDate, double totalPrice)
        {
            Id = id;
            Customer = customer;
            Car = car;
            RentalDate = rentalDate;
            ReturnDate = returnDate;
            TotalPrice = totalPrice;
        }

        public double CalculateTotalPrice()
        {
            TimeSpan rentalPeriod = ReturnDate - RentalDate;
            return rentalPeriod.TotalDays * Car.DailyPrice;
        }

        public string Serialize(DataType dataType) => dataType switch
        {
            DataType.CSV => $"{Id},{Customer.Id},{Car.Id},{RentalDate.ToShortDateString()},{ReturnDate.ToShortDateString()},{TotalPrice}",
            DataType.JSON => JsonConvert.SerializeObject(this),
            _ => throw new ArgumentException($"Unknown data type {dataType}."),
        };

        public static Transaction Deserialize(string data, DataType dataType) => dataType switch
        {
            DataType.CSV => CreateFromCsv(data),
            DataType.JSON => JsonConvert.DeserializeObject<Transaction>(data),
            _ => throw new ArgumentException($"Unknown data type {dataType}."),
        };

        private static Transaction CreateFromCsv(string csv)
        {
            string[] values = csv.Split(',');
            if (values.Length != 6)
            {
                throw new ArgumentException($"Invalid CSV data: {csv}");
            }
            string id = values[0];
            Customer customer = new Customer(values[1], "", "", "", "");
            Car car = new Car(values[2], "", "", 0, 0.0, false);
            DateTime rentalDate = DateTime.Parse(values[3]);
            DateTime returnDate = DateTime.Parse(values[4]);
            double totalPrice = double.Parse(values[5]);
            return new Transaction(id, customer, car, rentalDate, returnDate, totalPrice);
        }

        public Transaction DeserializeHandler(string data, DataType dataType)
        {
            return Deserialize(data, dataType);
        }
    }
}
