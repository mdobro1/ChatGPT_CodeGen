from abc import ABC, abstractmethod
from typing import List
from entities.Car import Car
from entities.Customer import Customer
from entities.Transaction import Transaction

class IEntitiesList(ABC):
    @abstractmethod
    def get_registered_customers(self) -> List[Customer]:
        pass
    
    @abstractmethod
    def lookup_customer(self, customer_id: str) -> Customer:
        pass
    
    @abstractmethod
    def get_available_cars(self) -> List[Car]:
        pass
    
    @abstractmethod
    def get_rented_cars(self) -> List[Car]:
        pass
    
    @abstractmethod
    def get_rented_cars_by_customer(self, customer: Customer) -> List[Car]:
        pass
    
    @abstractmethod
    def lookup_car(self, car_id: str) -> Car:
        pass
    
    @abstractmethod
    def new_transaction(self, transaction: Transaction):
        pass
    
    @abstractmethod
    def archive_transaction(self, transaction: Transaction):
        pass
    
    @abstractmethod
    def rent_car(self, car: Car):
        pass
    
    @abstractmethod
    def return_car(self, car: Car):
        pass
