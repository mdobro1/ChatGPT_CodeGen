from abc import ABC, abstractmethod
from typing import Type

class IErrorHandler(ABC):
    @abstractmethod
    def HandleError(self, e: Type[Exception]):
        pass
