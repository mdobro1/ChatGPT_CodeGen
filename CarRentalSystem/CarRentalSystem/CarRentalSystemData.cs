using System;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public partial class CarRentalSystem
    {
        private readonly DataManager dataManager;

        public CarRentalSystem(DataManager dataManager) : this()
        {
            this.dataManager = dataManager;
        }

        public void LoadData()
        {
            dataManager.ReadData<Customer>(customers, EntityType.CUSTOMER, DataType.CSV, string.Empty);
            dataManager.ReadData<Car>(availableCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.AVALIABLE));
            dataManager.ReadData<Car>(rentedCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.RENTED));
            dataManager.ReadData<Transaction>(transactions, EntityType.TRANSACTION, DataType.CSV, string.Empty);
        }

        public void SaveData()
        {
            dataManager.WriteData<Customer>(customers, EntityType.CUSTOMER, DataType.CSV, string.Empty);
            dataManager.WriteData<Car>(availableCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.AVALIABLE));
            dataManager.WriteData<Car>(rentedCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.RENTED));
            dataManager.WriteData<Transaction>(transactions, EntityType.TRANSACTION, DataType.CSV, string.Empty);
        }
    }
}
