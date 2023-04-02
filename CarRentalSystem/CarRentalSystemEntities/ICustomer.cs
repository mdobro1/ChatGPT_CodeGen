using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public interface ICustomer
    {
        string Address { get; set; }
        string Email { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        List<Car> RentedCarsCopy { get; }

        Customer DeserializeHandler(string data, DataType dataType);
        void RentedCarsPoolExtend(List<Car> rentedCars);
        void RentedCarsPoolNew(List<Car> rentedCars);
        string Serialize(DataType dataType);
    }
}