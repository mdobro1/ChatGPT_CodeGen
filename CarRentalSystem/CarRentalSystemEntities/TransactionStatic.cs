using Newtonsoft.Json;
using System;

namespace sf.systems.rentals.cars
{
    public partial class Transaction 
    {
        public static Transaction OpenTransaction(IEntitiesList entitiesList,
            string customerId, string carId, DateTime rentalDate, DateTime returnDate)
        {
            // validate entities source
            if (entitiesList == null) throw new ArgumentNullException("entitiesList");

            // validation dates
            if (rentalDate.Date < DateTime.Now.Date) throw new ArgumentOutOfRangeException("Rental date cannot be in the past!");
            if (rentalDate.Date > returnDate.Date) throw new ArgumentOutOfRangeException("Rental date cannot be bigger than return date!");

            // get customer
            Customer customer = entitiesList.LookupCustomer(customerId);
            // validate customer 
            if (customer == null) throw new ArgumentNullException($"Customer with ID: {customerId} has not been found!");

            // get car
            Car car = entitiesList.LookupCar(carId);
            // validate car
            if (car == null) throw new ArgumentNullException($"Car with ID: {customerId} has not been found!");

            // generate Tx-ID
            var txID = $"{Guid.NewGuid()}";

            // create rental transaction
            var newTransaction = new Transaction(txID, customer, car, rentalDate, returnDate, default, false);
            newTransaction.Customer.RentCar(car);
            entitiesList.RegisterCarAsRented(car);
            entitiesList.NewTransaction(newTransaction);
            return newTransaction;
        }

        public static Transaction Deserialize(string data, DataType dataType,
            IEntitiesList entitiesList) => dataType switch
            {
                DataType.CSV => CreateFromCsv(data, entitiesList),
                DataType.JSON => JsonConvert.DeserializeObject<Transaction>(data),
                _ => throw new ArgumentException($"Unknown data type {dataType}."),
            };

        private static Transaction CreateFromCsv(string csv,
            IEntitiesList entitiesList)
        {
            string[] values = csv.Split(',');
            if (values.Length < 7)
            {
                throw new ArgumentException($"Invalid CSV data: {csv}");
            }
            string id = values[0];

            Customer customer;
            Car car;

            string customerId = values[1];
            string carId = values[2];

            if (entitiesList == null)
            {
                customer = DefaultCustomer(customerId);
                car = DefaultCar(carId);
            }
            else
            {
                customer = entitiesList.LookupCustomer(customerId);
                if (customer == null)
                    customer = DefaultCustomer(customerId);
                else
                    customer.RentedCarsPoolNew(entitiesList.GetRentedCars(customer));

                car = entitiesList.LookupCar(carId);
                if (car == null) car = DefaultCar(carId);
            }

            DateTime rentalDate = DateTime.Parse(values[3]);
            DateTime returnDate = DateTime.Parse(values[4]);
            DateTime closedDate = DateTime.Parse(values[5]);
            bool isClosed = bool.Parse(values[6]);

            return new Transaction(id, customer, car, rentalDate, returnDate, closedDate, isClosed);

        }

        private static Car DefaultCar(string carId)
        {
            return new Car(carId, "", "", 0, 0.0, false);
        }

        private static Customer DefaultCustomer(string customerId)
        {
            return new Customer(customerId, "", "", "", "");
        }
    }
}