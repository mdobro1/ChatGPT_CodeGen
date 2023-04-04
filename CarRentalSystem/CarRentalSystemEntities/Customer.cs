using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public class Customer : ISerializedEntity<Customer>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        private readonly int CarRentalMaxLimit = 1;
        internal List<Car> RentedCarsPool { get; }

        public Customer() { }

        public Customer(string id, string name, string phoneNumber, string address, string email)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            Email = email;
            RentedCarsPool = new List<Car>();
        }

        public List<Car> RentedCarsCopy
        {
            get
            {
                var result = new List<Car>();
                result.AddRange(RentedCarsPool);
                return result;
            }
        }

        internal void RentCar(Car car)
        {
            if (car == null) throw new ArgumentNullException("car");
            if (RentedCarsPool.Contains(car)) throw new InvalidOperationException($"Already rented car with ID: {car.Id}!");
            if (RentedCarsPool.Count >= CarRentalMaxLimit) throw new InvalidOperationException(
                $"Customer ({Id}) is not allowed to rent more than {CarRentalMaxLimit} car(s)!");

            car.Rent();
            RentedCarsPool.Add(car);
        }

        internal void ReturnCar(Car car)
        {
            if (RentedCarsPool.Contains(car)) RentedCarsPool.Remove(car);
            car.Return();
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

        public void RentedCarsPoolNew(List<Car> rentedCars)
        {
            RentedCarsPool.Clear();
            RentedCarsPoolExtend(rentedCars);
        }
        public void RentedCarsPoolExtend(List<Car> rentedCars)
        {
            if (rentedCars == null) throw new ArgumentNullException("rentedCars");

            RentedCarsPool.AddRange(rentedCars);
        }
    }
}