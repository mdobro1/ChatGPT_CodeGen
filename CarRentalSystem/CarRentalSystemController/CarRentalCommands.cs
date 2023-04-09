using System;
using System.Collections.Generic;
using System.Text;

namespace sf.systems.rentals.cars
{
    public static class CarRentalCommands
    {
        public static CarRentalContext ReturnCar(CarRentalContext rentalContext, CarRentalContextAction posteriorReturnCar)
        {
            // validation
            validateContext(rentalContext);

            // go!
            rentalContext.RentalTransaction = rentalContext.CarRentalSystem.ReturnCar(
                rentalContext.CarRentalSystem.LookupCustomer(rentalContext.CustomerID));

            // finally
            rentalContext.ActionCompleted = rentalContext.RentalTransaction.IsClosed;
            
            if (rentalContext.ActionCompleted)
                rentalContext = posteriorReturnCar?.Invoke(rentalContext);
            
            return rentalContext;
        }

        public static CarRentalContext RentCar(CarRentalContext rentalContext, 
            CarRentalContextAction notifierEmptyCustomer,
            CarRentalContextAction notifierCarIsnotAvaliable,
            CarRentalContextAction posteriorReturnCar
            )
        {
            // validation
            validateContext(rentalContext);

            // go!
            // validate params
            var validationError = validateCustomer(rentalContext, notifierEmptyCustomer);
            var car = rentalContext.CarRentalSystem.GetCar(rentalContext.CarID);
            validationError = validateCar(rentalContext, notifierCarIsnotAvaliable, car);
            
            // exit if a validaton is occured
            if (validationError) 
                return rentalContext;

            // Rent fisrt avaliable car by given customer for 3 days
            rentalContext.RentalTransaction = rentalContext.CarRentalSystem.RentCar(
                rentalContext.CustomerID,
                rentalContext.CarID,
                rentalContext.RentFrom,
                rentalContext.RentTo
                );

            rentalContext.ActionCompleted = 
                rentalContext.RentalTransaction.Id != null 
                && rentalContext.RentalTransaction.Id.Trim().Length > 1;

            if (rentalContext.ActionCompleted)
                rentalContext = posteriorReturnCar?.Invoke(rentalContext);

            return rentalContext;
        }

        public static void RegisterCustomer(CarRentalContext context, string[] argCustomer)
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

        public static void DeleteCustomer(CarRentalContext context, string customerId)
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

        public static void DeleteCar(CarRentalContext context, string carId)
        {
            if (context.CarRentalSystem.DeleteCar(carId))
            {
                context.CarRentalSystem.LogAndShowMessage(
                    $"Car with ID: {carId} has been successfully deleted from the System!");
            }
        }

        public static void AddCar(CarRentalContext context, string[] argCar)
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

        public static void SaveData(CarRentalContext context)
        {
            // Save the data to disk
            context.CarRentalSystem.LogAndShowMessage("\nSaving data ...");
            context.CarRentalSystem.SaveData();
            context.CarRentalSystem.LogAndShowMessage("All data has been saved.\n");
        }

        public static void LoadData(CarRentalContext context)
        {
            // Load the data from disk
            context.CarRentalSystem.LogAndShowMessage("\nLoading data ...");
            context.CarRentalSystem.LoadData();
            context.CarRentalSystem.LogAndShowMessage("All data has been loaded.\n");
        }

        public static CarRentalContext ReturnCar(CarRentalContext context)
        {
            return ReturnCar(context,
                    (CarRentalContext ctx) =>
                    {
                        context.CarRentalSystem.LogAndShowMessage(
                            $"\nCustomer rental is closed (ID-Customer:{context.CustomerID}, ID-Rental: {context.RentalTransaction.Id}).\n");

                        return ctx;
                    }
                );
        }

        public static CarRentalContext RentCar(CarRentalContext context)
        {
            return RentCar(context,
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
        
        private static void validateContext(CarRentalContext rentalContext)
        {
            if (rentalContext == null) throw new ArgumentNullException("rentalContext");
            if (rentalContext.CarRentalSystem == null) throw new ArgumentNullException("rentalContext.carRentalSystem");
        }
        
        private static bool validateCar(CarRentalContext rentalContext, CarRentalContextAction notifierCarIsnotAvaliable, object car)
        {
            var validationError = false;

            if (car == null)
            {
                notifierCarIsnotAvaliable?.Invoke(rentalContext);
                validationError = true;
            }

            return validationError;
        }

        private static bool validateCustomer(CarRentalContext rentalContext, CarRentalContextAction notifierEmptyCustomer)
        {
            var validationError = false;

            if (string.IsNullOrEmpty(rentalContext.CustomerID))
            {
                notifierEmptyCustomer?.Invoke(rentalContext);
                validationError = true;
            }

            return validationError;
        }
    }
}
