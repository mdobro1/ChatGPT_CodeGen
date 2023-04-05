from Car import Car
from DataType import DataType
import json

class Customer:
    def __init__(self, id: str, name: str, phone_number: str, address: str, email: str):
        self.Id = id
        self.Name = name
        self.PhoneNumber = phone_number
        self.Address = address
        self.Email = email
        self.CarRentalMaxLimit = 1
        self.RentedCarsPool = []

    @property
    def RentedCarsCopy(self) -> list:
        return self.RentedCarsPool.copy()

    def RentCar(self, car: Car) -> None:
        if car is None:
            raise ValueError("car parameter cannot be None.")
        if car in self.RentedCarsPool:
            raise ValueError(f"Already rented car with ID: {car.Id}!")
        if len(self.RentedCarsPool) >= self.CarRentalMaxLimit:
            raise ValueError(f"Customer ({self.Id}) is not allowed to rent more than {self.CarRentalMaxLimit} car(s)!")
        
        car.Rent()
        self.RentedCarsPool.append(car)

    def ReturnCar(self, car: Car) -> None:
        if car in self.RentedCarsPool:
            self.RentedCarsPool.remove(car)
        car.Return()

    def Serialize(self, data_type: DataType) -> str:
        if data_type == DataType.CSV:
            return f"{self.Id},{self.Name},{self.PhoneNumber},{self.Address},{self.Email}"
        elif data_type == DataType.JSON:
            return json.dumps(self.__dict__)
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    @staticmethod
    def Deserialize(data: str, data_type: DataType) -> 'Customer':
        if data_type == DataType.CSV:
            return Customer.CreateFromCsv(data)
        elif data_type == DataType.JSON:
            return Customer(**json.loads(data))
        else:
            raise ValueError(f"Unknown data type {data_type}.")

    @staticmethod
    def CreateFromCsv(csv: str) -> 'Customer':
        values = csv.split(',')
        if len(values) != 5:
            raise ValueError(f"Invalid CSV data: {csv}")
        return Customer(values[0], values[1], values[2], values[3], values[4])

    def DeserializeHandler(self, data: str, data_type: DataType) -> 'Customer':
        return Customer.Deserialize(data, data_type)

    def RentedCarsPoolNew(self, rented_cars: list) -> None:
        self.RentedCarsPool.clear()
        self.RentedCarsPoolExtend(rented_cars)

    def RentedCarsPoolExtend(self, rented_cars: list) -> None:
        if rented_cars is None:
            raise ValueError("rented_cars parameter cannot be None.")
        self.RentedCarsPool.extend(rented_cars)
