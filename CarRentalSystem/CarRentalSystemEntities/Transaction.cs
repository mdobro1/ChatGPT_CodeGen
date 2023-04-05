using Newtonsoft.Json;
using System;

namespace sf.systems.rentals.cars
{
    public partial class Transaction : ISerializedExtendedEntity<Transaction>, ISerializeOwner
    {
        #region data
        public string Id { get; set; }
        public Customer Customer { get; set; }
        public Car Car { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public double TotalPrice { get; set; }
        public DateTime ClosedDate { get; private set; }
        public bool IsClosed { get; private set; }
        public IEntitiesList Owner { get; private set; }
        #endregion

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