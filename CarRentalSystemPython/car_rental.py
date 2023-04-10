import datetime
import sys
import argparse
from typing import List
from entities.Transaction import Transaction
from utils.DateUtils import DateUtils
from controller.CarRentalSystem import CarRentalSystem
from controller.CarRentalContext import CarRentalContext
from controller.CarRentalCommands import CarRentalCommands


def show_message(context: CarRentalContext, message: str) -> None:
    context.car_rental_system.show_message(message)


def log_and_show(context: CarRentalContext, message: str) -> None:
    context.car_rental_system.log_and_show_message(message)
    
def main(args) -> int:
    # init
    arg_command = ""
    arg_customer = []
    arg_car = []
    arg_from = []
    arg_to = []

    # resolve args
    for arg in args:
        arg_parts = arg.split("=")
        # command-args
        arg_key = arg_parts[0].lower().strip()

        if len(arg_parts) > 1:
            arg_value = arg_parts[1].strip()

            if arg_key == "cmd":
                arg_command = arg_value
            elif arg_key == "car":
                arg_car = arg_value.split(",")
            elif arg_key == "customer":
                arg_customer = arg_value.split(",")
            elif arg_key == "from":
                arg_from  = arg_value.split(",")
            elif arg_key == "to":
                arg_to  = arg_value.split(",")

    # Initialize car rental system 
    car_rental_system = CarRentalSystem()
    # create rental context
    context : CarRentalContext = CarRentalContext()
    context.car_rental_system = car_rental_system
    context.command = arg_command
    context.car_id = arg_car[0] if len(arg_car) else None
    context.customer_id = arg_customer[0] if len(arg_customer) else None
    now = datetime.datetime.now().date()
    context.rent_from = DateUtils.parse_date(arg_from[0]) if len(arg_from) > 0 else now
    context.rent_to =  DateUtils.parse_date(arg_to[0]) if len(arg_to) > 0 else context.rent_from.replace(day=now.day + 3)

    # Go!
    try:
        CarRentalCommands.load_data(context)

        # run command
        if arg_command == "add_car":
            if not arg_car or len(arg_car) != 5:
                log_and_show(context, "\nERROR: Invalid car data provided! Usage: cmd=add_car car=\"ID,Make,Model,Year,PricePerDay\"")
                sys.exit(-1)
            CarRentalCommands.add_car(context, arg_car)
        elif arg_command == "delete_car":
            if not arg_car or len(arg_car) != 1:
                log_and_show(context, "\nERROR: Invalid car ID provided! Usage: cmd=delete_car car=\"ID\"")
                sys.exit(-1)
            CarRentalCommands.delete_car(context, arg_car[0])
        elif arg_command == "register_customer":
            if not arg_customer or len(arg_customer) != 5:
                log_and_show(context, "\nERROR: Invalid customer data provided! Usage: cmd=register_customer customer=\"ID,Name,PhoneNumber,Address,Email\"")
                sys.exit(-1)
            CarRentalCommands.register_customer(context, arg_customer)
        elif arg_command == "delete_customer":
            if not arg_customer or len(arg_customer) != 1:
                log_and_show(context, "\nERROR: Invalid customer ID provided! Usage: cmd=delete_customer customer=\"ID\"")
                sys.exit(-1)
            CarRentalCommands.delete_customer(context, arg_customer[0])
        elif arg_command == "rent_car":
            if not arg_car or len(arg_car) != 1 or not arg_customer or len(arg_customer) != 1:
                log_and_show(context, "\nERROR: Invalid car or customer ID provided! Usage: cmd=rent_car car=\"ID\" customer=\"ID\"")
                sys.exit(-1)
            context = CarRentalCommands.rent_car(context, 
                                                lambda ctx: show_message(ctx, "Customer-ID is empty!"), 
                                                lambda ctx: show_message(ctx, f"Car with ID:{ctx.car_id} is not avaliable!"),
                                                lambda ctx: show_message(ctx, 
                                                                     f"\nCar with (ID-Car:{ctx.car_id}) has been rented " +
                                                                     f"by Customer (ID-Customer:{ctx.customer_id}," +
                                                                     f" ID-Rental: {ctx.rental_transaction.id}).\n"))

                                                 
        elif arg_command == "return_car":
            if not arg_customer or len(arg_customer) != 1:
                log_and_show(context, "\nERROR: Invalid customer ID provided! Usage: cmd=return_car customer=\"ID\"")
                sys.exit(-1)
            context.customer_id = arg_customer[0]
            
            context = CarRentalCommands.return_car(context, 
                                                    lambda ctx: show_closed_rentals(ctx),
                                                    lambda ctx: show_message(ctx, f"Customer with ID:{context.customer_id} has no rented cars!")
                                                    )
        
        else:
            log_and_show(context, f"\nUnknown command: \"{arg_command}\"!\n")
            show_message(context, "Use samples:")
            show_message(context, "car_rental cmd=register_customer customer=\"C003,Mary Jung,777 - 1234,911 Main St,mjung@example.com\"")
            show_message(context, "car_rental cmd=delete_customer customer=\"C003\"")
            show_message(context, "car_rental cmd=add_car car=\"CAR911,Porsche,Macant,2023,190.0\"")
            show_message(context, "car_rental cmd=add_car car=\"CAR11,Audi,A1,2021,90.0\"")
            show_message(context, "car_rental cmd=delete_car car=\"CAR11\"")
            show_message(context, "car_rental cmd=rent_car car=\"CAR911\" customer=\"C003\" from=\"2021-10-01\" to=\"2021-10-03\"")
            show_message(context, "car_rental cmd=return_car customer=\"C003\"")
            sys.exit(-1)

        CarRentalCommands.save_data(context)
        sys.exit(0)
        
    except Exception as ex:
        log_and_show(context, f"\nERROR: {ex} \n\nSTACK-TRACE: {ex.__traceback__}")
        # don'save date if exception is occured 
        # CarRentalCommands.save_data(context)
        sys.exit(-1)

def show_closed_rentals(context: CarRentalContext):
    show_message(context, f"All cars have been returned by Customer with ID:{context.customer_id}!\n------------------\n")
    for tx in context.closed_transactions:
        closed_transaction : Transaction = tx
        show_message(context, \
                     f"Closed Rental-ID: {closed_transaction.id}, " + \
                     f"Customer-ID: {closed_transaction.customer.id}," + \
                     f" Car-ID: {closed_transaction.car.id}")   


def customer_empty(context: CarRentalContext): 
    show_message(context, "No customer found!")       

if __name__ == '__main__':
    main(sys.argv[1:])



