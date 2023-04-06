from __future__ import annotations
from typing import List, Type
import json

from ISerializedEntity import ISerializedEntity, DataType
from Car import Car
from Customer import Customer

class Customer(ISerializedEntity[Customer]):
    CAR_RENTAL_MAX_LIMIT = 1

    def __init__(self, id: str, name: str, phone_number: str, address: str, email: str):
        self._id = id
        self._name = name
        self._phone_number = phone_number
        self._address = address
        self._email = email
        self._rented_cars_pool = []

    @property
    def id(self) -> str:
        return self._id

    @property
    def name(self) -> str:
        return self._name

    @property
    def phone_number(self) -> str:
        return self._phone_number

    @property
    def address(self) -> str:
        return self._address

    @property
    def email(self) -> str:
        return self._email

    @property
    def rented_cars_copy(self) -> List[Car]:
        return self._rented_cars_pool.copy()

    def rent_car(self, car: Car) -> None:
        if car is None:
            raise ValueError("car cannot be None")
        if car in self._rented_cars_pool:
            raise ValueError(f"Already rented car with ID: {car.id}!")
        if len(self._rented_cars_pool) >= Customer.CAR_RENTAL_MAX_LIMIT:
            raise ValueError(f"Customer ({self._id}) is not allowed to rent more than {Customer.CAR_RENTAL_MAX_LIMIT} car(s)!")

        car.rent()
        self._rented_cars_pool.append(car)

    def return_car(self, car: Car) -> None:
        if car in self._rented_cars_pool:
            self._rented_cars_pool.remove(car)
        car.return_car()

    def serialize(self, data_type: DataType) -> str:
        if data_type == DataType.CSV:
            return f"{self._id},{self._name},{self._phone_number},{self._address},{self._email}"
        elif data_type == DataType.JSON:
            return json.dumps(self.__dict__)
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    @staticmethod
    def deserialize(data: str, data_type: DataType) -> Customer:
        if data_type == DataType.CSV:
            return Customer.create_from_csv(data)
        elif data_type == DataType.JSON:
            return Customer(**json.loads(data))
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    def deserialize_handler(self, data: str, data_type: DataType) -> Customer:
        return Customer.deserialize(data, data_type)

    @staticmethod
    def create_from_csv(csv: str) -> Customer:
        values = csv.split(",")
        if len(values) != 5:
            raise ValueError(f"Invalid CSV data: {csv}")
        return Customer(values[0], values[1], values[2], values[3], values[4])

    def rented_cars_pool_new(self, rented_cars: List[Car]) -> None:
        self._rented_cars_pool.clear()
        self._rented_cars_pool_extend(rented_cars)

    def rented_cars_pool_extend(self, rented_cars: List[Car]) -> None:
        if rented_cars is None:
            raise ValueError("rented_cars cannot be None")

