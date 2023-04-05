from abc import ABC, abstractmethod

class IMessageHandler(ABC):
    @abstractmethod
    def log(self, message):
        pass

    @abstractmethod
    def show_message(self, message):
        pass
