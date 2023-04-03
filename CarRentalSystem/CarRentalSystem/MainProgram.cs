using System;
using System.Linq;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    class MainProgram
    {
        static int Main(string[] args)
        {
            // args samples:
            // cmd=register_customer customer="C003,Mary Jung,777 - 1234,911 Main St,mjung@example.com"
            // cmd=delete_customer customer="C003"
            // cmd=add_car car="CAR911,Porsche,Macant,2023,190.0"
            // cmd=add_car car="CAR11,Audi,A1,2021,90.0"
            // cmd=delete_car car="CAR11"
            // cmd=rent_car car="CAR911" customer="C003"

            // bool test-mode
            var testMode = false;
            string argCommand = default;
            string[] argCustomer = default;
            string[] argCar = default;

            // resolve args
            if (!testMode)
            {
                foreach (var arg in args)
                {
                    var arg_parts = arg.Split("=");
                    // command-args
                    string argKey = arg_parts[0].ToLowerInvariant().Trim();

                    if (arg_parts.Length > 1)
                    {
                        var argValue = arg_parts[1].Trim();

                        switch (argKey)
                        {
                            case "cmd":
                                argCommand = argValue;
                                break;
                            case "car":
                                argCar = argValue.Split(',');
                                break;
                            case "customer":
                                argCustomer = argValue.Split(',');
                                break;
                        }
                    }
                }
            }
            else
            {
                // Test/Debug mode
                argCommand = "add_cars_test"; // 1)
                // argCommand = "register_customers_test"; // 2)
                // argCommand = "rent_car_test"; // 3)
                // argCommand = "return_car_test"; // 4)

                argCustomer = new string[] { "C002" };
                argCar = new string[] { "CAR1" };
            }

            // Initialize car rental system 
            var carRentalSystem = new CarRentalSystem();
            // create rental context
            var context = new CarRentalContext();
            context.CarRentalSystem = carRentalSystem;
            context.Command = argCommand;
            context.CarID = argCar?[0];
            context.CustomerID = argCustomer?[0];

            // Go!
            try
            {
                loadData(context);

                // run command
                switch (argCommand)
                {
                    // production commands
                    case "add_car":
                        addCar(context, argCar);
                        break;
                    case "delete_car":
                        deleteCar(context, argCar[0]);
                        break;
                    case "register_customer":
                        registerCustomer(context, argCustomer);
                        break;
                    case "delete_customer":
                        deleteCustomer(context, argCustomer[0]);
                        break;
                    case "rent_car":
                        context = rentCar(context);
                        break;
                    case "return_car":
                        context = returnCar(context);
                        break;

                    // test commands
                    case "add_cars_test":
                        addCarsTest(context);
                        break;
                    case "register_customers_test":
                        registerCustomersTest(context);
                        break;
                    case "rent_car_test":
                        context = rentCar(context);
                        break;
                    case "return_car_test":
                        context = returnCar(context);
                        break;
                    default:
                        logAndShow(context, $"\nUnknown command: \"{argCommand}\"!\n");
                        showMessage(context, "Use samples:");
                        showMessage(context, "CarRentalSystem cmd=register_customer customer=\"C003,Mary Jung,777 - 1234,911 Main St,mjung@example.com\"");
                        showMessage(context, "CarRentalSystem cmd=delete_customer customer=\"C003\"");
                        showMessage(context, "CarRentalSystem cmd=add_car car=\"CAR911,Porsche,Macant,2023,190.0\"");
                        showMessage(context, "CarRentalSystem cmd=add_car car=\"CAR11,Audi,A1,2021,90.0\"");
                        showMessage(context, "CarRentalSystem cmd=delete_car car=\"CAR11\"");
                        showMessage(context, "CarRentalSystem cmd=rent_car car=\"CAR911\" customer=\"C003\"");
                        return -1;
                }

                saveData(context);
                return 0;
            }
            catch (Exception ex)
            {
                logAndShow(context, $"\nERROR: {ex.Message} \n\nSTACK-TRACE: {ex.StackTrace}");
                saveData(context);

                return -1;
            }
        }

        private static void showMessage(CarRentalContext context, string message)
        {
            context.CarRentalSystem.ShowMessage(message);
        }

        private static void logAndShow(CarRentalContext context, string message)
        {
            context.CarRentalSystem.LogAndShowMessage(message);
        }

        private static void registerCustomer(CarRentalContext context, string[] argCustomer)
        {
            // init
            var rentalSystem = context.CarRentalSystem;
            var customerID = argCustomer[0];

            // go
            var customer = rentalSystem.LookupCustomer(customerID);
            if (customer == null)
            {
                customer = rentalSystem.RegisterCustomer(argCustomer[0], argCustomer[1], argCustomer[2], argCustomer[3], argCustomer[4]);
                context.CarRentalSystem.LogAndShowMessage(
                    $"New Customer \"{customer.Name}\" (ID: {customer.Id}) has been successfully added to the System!");
            }
            else
            {
                context.CarRentalSystem.LogAndShowMessage(
                    $"Customer \"{customer.Name}\" (ID: {customer.Id}) is already registered in the System!");
            }
        }

        private static void deleteCustomer(CarRentalContext context, string customerId)
        {
            // init
            var rentalSystem = context.CarRentalSystem;
            var customer = rentalSystem.LookupCustomer(customerId);

            // go
            if (rentalSystem.DeleteCustomer(customerId))
            {
                context.CarRentalSystem.LogAndShowMessage(
                    $"Customer \"{customer.Name}\" (ID: {customer.Id}) has been successfully deleted from the System!");
            }
            else
            {
                context.CarRentalSystem.LogAndShowMessage(
                    $"Customer with ID: {customerId} has not been found!");
            }
        }

        private static void deleteCar(CarRentalContext context, string carId)
        {
            if (context.CarRentalSystem.DeleteCar(carId))
            {
                context.CarRentalSystem.LogAndShowMessage(
                    $"Car with ID: {carId} has been successfully deleted from the System!");
            }
        }

        private static void addCar(CarRentalContext context, string[] argCar)
        {
            // init
            var carRentalSystem = context.CarRentalSystem;
            var carId = argCar[0];
            var car = carRentalSystem.LookupCar(carId);

            // go!
            if (car == null)
            {
                var newCar = context.CarRentalSystem.AddCar(argCar[0], argCar[1], argCar[2], Convert.ToInt32(argCar[3]), Convert.ToDouble(argCar[4]));
                context.CarRentalSystem.LogAndShowMessage($"New Car with ID: {newCar.Id} has been successfully added to the System!");
            }
            else
                context.CarRentalSystem.LogAndShowMessage($"Car with ID: {car.Id} already exists in the System!");
        }

        private static void saveData(CarRentalContext context)
        {
            // Save the data to disk
            context.CarRentalSystem.LogAndShowMessage("\nSaving data ...");
            context.CarRentalSystem.SaveData();
            context.CarRentalSystem.LogAndShowMessage("All data has been saved.\n");
        }

        private static void loadData(CarRentalContext context)
        {
            // Load the data from disk
            context.CarRentalSystem.LogAndShowMessage("\nLoading data ...");
            context.CarRentalSystem.LoadData();
            context.CarRentalSystem.LogAndShowMessage("All data has been loaded.\n");
        }

        private static CarRentalContext returnCar(CarRentalContext context)
        {
            return CarRentalCommands.ReturnCar(context,
                    (CarRentalContext ctx) =>
                    {
                        context.CarRentalSystem.LogAndShowMessage(
                            $"\nCustomer rental is closed (ID-Customer:{context.CustomerID}, ID-Rental: {context.RentalTransaction.Id}).\n");
                    
                        return ctx;
                    }
                );           
        }

        private static CarRentalContext rentCar(CarRentalContext context)
        {
            return CarRentalCommands.RentCar(context,
                    // empty customer
                    (CarRentalContext ctx) =>
                    {
                        ctx.CarRentalSystem.LogAndShowMessage("Customer-ID is empty!");
                        return ctx;
                    },
                    // car is not avaliable
                    (CarRentalContext ctx) =>
                    {
                        ctx.CarRentalSystem.LogAndShowMessage($"Car with ID:{ctx.CarID} is not avaliable!");
                        return ctx;
                    },
                    // posterior - rent message
                    (CarRentalContext ctx) =>
                    {
                        ctx.CarRentalSystem.LogAndShowMessage(
                            $"\nCar with (ID-Car:{ctx.CarID}) has been rented by Customer (ID-Customer:{ctx.CustomerID}," +
                            $" ID-Rental: {ctx.RentalTransaction.Id}).\n");
                        return ctx;
                    }
                );
        }

        private static void addCarsTest(CarRentalContext context)
        {
            context.CarRentalSystem.AddCar("CAR1", "Toyota", "Corolla", 2022, 50.0);
            context.CarRentalSystem.AddCar("CAR2", "Honda", "Civic", 2021, 45.0);
            context.CarRentalSystem.AddCar("CAR7", "Audi", "Q7", 2020, 120.0);
        }

        private static void registerCustomersTest(CarRentalContext context)
        {
            // Register a new customer
            context.CarRentalSystem.RegisterCustomer("C001", "John Smith", "555-1234", "123 Main St", "jsmith@example.com");

            // Register another customer
            context.CarRentalSystem.RegisterCustomer("C002", "Jane Doe", "555-5678", "456 Elm St", "jdoe@example.com");
        }
    }
}
