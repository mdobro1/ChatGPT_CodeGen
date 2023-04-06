from abc import ABC, abstractmethod
from IEntitiesList import IEntitiesList

class ISerializeOwner(ABC):
    @abstractmethod
    def assign_owner(self, entities_list: IEntitiesList):
        pass
