import sys
import argparse
from typing import List
from controller.CarRentalSystem import CarRentalSystem
from controller.CarRentalContext import CarRentalContext
from controller.CarRentalCommands import CarRentalCommands


def show_message(context: CarRentalContext, message: str) -> None:
    context.CarRentalSystem.show_message(message)


def log_and_show(context: CarRentalContext, message: str) -> None:
    context.CarRentalSystem.log_and_show_message(message)
    
def main(args) -> int:
    # init
    arg_command = ""
    arg_customer = []
    arg_car = []

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

    # Initialize car rental system 
    car_rental_system = CarRentalSystem()
    # create rental context
    context : CarRentalContext = CarRentalContext()
    context.car_rental_system = car_rental_system
    context.command = arg_command
    context.car_id = arg_car[0] if arg_car else None
    context.customer_id = arg_customer[0] if arg_customer else None

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
            context.car_id = arg_car[0]
            context.customer_id = arg_customer[0]
            context = CarRentalCommands.rent_car(context)
        elif arg_command == "return_car":
            if not arg_customer or len(arg_customer) != 1:
                log_and_show(context, "\nERROR: Invalid customer ID provided! Usage: cmd=return_car customer=\"ID\"")
                sys.exit(-1)
            context.customer_id = arg_customer[0]
            context = CarRentalCommands.return_car(context)
        else:
            log_and_show(context, f"\nUnknown command: \"{arg_command}\"!\n")
            show_message(context, "Use samples:")
            show_message(context, "car_rental cmd=register_customer customer=\"C003,Mary Jung,777 - 1234,911 Main St,mjung@example.com\"")
            show_message(context, "car_rental cmd=delete_customer customer=\"C003\"")
            show_message(context, "car_rental cmd=add_car car=\"CAR911,Porsche,Macant,2023,190.0\"")
            show_message(context, "car_rental cmd=add_car car=\"CAR11,Audi,A1,2021,90.0\"")
            show_message(context, "car_rental cmd=delete_car car=\"CAR11\"")
            show_message(context, "car_rental cmd=rent_car car=\"CAR911\" customer=\"C003\"")
            sys.exit(-1)

        CarRentalCommands.SaveData(context)
        sys.exit(0)
        
    except Exception as ex:
        log_and_show(context, f"\nERROR: {ex} \n\nSTACK-TRACE: {ex.__traceback__}")
        CarRentalCommands.SaveData(context)
        sys.exit(-1)

if __name__ == '__main__':
    main(sys.argv[1:])



