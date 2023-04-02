using System;

namespace sf.systems.rentals.cars
{
    public interface ITransaction
    {
        Car Car { get; set; }
        DateTime ClosedDate { get; }
        Customer Customer { get; set; }
        string Id { get; set; }
        bool IsClosed { get; }
        IEntitiesList Owner { get; }
        DateTime RentalDate { get; set; }
        DateTime ReturnDate { get; set; }
        double TotalPrice { get; set; }

        void AssignOwner(IEntitiesList entitiesList);
        double CalculateTotalPrice();
        void CloseTransaction(IEntitiesList entitiesList);
        Transaction DeserializeHandler(string data, DataType dataType);
        Transaction DeserializeHandler(string data, DataType dataType, IEntitiesList entitiesList);
        string Serialize(DataType dataType);
    }
}