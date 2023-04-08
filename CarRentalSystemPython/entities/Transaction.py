from __future__ import annotations
from typing import Type
import json
from abc import ABC, abstractmethod
from typing import TypeVar
from typing import List
from datetime import datetime, timedelta
from uuid import uuid4
import uuid  # for generating unique ids

from entities.DataType import DataType
from entities.ISerializedEntity import ISerializedEntity
from entities.Customer import Customer
from entities.Car import Car
from entities.DataType import DataType

T = TypeVar('T')

#====================================================================================================
# Serialize Owner Interface
#====================================================================================================
class ISerializeOwner(ABC):
    @abstractmethod
    def assign_owner(self, entities_list: IEntitiesList):
        pass

#====================================================================================================
# Entities List Interface
#====================================================================================================
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


#====================================================================================================
# SerializedExtendedEntity interface
#====================================================================================================
class ISerializedExtendedEntity(ISerializedEntity[T], ABC):
    def deserialize_handler(self, data: str, data_type: DataType, entities_list: IEntitiesList) -> T:
        pass


#===============================================================================
# Transaction class
#===============================================================================
class Transaction(ISerializedExtendedEntity["Transaction"], ISerializeOwner):

    def __init__(self, id: str = str(uuid.uuid4()), customer: Customer = None, car: Car = None, \
                 rental_date: datetime = None, return_date: datetime = None, closed_date: datetime = None, is_closed: bool = False):
        self._id = id
        self._customer = customer
        self._car = car
        self._rental_date = rental_date
        self._return_date = return_date
        self._closed_date = closed_date
        self._is_closed = is_closed
        self._owner = None
        self._total_price = self._calculate_total_price()        

    @property
    def id(self) -> str:
        return self._id

    @property
    def customer(self) -> Customer:
        return self._customer

    @property
    def car(self) -> Car:
        return self._car

    @property
    def rental_date(self) -> datetime:
        return self._rental_date

    @property
    def return_date(self) -> datetime:
        return self._return_date

    @property
    def closed_date(self) -> datetime:
        return self._closed_date

    @property
    def is_closed(self) -> bool:
        return self._is_closed

    @property
    def total_price(self) -> float:
        return self._total_price

    @property
    def owner(self) -> IEntitiesList:
        return self._owner

    def close_transaction(self, entities_list: IEntitiesList) -> None:
        # validate entities source
        if entities_list is None:
            raise ValueError("entitiesList cannot be None")

        # close transaction
        self._customer.return_car(self._car)
        self._is_closed = True
        self._closed_date = datetime.now()
        entities_list.return_car(self._car)
        entities_list.archive_transaction(self)

    def _calculate_total_price(self) -> float:
        rental_period = self._return_date - self._rental_date
        total_days = int(rental_period.total_seconds() / timedelta(days=1).total_seconds())
        if total_days < 1:
            total_days = 1
        return total_days * self._car.daily_price

    def serialize(self, data_type: DataType) -> str:
        if data_type == DataType.CSV:
            return f"{self._id},{self._customer.id},{self._car.id},{self._rental_date.strftime('%m/%d/%Y')}," \
                 + f"{self._return_date.strftime('%m/%d/%Y')},{self._closed_date.strftime('%m/%d/%Y')},{self._is_closed}"
        elif data_type == DataType.JSON:
            return json.dumps(self.__dict__)
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    def deserialize_handler(self, data: str, data_type: DataType, entities_list: IEntitiesList) -> Transaction:
        return Transaction.deserialize(data, data_type, entities_list)

    def deserialize_handler(self, data: str, data_type: DataType) -> Transaction:
        return Transaction.deserialize(data, data_type, self._owner)

    def assign_owner(self, entities_list: IEntitiesList) -> None:
        self._owner = entities_list

    @staticmethod
    def deserialize(data: str, data_type: DataType) -> Transaction:
        if data_type == DataType.CSV:
            return Transaction.create_from_csv(data)
        elif data_type == DataType.JSON:
            return Transaction(**json.loads(data))               
        
    @staticmethod
    def open_transaction(entities_list : IEntitiesList, customer_id : str, car_id : str, rental_date : datetime, return_date: datetime) -> Transaction:
        # validate entities source
        if entities_list is None:
            raise ValueError("entitiesList cannot be None!")
        
        # validation dates
        if rental_date.date() < datetime.now().date():
            raise ValueError("Rental date cannot be in the past!")
        if rental_date.date() > return_date.date():
            raise ValueError("Rental date cannot be bigger than return date!")
        
        # get customer
        customer = entities_list.LookupCustomer(customer_id)
        # validate customer 
        if customer is None:
            raise ValueError(f"Customer with ID: {customer_id} has not been found!")
        
        # get car
        car = entities_list.lookup_car(car_id)
        # validate car
        if car is None:
            raise ValueError(f"Car with ID: {car_id} has not been found!")
        
        # generate Tx-ID
        txID = str(uuid4.uuid4())
        
        # create rental transaction
        newTransaction = Transaction(txID, customer, car, rental_date, return_date, None, False)
        newTransaction.customer.rent_car(car)
        entities_list.rent_car(car)
        entities_list.NewTransaction(newTransaction)
        return newTransaction
        
    @staticmethod
    def create_from_csv(csv: str, entities_list: IEntitiesList)->Transaction:
        values = csv.split(',')
        if len(values) < 7:
            raise ValueError(f"Invalid CSV data: {csv}")
        
        id = values[0]
        
        customer = None
        car = None
        
        customer_id = values[1]
        car_id = values[2]
        
        if entities_list is None:
            customer = Transaction.default_customer(customer_id)
            car = Transaction.default_car(car_id)
        else:
            customer = entities_list.LookupCustomer(customer_id)
            if customer is None:
                customer = Transaction.default_customer(customer_id)
            else:
                customer.RentedCarsPoolNew(entities_list.GetRentedCars(customer))

            car = entities_list.LookupCar(car_id)
            if car is None:
                car = Transaction.default_car(car_id)
        
        rental_date = datetime.strptime(values[3], '%m/%d/%Y')
        return_date = datetime.strptime(values[4], '%m/%d/%Y')
        
        closed_date = None
        if values[5] != "None":
            closed_date = datetime.strptime(values[5], '%m/%d/%Y')
        
        is_closed = bool(values[6])
        
        return Transaction(id, customer, car, rental_date, return_date, closed_date, is_closed)
        
    @staticmethod
    def calculate_total_price(self: str) -> float:
        rental_period = self.return_date - self.rental_date
        total_days = max(rental_period.days, 1)
        return total_days * self.car.daily_price

    @staticmethod
    def default_car(car_id: str)-> Car:
        return Car(car_id, "", "", 0, 0.0, False)

    @staticmethod
    def default_customer(customer_id: str)-> Customer:
        return Customer(customer_id, "", "", "", "")
        
