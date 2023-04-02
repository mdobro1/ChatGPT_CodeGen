using Newtonsoft.Json;
using System;

namespace sf.systems.rentals.cars
{
    public class Car : ISerializedEntity<Car>, ICar
    {
        public string Id { get; }
        public string Make { get; }
        public string Model { get; }
        public int Year { get; }
        public double DailyPrice { get; }
        public bool Rented { get; private set; }

        public Car() { }

        public Car(string id, string make, string model, int year, double dailyPrice, bool rented)
        {
            Id = id;
            Make = make;
            Model = model;
            Year = year;
            DailyPrice = dailyPrice;
            Rented = rented;
        }

        internal void Rent() => Rented = true;

        internal void Return() => Rented = false;

        public string Serialize(DataType dataType) => dataType switch
        {
            DataType.CSV => $"{Id},{Make},{Model},{Year},{DailyPrice},{Rented}",
            DataType.JSON => JsonConvert.SerializeObject(this),
            _ => throw new ArgumentException($"Unknown data type {dataType}."),
        };

        public static Car Deserialize(string data, DataType dataType) => dataType switch
        {
            DataType.CSV => CreateFromCsv(data),
            DataType.JSON => JsonConvert.DeserializeObject<Car>(data),
            _ => throw new ArgumentException($"Unknown data type {dataType}."),
        };

        private static Car CreateFromCsv(string csv)
        {
            int csv_len = 6;
            string[] values = csv.Split(',');
            if (values.Length != csv_len)
            {
                throw new ArgumentException($"Invalid CSV data length - expected {csv_len}, got {values.Length}. CSV-Data: {csv}.");
            }
            return new Car(values[0], values[1], values[2], int.Parse(values[3]),
                double.Parse(values[4]), bool.Parse(values[5]));
        }

        public Car DeserializeHandler(string data, DataType dataType)
        {
            return Deserialize(data, dataType);
        }
    }
}
