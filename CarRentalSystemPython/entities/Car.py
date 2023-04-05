from DataType import DataType
import json

class Car:
    def __init__(self, id: str, make: str, model: str, year: int, daily_price: float, rented: bool):
        self.Id = id
        self.Make = make
        self.Model = model
        self.Year = year
        self.DailyPrice = daily_price
        self.Rented = rented

    def Rent(self) -> None:
        self.Rented = True

    def Return(self) -> None:
        self.Rented = False

    def Serialize(self, data_type: DataType) -> str:
        if data_type == DataType.CSV:
            return f"{self.Id},{self.Make},{self.Model},{self.Year},{self.DailyPrice},{self.Rented}"
        elif data_type == DataType.JSON:
            return json.dumps(self.__dict__)
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    @staticmethod
    def Deserialize(data: str, data_type: DataType) -> 'Car':
        if data_type == DataType.CSV:
            return Car.CreateFromCsv(data)
        elif data_type == DataType.JSON:
            return Car(**json.loads(data))
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    @staticmethod
    def CreateFromCsv(csv: str) -> 'Car':
        values = csv.split(',')
        if len(values) != 6:
            raise ValueError(f"Invalid CSV data length - expected 6, got {len(values)}. CSV-Data: {csv}.")
        return Car(values[0], values[1], values[2], int(values[3]), float(values[4]), bool(values[5]))

    def DeserializeHandler(self, data: str, data_type: DataType) -> 'Car':
        return Car.Deserialize(data, data_type)
