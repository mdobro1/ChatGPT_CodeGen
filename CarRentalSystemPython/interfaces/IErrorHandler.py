from abc import ABC, abstractmethod
from typing import Type

class IErrorHandler(ABC):
    @abstractmethod
    # This method is called when an error occurs
    def handle_error(self, e: Type[Exception]):
        pass
