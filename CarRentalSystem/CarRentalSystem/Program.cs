using System;
using System.Linq;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    class Program
    {
        static void Main(string[] args)
        {
            // init command
            string cmd = "new_car"; // "register_customers"; // args[0].ToLower();

            // Initialize system 
            var carRentalSystem = new CarRentalSystem();

            // Load the data from disk
            carRentalSystem.LoadData();

            switch (cmd)
            {
                case "new_car":
                    break;
                case "register_customers":
                    registerCustomers(carRentalSystem);
                    break;
                case "rent_car":
                    rentCar(carRentalSystem);
                    break;
                case "return_car":
                    carRentalSystem.ReturnCar(getFirstCustomer(carRentalSystem), getFirstCar(carRentalSystem), DateTime.Now);
                    break;
            }


            // Save the data to disk
            carRentalSystem.SaveData();
        }

        private static void rentCar(CarRentalSystem carRentalSystem)
        {
            Car firstCar = getFirstCar(carRentalSystem);
            Customer firstCustomer = getFirstCustomer(carRentalSystem);

            // Rent fisrt avaliable car by first customer for 3 days
            if (firstCar != null && firstCustomer != null)
                carRentalSystem.RentCar(firstCustomer, firstCar, DateTime.Now, DateTime.Now.AddDays(3));
            else
            {
                if (firstCar == null) carRentalSystem.LogAndShowMessage("No cars avaliable!");
                if (firstCustomer == null) carRentalSystem.LogAndShowMessage("No registered customers!");
            }
        }

        private static Customer getFirstCustomer(CarRentalSystem carRentalSystem)
        {
            return carRentalSystem.ListRegisteredCustomers().FirstOrDefault();
        }

        private static Car getFirstCar(CarRentalSystem carRentalSystem)
        {
            return carRentalSystem.ListAvailableCars().FirstOrDefault();
        }

        private static void registerCustomers(CarRentalSystem system)
        {
            // Register a new customer
            system.RegisterCustomer("C001", "John Smith", "555-1234", "123 Main St", "jsmith@example.com");

            // Register another customer
            system.RegisterCustomer("C002", "Jane Doe", "555-5678", "456 Elm St", "jdoe@example.com");
        }
    }
}
