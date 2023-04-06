from abc import ABC, abstractmethod

class IMessageHandler(ABC):
    @abstractmethod
    # This method is called when a message is logged
    def log(self, message):
        pass

    @abstractmethod
    # This method is called when a message is shown
    def show_message(self, message):
        pass
