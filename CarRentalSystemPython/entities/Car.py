from __future__ import annotations
from typing import Type
import json

from Car import Car
from DataType import DataType  
from ISerializedEntity import ISerializedEntity 

class Car(ISerializedEntity[Car]):
    def __init__(self, id: str, make: str, model: str, year: int, daily_price: float, rented: bool):
        self._id = id
        self._make = make
        self._model = model
        self._year = year
        self._daily_price = daily_price
        self._rented = rented

    @property
    def id(self) -> str:
        return self._id

    @property
    def make(self) -> str:
        return self._make

    @property
    def model(self) -> str:
        return self._model

    @property
    def year(self) -> int:
        return self._year

    @property
    def daily_price(self) -> float:
        return self._daily_price

    @property
    def rented(self) -> bool:
        return self._rented

    def rent(self) -> None:
        self._rented = True

    def return_car(self) -> None:
        self._rented = False

    def serialize(self, data_type: DataType) -> str:
        if data_type == DataType.CSV:
            return f"{self._id},{self._make},{self._model},{self._year},{self._daily_price},{self._rented}"
        elif data_type == DataType.JSON:
            return json.dumps(self.__dict__)
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    @staticmethod
    def deserialize(data: str, data_type: DataType) -> Car:
        if data_type == DataType.CSV:
            return Car.create_from_csv(data)
        elif data_type == DataType.JSON:
            return Car(**json.loads(data))
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    def deserialize_handler(self, data: str, data_type: DataType) -> Car:
        return Car.deserialize(data, data_type)

    @staticmethod
    def create_from_csv(csv: str) -> Car:
        csv_len = 6
        values = csv.split(",")
        if len(values) != csv_len:
            raise ValueError(f"Invalid CSV data length - expected {csv_len}, got {len(values)}. CSV-Data: {csv}.")
        return Car(values[0], values[1], values[2], int(values[3]), float(values[4]), bool(values[5]))
