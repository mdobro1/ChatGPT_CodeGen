import os
from typing import List, TypeVar, Generic

from aspects import ErrorHandler
from aspects import MessageHandler
from entities.EntityType import EntityType
from entities.DataType import DataType
from entities.ISerializedEntity import ISerializedEntity
from entities.Transaction import ISerializedExtendedEntity
from entities.Transaction import IEntitiesList
from entities.Transaction import ISerializeOwner

T = TypeVar('T', bound=ISerializedEntity)
E = TypeVar('E', bound=ISerializedExtendedEntity)

# This class is used to read and write data from and to files.
class DataManager(Generic[T, E], ISerializeOwner):
    DATA_FOLDER_PATH = "data"

    def __init__(self, error_handler: ErrorHandler, message_handler: MessageHandler):
        self.error_handler = error_handler
        self.message_handler = message_handler
        self.owner: IEntitiesList = None

    def read_data(self, entity_type: EntityType, data_type: DataType, file_suffix: str) -> List[T]:
        file_path = self.get_file_path(entity_type, data_type, file_suffix)
        data_list = []

        if os.path.exists(file_path):
            try:
                with open(file_path, "r") as f:
                    for line in f:
                        data = self.deserialize(line.strip(), data_type)

                        if data is not None:
                            data_list.append(data)
            except Exception as ex:
                self.error_handler.handle_error(ex)

        return data_list

    def read_data_extended(self, entity_type: EntityType, data_type: DataType, file_suffix: str) -> List[E]:
        file_path = self.get_file_path(entity_type, data_type, file_suffix)
        data_list = []

        if os.path.exists(file_path):
            try:
                with open(file_path, "r") as f:
                    for line in f:
                        data = None

                        if self.owner is None:
                            data = self.deserialize(line.strip(), data_type)
                        else:
                            data = self.deserialize_extended(line.strip(), data_type, self.owner)

                        if data is not None:
                            data_list.append(data)
            except Exception as ex:
                self.error_handler.handle_error(ex)

        return data_list

    def write_data(self, data_list: List[T], entity_type: EntityType, data_type: DataType, file_suffix: str) -> bool:
        file_path = self.get_file_path(entity_type, data_type, file_suffix)
        directory_path = os.path.dirname(file_path)

        if not os.path.exists(directory_path):
            os.makedirs(directory_path)

        try:
            with open(file_path, "w") as f:
                for data in data_list:
                    line = self.serialize(data, data_type)
                    f.write(f"{line}\n")
                self.message_handler.log_plus_message(f"Write Data - Rows:{len(data_list)}, Entity:{entity_type}, Data:{data_type} ({file_suffix}).")
                return True
        except Exception as ex:
            self.error_handler.handle_error(ex)
            return False

    def read_data(self, target_list: List[T], entity_type: EntityType, data_type: DataType, file_suffix: str):
        if target_list is None:
            self.error_handler.handle_error(ValueError("target_list"))

        list_items = self.read_data(entity_type, data_type, file_suffix)

        if list_items:
            target_list.clear()
            target_list.extend(list_items)
            self.message_handler.log_plus_message(f"Read Data - Rows:{len(target_list)}, Entity:{entity_type}, Data:{data_type} ({file_suffix}).")
        else:
            self.message_handler.log_plus_message(f"No Data - Entity:{entity_type}, Data:{data_type}.")

    def get_file_path(self, entity_type, data_type, file_suffix):
        suffix = f"_{file_suffix}" if file_suffix else ""
        file_name = f"{entity_type.name.lower()}{suffix}.{data_type.name.lower()}"
        return os.path.join(self.DataFolderPath, file_name)

    def read_lines_from_file(self, file_path):
        lines = []
        try:
            with open(file_path, "r") as f:
                lines = f.readlines()
        except Exception as ex:
            self.error_handler.HandleError(ex)
        return lines

    def write_lines_to_file(self, file_path, lines):
        try:
            with open(file_path, "w") as f:
                f.writelines(lines)
        except Exception as ex:
            self.error_handler.HandleError(ex)

    def assign_owner(self, entities_list):
        self.owner = entities_list        

    def serialize(self, data: T, dataType: DataType) -> str:
            if data is None:
                raise ValueError("data cannot be None.")

            if dataType == DataType.CSV or dataType == DataType.JSON:
                return data.Serialize(dataType)
            else:
                raise ValueError(f"Invalid data type: {dataType}")

    def deserialize(self, data: str, dataType: DataType) -> T:
        if data is None:
            raise ValueError("data cannot be None.")

        if dataType == DataType.CSV or dataType == DataType.JSON:
            return T().DeserializeHandler(data, dataType)
        else:
            raise ValueError(f"Invalid data type: {dataType}")

    
    def deserialize_extended(self, data: str, dataType: DataType, entitiesList: IEntitiesList) -> E:
        if data is None:
            raise ValueError("data cannot be None.")

        if dataType == DataType.CSV or dataType == DataType.JSON:
            return E().deserialize_handler(data, dataType, entitiesList)
        else:
            raise ValueError(f"Invalid data type: {dataType}")        