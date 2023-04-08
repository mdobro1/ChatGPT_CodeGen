from abc import ABC, abstractmethod
from typing import TypeVar

from entities.Car import Car
from entities.Customer import Customer
from entities.EntityType import EntityType
from entities.Transaction import Transaction


Entity = TypeVar('Entity', Car, Customer, Transaction)

#====================================================================================================
# Entities List Interface
#====================================================================================================
class IEntitesFactory(ABC):
    @abstractmethod
    def new_entity(self, entity_type: EntityType) -> Entity:
        pass