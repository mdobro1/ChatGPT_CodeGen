from abc import ABC, abstractmethod
from enum import Enum
from entities.DataType import DataType
from typing import TypeVar, Generic

T = TypeVar('T')    # Declare type variable

class ISerializedEntity(ABC, Generic[T]):
    @abstractmethod
    def serialize(self, data_type: DataType) -> str:
        pass
    
    @abstractmethod
    def deserialize_handler(self, data: str, data_type: DataType) -> T:
        pass
