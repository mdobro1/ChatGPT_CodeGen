using System;
using System.Linq;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    class Program
    {
        static void Main(string[] args)
        {
            // command-args
            string cmd;
            string customerId;
            string carId;

            // get command
            if (args.Length > 0)
                cmd = args[0].ToLower();
            else
            {
                // init command
                // cmd = "new_car"; // first run cmd = "new_car"
                // cmd = "register_customers"; // then "register_customers"
                cmd = "rent_car"; // then "rent_car"
                // cmd = "return_car"; // and finally return car
            }

            // get customer
            if (args.Length > 1)
                customerId = args[1];
            else
                customerId = "C002"; 

            // get customer
            if (args.Length > 2)
                carId = args[2];
            else
                carId = "CAR1";

            // Initialize system 
            var carRentalSystem = new CarRentalSystem();
            
            try
            {
                // Load the data from disk
                carRentalSystem.LoadData();

                switch (cmd)
                {
                    case "new_car":
                        addNewCars(carRentalSystem);
                        break;
                    case "register_customers":
                        registerCustomers(carRentalSystem);
                        break;
                    case "rent_car":
                        rentCar(carRentalSystem, customerId, carId);
                        break;
                    case "return_car":
                        returnCar(carRentalSystem, customerId);
                        break;
                }

                // Save the data to disk
                carRentalSystem.SaveData();
            }
            catch (Exception ex)
            {
                carRentalSystem.LogAndShowMessage($"\nERROR: {ex.Message} \n\nSTACK-TRACE: {ex.StackTrace}");
            }
        }

        private static void returnCar(CarRentalSystem carRentalSystem, string idCustomer)
        {
            var rental = carRentalSystem.ReturnCar(
                carRentalSystem.LookupCustomer(idCustomer));

            carRentalSystem.LogAndShowMessage($"\nCustomer rental is closed (ID-Customer:{idCustomer}, ID-Rental: {rental.Id}).\n");
        }

        private static void addNewCars(CarRentalSystem carRentalSystem)
        {
            carRentalSystem.AddCar("CAR1","Toyota", "Corolla", 2022, 50.0);
            carRentalSystem.AddCar("CAR2","Honda", "Civic", 2021, 45.0);
            carRentalSystem.AddCar("CAR7","Audi", "Q7", 2020, 120.0); 
        }

        private static void rentCar(CarRentalSystem carRentalSystem, string idCustomer, string idCar)
        {
            var validationError = false;
            // validate params
            if (string.IsNullOrEmpty(idCustomer))
            {
                carRentalSystem.LogAndShowMessage("Customer-ID is empty!");
                validationError = true;
            }
            var car = carRentalSystem.GetCar(idCar);
            if (car == null)
            {
                carRentalSystem.LogAndShowMessage($"Car with ID:{idCar} is not avaliable!");
                validationError = true;
            }
            if (validationError) return;


            // Rent fisrt avaliable car by given customer for 3 days
            var rental = carRentalSystem.RentCar(
                idCustomer, 
                car.Id, 
                DateTime.Now, 
                DateTime.Now.AddDays(3)
                );

            carRentalSystem.LogAndShowMessage($"\nCar with (ID-Car:{idCar}) has been rented by Customer (ID-Customer:{idCustomer}).\n");
        }

        private static Customer getFirstCustomer(CarRentalSystem carRentalSystem)
        {
            return carRentalSystem.ListRegisteredCustomers().FirstOrDefault();
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
