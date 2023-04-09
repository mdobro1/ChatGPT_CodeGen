import datetime
from typing import List, Union, Callable
from typing import Callable, Union
from controller.CarRentalContext import CarRentalContext
from controller.CarRentalContextAction import CarRentalContextAction


class CarRentalCommands:
    @staticmethod
    def return_car(rentalContext: CarRentalContext, posteriorReturnCar: CarRentalContextAction) -> CarRentalContext:
        # validation
        CarRentalCommands.validate_context(rentalContext)

        # go!
        rentalContext.RentalTransaction = rentalContext.car_rental_system.return_car(
            rentalContext.car_rental_system.lookup_customer(rentalContext.customer_id))

        # finally
        rentalContext.ActionCompleted = rentalContext.RentalTransaction.is_closed
        
        if rentalContext.ActionCompleted:
            rentalContext = posteriorReturnCar(rentalContext)
        
        return rentalContext

    @staticmethod
    def rent_car(rentalContext: CarRentalContext, 
                notifierEmptyCustomer: CarRentalContextAction, 
                notifierCarIsnotAvaliable: CarRentalContextAction, 
                posteriorReturnCar: CarRentalContextAction) -> CarRentalContext:
        # validation
        CarRentalCommands.validate_context(rentalContext)

        # go!
        # validate params
        validationError = CarRentalCommands.validate_customer(rentalContext, notifierEmptyCustomer)
        car = rentalContext.car_rental_system.get_car(rentalContext.car_id)
        validationError = CarRentalCommands.validate_car(rentalContext, notifierCarIsnotAvaliable, car)
        
        # exit if a validation is occurred
        if validationError: 
            return rentalContext

        # Rent first available car by given customer for 3 days
        rentalContext.rental_transaction = rentalContext.car_rental_system.rent_car(
            rentalContext.customer_id,
            rentalContext.car_id,
            rentalContext.rent_from,
            rentalContext.rent_to)

        rentalContext.action_completed = bool(rentalContext.rental_transaction.id)

        if rentalContext.action_completed: 
            posteriorReturnCar(rentalContext)

        return rentalContext

    @staticmethod
    def register_customer(context: CarRentalContext, argCustomer: List[str]):
        # init
        rentalSystem = context.car_rental_system
        customerID = argCustomer[0]

        # go
        customer = rentalSystem.lookup_customer(customerID)
        if customer is None:
            customer = rentalSystem.register_customer(argCustomer[0], argCustomer[1], argCustomer[2], argCustomer[3], argCustomer[4])
            rentalSystem.log_and_show_message(f'New Customer "{customer.name}" (ID: {customer.id}) has been successfully added to the System!')
        else:
            rentalSystem.log_and_show_message(f'Customer "{customer.name}" (ID: {customer.id}) is already registered in the System!')

    @staticmethod
    def delete_customer(context: CarRentalContext, customerId: str):
        # init
        rentalSystem = context.car_rental_system
        customer = rentalSystem.lookup_customer(customerId)

        # go
        if rentalSystem.delete_customer(customerId):
            rentalSystem.log_and_show_message(f'Customer "{customer.name}" (ID: {customer.id}) has been successfully deleted from the System!')
        else:
            rentalSystem.log_and_show_message(f'Customer with ID: {customerId} has not been found!')

    @staticmethod
    def delete_car(context: CarRentalContext, car_id: str) -> None:
        if context.car_rental_system.delete_car(car_id):
            context.car_rental_system.log_and_show_message(f"Car with ID: {car_id} has been successfully deleted from the system!")

    @staticmethod
    def add_car(context: CarRentalContext, arg_car: List[str]) -> None:
        car_id = arg_car[0]
        car = context.car_rental_system.lookup_car(car_id)
        
        if car is None:
            new_car = context.car_rental_system.add_car(car_id, arg_car[1], arg_car[2], int(arg_car[3]), float(arg_car[4]))
            context.car_rental_system.log_and_show_message(f"New Car with ID: {new_car.id} has been successfully added to the system!")
        else:
            context.car_rental_system.log_and_show_message(f"Car with ID: {car.id} already exists in the system!")

    @staticmethod
    def save_data(context: CarRentalContext) -> None:
        context.car_rental_system.log_and_show_message("\nSaving data ...")
        context.car_rental_system.save_data()
        context.car_rental_system.log_and_show_message("All data has been saved.\n")

    @staticmethod
    def load_data(context: CarRentalContext) -> None:
        context.car_rental_system.log_and_show_message("\nLoading data ...")
        context.car_rental_system.load_data()
        context.car_rental_system.log_and_show_message("All data has been loaded.\n")

    @staticmethod
    def validate_context(rental_context: CarRentalContext) -> None:
        if rental_context is None:
            raise ValueError("rental_context must not be None")
        if rental_context.car_rental_system is None:
            raise ValueError("rental_context.car_rental_system must not be None")
        
    @staticmethod
    def validate_car(rental_context: CarRentalContext, notifier_car_is_not_available: CarRentalContextAction, car) -> bool:
        validation_error = False
        if car is None:
            if notifier_car_is_not_available:
                notifier_car_is_not_available(rental_context)
            validation_error = True
        return validation_error
    
    @staticmethod
    def validate_customer(rental_context: CarRentalContext, notifier_empty_customer: CarRentalContextAction) -> bool:
        validation_error = False
        if not rental_context.customer_id:
            if notifier_empty_customer:
                notifier_empty_customer(rental_context)
            validation_error = True
        return validation_error
