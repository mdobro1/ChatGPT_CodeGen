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
                DateTime.Now,
                DateTime.Now.AddDays(3)
                );

            rentalContext.ActionCompleted = 
                rentalContext.RentalTransaction.Id != null 
                && rentalContext.RentalTransaction.Id.Trim().Length > 1;

            if (rentalContext.ActionCompleted)
                rentalContext = posteriorReturnCar?.Invoke(rentalContext);

            return rentalContext;
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
