from abc import ABC, abstractmethod
from typing import List, TypeVar
from datetime import datetime
import os
import uuid  

from aspects.ErrorHandler import ErrorHandler
from aspects.MessageHandler import MessageHandler
from aspects.AuthenticationManager import AuthenticationManager
from aspects.SecurityManager import SecurityManager, User, UserRole
from aspects.DataManager import DataManager
from entities.IEntitesFactory import Entity, IEntitesFactory

from entities.Transaction import IEntitiesList
from entities.EntityType import EntityType
from entities.DataType import DataType
from entities.RentedType import RentedType
from entities.Customer import Customer
from entities.Car import Car
from entities.Transaction import Transaction



class CarRentalSystem(IEntitesFactory):
    #==========================================================
    # Constructors
    #==========================================================
    def __init__(self):
        self.customers = []
        self.available_cars = []
        self.rented_cars = []
        self.current_transactions = []
        self.archive_transactions = []
        self.error_handler = ErrorHandler(True)
        self.data_manager = DataManager(self.error_handler, MessageHandler(self.error_handler))
        self.message_handler = MessageHandler(self.error_handler)
        if os.path.exists(SecurityManager.KEY_FILE):
            self.security_manager = SecurityManager(open(SecurityManager.KEY_FILE, 'rb').read())
        else:
            self.security_manager = SecurityManager()
        self.authentication_manager = AuthenticationManager(self.security_manager)
        
    #==========================================================
    # Main methods
    #==========================================================
    def add_car(self, id_car: str, make: str, model: str, year: int, daily_price: float) -> Car:
        seek_car = self.lookup_car(id_car)

        if seek_car is None:
            new_car = Car(id_car, make, model, year, daily_price, False)
            self.available_cars.append(new_car)
            return new_car
        else:
            return seek_car

    def delete_car(self, id: str) -> bool:
        seek_car = self.lookup_available_car(id)
        seek_car_transactions = self.lookup_car_transactions(id)

        if seek_car is None:
            seek_car = self.lookup_rented_car(id)

            if seek_car is None:
                raise ValueError(f"The specified car (ID={id}) does not exist in the system!")
            else:
                raise ValueError(f"The specified car (ID={id}) is rented and can't be deleted!")

        elif len(seek_car_transactions) > 0:
            raise ValueError(f"The specified car (ID={id}) was rented and can't be deleted!")

        else:
            self.available_cars.remove(seek_car)
            return True

    def delete_customer(self, id: str) -> bool:
        seek_customer_transactions = self.lookup_customer_transactions(id)

        if len(seek_customer_transactions) > 0:
            raise ValueError(f"The specified customer (ID={id}) has rented a car and can't be deleted!")
        else:
            seek_customer = self.lookup_customer(id)
            self.customers.remove(seek_customer)
            return True

    def register_customer(self, id: str, name: str, phone_number: str, address: str, email: str) -> Customer:
        seek_customer = self.lookup_customer(id)

        if seek_customer is None:
            customer = Customer(id, name, phone_number, address, email)
            self.customers.append(customer)
            return customer
        else:
            return seek_customer
        
    def rent_car(self, customer_id: str, car_id: str, rental_date: datetime, return_date: datetime) -> Transaction:
            car = self.lookup_car(car_id)
            if car not in self.available_cars:
                raise ValueError(f"The specified car (ID={car_id}) is not available for rental.")

            customer = self.lookup_customer(customer_id)

            if customer:
                return Transaction.open_transaction(self, customer_id, car_id, rental_date, return_date)
            else:
                print(f"The specified customer with ID: {customer_id} has not been found!")
                return Transaction()

    def list_available_cars(self) -> List[Car]:
        return list(self.available_cars)

    def list_registered_customers(self) -> List[Customer]:
        return list(self.customers)

    def list_rented_cars(self) -> List[Car]:
        return list(self.rented_cars)

    def list_customer_transactions(self, customer: Customer) -> List[Transaction]:
        return [t for t in self.current_transactions if t.customer.id == customer.id]

    def lookup_customer(self, customer_id: str) -> Customer:
        customers = self.get_registered_customers()
        if not customers:
            raise ValueError("No registered customers!")
        return next((c for c in customers if c.id.lower() == customer_id.lower()), None)

    def lookup_customer_transactions(self, customer_id: str) -> List[Transaction]:
        result = []
        result.extend(t for t in self.current_transactions if t.customer.id.lower() == customer_id.lower())
        result.extend(t for t in self.archive_transactions if t.customer.id.lower() == customer_id.lower())
        return result

    def lookup_car_transactions(self, car_id: str) -> List[Transaction]:
        result = []
        result.extend(t for t in self.current_transactions if t.car.id.lower() == car_id.lower())
        result.extend(t for t in self.archive_transactions if t.car.id.lower() == car_id.lower())
        return result        

    def return_all_customer_cars(self, customer: Customer) -> List[Transaction]:
        closed_transactions = []
        cars = self.get_rented_cars_by_customer(customer)
        for car in cars:
            closed_transactions.append(self.return_car(customer, car))
        return closed_transactions


    def return_car(self, customer: Customer, car: Car) -> Transaction:
        if car not in self.rented_cars:
            self.error_handler.handle_error(ValueError("The specified car has not been rented by the specified customer."))
        transaction = next((t for t in self.current_transactions if not t.is_closed and t.customer.id == customer.id and t.car.id == car.id), None)

        if transaction is None:
            self.error_handler.handle_error(ValueError("The specified transaction could not be found."))
        else:
            transaction.close_transaction(self)
            return transaction
    
    def list_available_cars(self) -> List[Car]:
        return list(self.available_cars)

    def list_registered_customers(self) -> List[Customer]:
        return list(self.customers)

    def list_rented_cars(self) -> List[Car]:
        return list(self.rented_cars)

    def list_customer_transactions(self, customer: Customer) -> List[Transaction]:
        return [t for t in self.current_transactions if t.customer.id == customer.id]

    def lookup_customer(self, customer_id: str) -> Customer:
        return next((c for c in self.customers if c.id.lower() == customer_id.lower()), None)

    def lookup_customer_transactions(self, customer_id: str) -> List[Transaction]:
        result = []
        for tx in self.current_transactions:
            if tx.customer.id.lower() == customer_id.lower():
                result.append(tx)
        for tx in self.archive_transactions:
            if tx.customer.id.lower() == customer_id.lower():
                result.append(tx)
        return result

    def lookup_car_transactions(self, car_id: str) -> List[Transaction]:
        result = []
        for tx in self.current_transactions:
            if tx.car.id.lower() == car_id.lower():
                result.append(tx)
        for tx in self.archive_transactions:
            if tx.car.id.lower() == car_id.lower():
                result.append(tx)
        return result

    def lookup_car(self, car_id: str) -> Car:
        car = self.lookup_available_car(car_id)
        if car is None:
            car = self.lookup_rented_car(car_id)
        return car
    
    def lookup_rented_car(self, car_id: str) -> Car:
        rented_cars = self.get_rented_cars()
        return next((car for car in rented_cars if car.id == car_id), None)

    def lookup_available_car(self, car_id: str) -> Car:
        available_cars = self.get_available_cars()
        return next((car for car in available_cars if car.id == car_id), None)

    def get_available_cars(self) -> List[Car]:
        return self.available_cars

    def get_car(self, car_id: str) -> Car:
        available_car = self.lookup_available_car(car_id)
        if available_car is not None:
            return available_car
        rented_car = self.lookup_rented_car(car_id)
        if rented_car is not None:
            return rented_car
        return None

    def get_first_available_car(self) -> Car:
        available_cars = self.get_available_cars()
        if len(available_cars) > 0:
            return available_cars[0]
        return None

    def get_registered_customers(self) -> List[Customer]:
        return self.customers

    def get_rented_cars(self):
        return self.rented_cars
    
    def get_rented_cars_by_customer(self, customer):
        return self.get_rented_cars_customer(customer)

    def get_rented_cars_customer(self, customer):
        result = []
        for tx in self.current_transactions:
            if tx.customer == customer:
                result.append(tx.car)
        return result

    def archive_transaction(self, transaction):
        self.archive_transactions.append(transaction)
        self.current_transactions.remove(transaction)

    def register_car_as_rented(self, car):
        self.available_cars.remove(car)
        self.rented_cars.append(car)

    def unregister_car_as_rented(self, car: Car):
        self.rented_cars.remove(car)
        self.available_cars.append(car)

    def new_transaction(self, transaction):
        self.current_transactions.append(transaction)

    #===========================================================================
    # Authentication
    #===========================================================================
    def login(self, username: str, password: str, user_role: UserRole) -> bool:
        success = self.authentication_manager.login(username, password)
        if success:
            self.current_user = User(str(uuid.uuid4()), username, user_role)
            self.log_and_show_message("Login successful.")
        else:
            self.log_and_show_message("Login failed.")
        return success

    def logout(self) -> None:
        self.authentication_manager.logout()
        self.current_user = None
        self.log_and_show_message("Logout successful.")

    def is_logged_in(self) -> bool:
        return self.current_user is not None

    def get_current_user(self) -> User:
        return self.current_user

    #===========================================================================
    # Data persistence
    #===========================================================================
    FILE_SUFFIX_CURRENT = "current"
    FILE_SUFFIX_ARCHIVE = "archive"


    def load_data(self):
        self.data_manager.assign_owner(self)
        
        self.data_manager.read_data(self.customers, EntityType.CUSTOMER, DataType.CSV, "")
        self.data_manager.read_data(self.available_cars, EntityType.CAR, DataType.CSV, str(RentedType.AVAILABLE).split(".")[1].lower())
        self.data_manager.read_data(self.rented_cars, EntityType.CAR, DataType.CSV, str(RentedType.RENTED).split(".")[1].lower())
        
        self.data_manager.read_data_extended(self.current_transactions, EntityType.TRANSACTION, DataType.CSV, self.FILE_SUFFIX_CURRENT, self)
        self.assign_customer_tranasctions(self.current_transactions)

        self.data_manager.read_data_extended(self.archive_transactions, EntityType.TRANSACTION, DataType.CSV, self.FILE_SUFFIX_ARCHIVE, self)

    def assign_customer_tranasctions(self, transactions : List[Transaction]):
        for transaction in transactions:   
            self.assign_customer_transaction(transaction)
    
    def assign_customer_transaction(self, transaction):
        customer = transaction.customer

        if customer is not None:
            rented_cars = self.get_rented_cars_by_customer(customer)
            customer.rented_cars_pool_new(rented_cars)
        else:
            self.log_and_show_message("Customer not found for transaction: " + transaction.id)

    def save_data(self):
        self.data_manager.write_data(self.customers, EntityType.CUSTOMER, DataType.CSV, "")
        self.data_manager.write_data(self.available_cars, EntityType.CAR, DataType.CSV, str(RentedType.AVAILABLE).split(".")[1].lower())
        self.data_manager.write_data(self.rented_cars, EntityType.CAR, DataType.CSV, str(RentedType.RENTED).split(".")[1].lower())
        self.data_manager.write_data(self.current_transactions, EntityType.TRANSACTION, DataType.CSV, self.FILE_SUFFIX_CURRENT)
        self.data_manager.write_data(self.archive_transactions, EntityType.TRANSACTION, DataType.CSV, self.FILE_SUFFIX_ARCHIVE)

    def new_entity(self, entity_type: EntityType) -> Entity:
        if entity_type == EntityType.CUSTOMER: 
            return Customer()
        elif entity_type == EntityType.CAR: 
            return Car()  
        elif entity_type == EntityType.TRANSACTION: 
            return Transaction()
        else:
            return None

    #===========================================================================
    # Logging, messages and errors handling
    #===========================================================================
    def log(self, message: str):
        self.message_handler.log(message)

    def show_message(self, message: str):
        self.message_handler.show_message(message)

    def log_and_show_message(self, message: str):
        self.message_handler.log(message)
        self.message_handler.show_message(message)

    def handle_error(self, ex: Exception):
        self.error_handler.handle_error(ex)

    #===========================================================================
    # Security
    #===========================================================================
    def authorize(self, user: User, permission: str) -> bool:
        return self.security_manager.authorize(user, permission)

    def encrypt_data(self, customers: List[Customer]):
        for customer in customers:
            customer.phone_number = self.security_manager.encrypt(customer.phone_number)
            customer.address = self.security_manager.encrypt(customer.address)
            customer.email = self.security_manager.encrypt(customer.email)

    def decrypt_data(self, customers: List[Customer]):
        for customer in customers:
            customer.phone_number = self.security_manager.decrypt(customer.phone_number)
            customer.address = self.security_manager.decrypt(customer.address)
            customer.email = self.security_manager.decrypt(customer.email)
