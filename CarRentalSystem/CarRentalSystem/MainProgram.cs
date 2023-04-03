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
            // cmd=return_car customer="C001"

            // init
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
                CarRentalCommands.LoadData(context);

                // run command
                switch (argCommand)
                {
                    // production commands
                    case "add_car":
                        if (argCar == null || argCar.Length != 5)
                        {
                            logAndShow(context, "\nERROR: Invalid car data provided! Usage: cmd=add_car car=\"ID,Make,Model,Year,PricePerDay\"");
                            return -1;
                        }
                        CarRentalCommands.AddCar(context, argCar);
                        break;
                    case "delete_car":
                        if (argCar == null || argCar.Length != 1)
                        {
                            logAndShow(context, "\nERROR: Invalid car ID provided! Usage: cmd=delete_car car=\"ID\"");
                            return -1;
                        }
                        CarRentalCommands.DeleteCar(context, argCar[0]);
                        break;
                    case "register_customer":
                        if (argCustomer == null || argCustomer.Length != 5)
                        {
                            logAndShow(context, "\nERROR: Invalid customer data provided! Usage: cmd=register_customer customer=\"ID,Name,PhoneNumber,Address,Email\"");
                            return -1;
                        }
                        CarRentalCommands.RegisterCustomer(context, argCustomer);
                        break;
                    case "delete_customer":
                        if (argCustomer == null || argCustomer.Length != 1)
                        {
                            logAndShow(context, "\nERROR: Invalid customer ID provided! Usage: cmd=delete_customer customer=\"ID\"");
                            return -1;
                        }
                        CarRentalCommands.DeleteCustomer(context, argCustomer[0]);
                        break;
                    case "rent_car":
                        if (argCar == null || argCar.Length != 1 || argCustomer == null || argCustomer.Length != 1)
                        {
                            logAndShow(context, "\nERROR: Invalid car or customer ID provided! Usage: cmd=rent_car car=\"ID\" customer=\"ID\"");
                            return -1;
                        }
                        context.CarID = argCar[0];
                        context.CustomerID = argCustomer[0];
                        context = CarRentalCommands.RentCar(context);
                        break;
                    case "return_car":
                        if (argCustomer == null || argCustomer.Length != 1)
                        {
                            logAndShow(context, "\nERROR: Invalid customer ID provided! Usage: cmd=return_car customer=\"ID\"");
                            return -1;
                        }
                        context.CustomerID = argCustomer[0];
                        context = CarRentalCommands.ReturnCar(context);
                        break;

                    // test commands
                    case "add_cars_test":
                        addCarsTest(context);
                        break;
                    case "register_customers_test":
                        registerCustomersTest(context);
                        break;
                    case "rent_car_test":
                        context = CarRentalCommands.RentCar(context);
                        break;
                    case "return_car_test":
                        context = CarRentalCommands.ReturnCar(context);
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

                CarRentalCommands.SaveData(context);
                return 0;
            }
            catch (Exception ex)
            {
                logAndShow(context, $"\nERROR: {ex.Message} \n\nSTACK-TRACE: {ex.StackTrace}");
                CarRentalCommands.SaveData(context);

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
