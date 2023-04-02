using System;
using System.Collections.Generic;
using System.Text;

namespace sf.systems.rentals.cars
{
    public interface IEntitiesList
    {
        List<Customer> GetRegisteredCustomers();
        Customer LookupCustomer(string customerId);
        List<Car> GetAvaliableCars();
        List<Car> GetRentedCars();
        List<Car> GetRentedCars(Customer customer);
        Car LookupCar(string carId);
        void NewTransaction(Transaction transaction);
        void ArchiveTransaction(Transaction transaction);
        void RentCar(Car car);
        void ReturnCar(Car car);
    }
}