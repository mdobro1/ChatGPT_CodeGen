from abc import ABC
from typing import TypeVar
from IEntitiesList import IEntitiesList
from ISerializedEntity import ISerializedEntity, DataType

T = TypeVar('T')

class ISerializedExtendedEntity(ISerializedEntity[T], ABC):
    def deserialize_handler(self, data: str, data_type: DataType, entities_list: IEntitiesList) -> T:
        pass
