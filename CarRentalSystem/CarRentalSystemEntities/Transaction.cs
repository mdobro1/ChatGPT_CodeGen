using Newtonsoft.Json;
using System;

namespace sf.systems.rentals.cars
{
    public class Transaction : ISerializedExtendedEntity<Transaction>, ISerializeOwner
    {
        public string Id { get; set; }
        public Customer Customer { get; set; }
        public Car Car { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public double TotalPrice { get; set; }
        public DateTime ClosedDate { get; private set; }
        public bool IsClosed { get; private set; }
        public IEntitiesList Owner { get; private set; }

        public Transaction() { Id = "?"; }

        public Transaction(string id, Customer customer, Car car, DateTime rentalDate, DateTime returnDate, DateTime closedDate, bool isClosed)
        {
            Id = id;
            Car = car;
            Customer = customer;
            RentalDate = rentalDate;
            ReturnDate = returnDate;
            ClosedDate = closedDate;
            IsClosed = isClosed;
            TotalPrice = CalculateTotalPrice();
        }

        public void CloseTransaction(IEntitiesList entitiesList)
        {
            // validate entities source
            if (entitiesList == null) throw new ArgumentNullException("entitiesList");

            // close transaction
            Customer.ReturnCar(Car);
            IsClosed = true;
            ClosedDate = DateTime.Now;
            entitiesList.ReturnCar(Car);
            entitiesList.ArchiveTransaction(this);
        }

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
            entitiesList.RentCar(car);
            entitiesList.NewTransaction(newTransaction);
            return newTransaction;
        }

        public double CalculateTotalPrice()
        {
            TimeSpan rentalPeriod = ReturnDate - RentalDate;
            var totalDays = (int)rentalPeriod.TotalDays;
            if (totalDays < 1) totalDays = 1;
            return totalDays * Car.DailyPrice;
        }

        public string Serialize(DataType dataType) => dataType switch
        {
            DataType.CSV =>
                $"{Id},{Customer.Id},{Car.Id}," +
                $"{RentalDate.ToShortDateString()}," +
                $"{ReturnDate.ToShortDateString()}," +
                $"{ClosedDate.ToShortDateString()}," +
                $"{IsClosed}",

            DataType.JSON => JsonConvert.SerializeObject(this),
            _ => throw new ArgumentException($"Unknown data type {dataType}."),
        };

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
                customer = defaultCustomer(customerId);
                car = defaultCar(carId);
            }
            else
            {
                customer = entitiesList.LookupCustomer(customerId);
                if (customer == null)
                    customer = defaultCustomer(customerId);
                else
                    customer.RentedCarsPoolNew(entitiesList.GetRentedCars(customer));

                car = entitiesList.LookupCar(carId);
                if (car == null) car = defaultCar(carId);
            }

            DateTime rentalDate = DateTime.Parse(values[3]);
            DateTime returnDate = DateTime.Parse(values[4]);
            DateTime closedDate = DateTime.Parse(values[5]);
            bool isClosed = bool.Parse(values[6]);

            return new Transaction(id, customer, car, rentalDate, returnDate, closedDate, isClosed);

        }

        private static Car defaultCar(string carId)
        {
            return new Car(carId, "", "", 0, 0.0, false);
        }

        private static Customer defaultCustomer(string customerId)
        {
            return new Customer(customerId, "", "", "", "");
        }

        public Transaction DeserializeHandler(string data, DataType dataType,
            IEntitiesList entitiesList)
        {
            return Deserialize(data, dataType, entitiesList);
        }

        public Transaction DeserializeHandler(string data, DataType dataType)
        {
            return Deserialize(data, dataType, Owner);
        }

        public void AssignOwner(IEntitiesList entitiesList)
        {
            Owner = entitiesList;
        }
    }
}