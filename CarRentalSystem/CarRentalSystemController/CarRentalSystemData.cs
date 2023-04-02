using System;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public partial class CarRentalSystem
    {
        private const string FileSuffixCurrent = "current";
        private const string FileSuffixArchive = "archive";

        private readonly DataManager dataManager;

        public CarRentalSystem(DataManager dataManager) : this()
        {
            this.dataManager = dataManager;
        }

        public void LoadData()
        {
            dataManager.AssignOwner(null);
            dataManager.ReadData(customers, EntityType.CUSTOMER, DataType.CSV, string.Empty);
            dataManager.ReadData(availableCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.AVALIABLE));
            dataManager.ReadData(rentedCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.RENTED));
            
            dataManager.AssignOwner(this);
            dataManager.ReadDataExtended(currentTransactions, EntityType.TRANSACTION, DataType.CSV, FileSuffixCurrent, this);
            dataManager.ReadDataExtended(archiveTransactions, EntityType.TRANSACTION, DataType.CSV, FileSuffixArchive, this);
        }

        public void SaveData()
        {
            dataManager.WriteData<Customer>(customers, EntityType.CUSTOMER, DataType.CSV, string.Empty);
            dataManager.WriteData<Car>(availableCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.AVALIABLE));
            dataManager.WriteData<Car>(rentedCars, EntityType.CAR, DataType.CSV, Convert.ToString(RentedType.RENTED));
            dataManager.WriteData<Transaction>(currentTransactions, EntityType.TRANSACTION, DataType.CSV, FileSuffixCurrent);
            dataManager.WriteData<Transaction>(archiveTransactions, EntityType.TRANSACTION, DataType.CSV, FileSuffixArchive);
        }
    }
}
