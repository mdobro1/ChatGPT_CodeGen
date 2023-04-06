from abc import ABC, abstractmethod
from enum import Enum
from DataType import DataType
from typing import TypeVar

T = TypeVar('T')    # Declare type variable

class ISerializedEntity(ABC):
    @abstractmethod
    def serialize(self, data_type: DataType) -> str:
        pass
    
    @abstractmethod
    def deserialize_handler(self, data: str, data_type: DataType) -> T:
        pass
