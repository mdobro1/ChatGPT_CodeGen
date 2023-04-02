using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;

namespace sf.systems.rentals.cars.tests
{
    [TestFixture]
    public class CarRentalSystemTests
    {
        /// <summary>
        /// Test if the system can add new cars
        /// </summary>
        [Test]
        public void TestAddNewCars()
        {
            var carRentalSystem = new CarRentalSystem();
            carRentalSystem.AddCar("CAR1", "Toyota", "Corolla", 2022, 50.0);
            carRentalSystem.AddCar("CAR2", "Honda", "Civic", 2021, 45.0);
            carRentalSystem.AddCar("CAR7", "Audi", "Q7", 2020, 120.0);

            Assert.AreEqual(3, carRentalSystem.ListAvailableCars().Count);
        }

        /// <summary>
        /// Test if the system can register customers
        /// </summary>
        [Test]
        public void TestRegisterCustomers()
        {
            var carRentalSystem = new CarRentalSystem();
            carRentalSystem.RegisterCustomer("C001", "John Smith", "555-1234", "123 Main St", "jsmith@example.com");
            carRentalSystem.RegisterCustomer("C002", "Jane Doe", "555-5678", "456 Elm St", "jdoe@example.com");

            Assert.AreEqual(2, carRentalSystem.ListRegisteredCustomers().Count);
        }

        /// <summary>
        /// Test if the system can rent a car
        /// </summary>
        [Test]
        public void TestRentCar()
        {
            var carRentalSystem = new CarRentalSystem();
            carRentalSystem.RegisterCustomer("C001", "John Smith", "555-1234", "123 Main St", "jsmith@example.com");
            carRentalSystem.AddCar("CAR1", "Toyota", "Corolla", 2022, 50.0);

            var rental = carRentalSystem.RentCar("C001", "CAR1", DateTime.Now, DateTime.Now.AddDays(3));

            var rentedCar = carRentalSystem.ListRentedCars().FirstOrDefault();
            var customer = carRentalSystem.LookupCustomer("C001");

            Assert.IsNotNull(rentedCar);
            Assert.IsTrue(rentedCar.Rented);
            Assert.AreEqual(customer.Id, rental.Customer.Id);
            Assert.AreEqual(rentedCar.Id, rental.Customer.RentedCarsCopy.FirstOrDefault().Id);
        }

        [Test]
        public void TestReturnCar()
        {
            var carRentalSystem = new CarRentalSystem();
            var customer = carRentalSystem.RegisterCustomer("C001", "John Smith", "555-1234", "123 Main St", "jsmith@example.com");
            var car = carRentalSystem.AddCar("CAR1", "Toyota", "Corolla", 2022, 50.0);
            var rental = carRentalSystem.RentCar("C001", "CAR1", DateTime.Now, DateTime.Now.AddDays(3));

            carRentalSystem.ReturnCar(customer, car);
            var availableCar = carRentalSystem.ListAvailableCars().FirstOrDefault();

            Assert.IsTrue(carRentalSystem.ListRentedCars().Count == 0);
            Assert.IsTrue(carRentalSystem.ListAvailableCars().Count == 1);
        }


    }
}
