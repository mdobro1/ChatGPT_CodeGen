using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public class Customer : ISerializedEntity<Customer>
    {
        public string Id { get; set; }
        public string Name { get; set;  }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        private readonly List<Car> rentedCars;

        public Customer() { }

        public Customer(string id, string name, string phoneNumber, string address, string email)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            Email = email;
            rentedCars = new List<Car>();
        }

        public List<Car> RentedCars => rentedCars;

        public void RentCar(Car car)
        {
            if (!rentedCars.Contains(car))
            {
                rentedCars.Add(car);
                car.Rent();
            }
        }

        public void ReturnCar(Car car)
        {
            if (rentedCars.Contains(car))
            {
                rentedCars.Remove(car);
                car.Return();
            }
        }

        public string Serialize(DataType dataType) => dataType switch
        {
            DataType.CSV => $"{Id},{Name},{PhoneNumber},{Address},{Email}",
            DataType.JSON => JsonConvert.SerializeObject(this),
            _ => throw new ArgumentException($"Unknown data type {dataType}."),
        };

        public static Customer Deserialize(string data, DataType dataType) => dataType switch
        {
            DataType.CSV => CreateFromCsv(data),
            DataType.JSON => JsonConvert.DeserializeObject<Customer>(data),
            _ => throw new ArgumentException($"Unknown data type {dataType}."),
        };

        private static Customer CreateFromCsv(string csv)
        {
            string[] values = csv.Split(',');
            if (values.Length != 5)
            {
                throw new ArgumentException($"Invalid CSV data: {csv}");
            }
            return new Customer(values[0], values[1], values[2], values[3], values[4]);
        }

        public Customer DeserializeHandler(string data, DataType dataType)
        {
            return Deserialize(data, dataType);
        }
    }
}