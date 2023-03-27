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
            List<Customer> loadedCustomers = (List<Customer>)dataManager.ReadData<Customer>(EntityType.CUSTOMER, DataType.CSV);
            if (loadedCustomers != null)
            {
                customers.Clear();
                customers.AddRange(loadedCustomers);
            }

            List<Car> loadedCars = (List<Car>)dataManager.ReadData<Car>(EntityType.CAR, DataType.CSV);
            if (loadedCars != null)
            {
                availableCars.Clear();
                rentedCars.Clear();
                foreach (Car car in loadedCars)
                {
                    if (car.Rented)
                    {
                        rentedCars.Add(car);
                    }
                    else
                    {
                        availableCars.Add(car);
                    }
                }
            }

            List<Transaction> loadedTransactions = (List<Transaction>)dataManager.ReadData<Transaction>(EntityType.TRANSACTION, DataType.CSV);
            if (loadedTransactions != null)
            {
                transactions.Clear();
                transactions.AddRange(loadedTransactions);
            }
        }

        public void SaveData()
        {
            dataManager.WriteData<Customer>(customers, EntityType.CUSTOMER, DataType.CSV);
            dataManager.WriteData<Car>(rentedCars, EntityType.CAR, DataType.CSV);
            dataManager.WriteData<Transaction>(transactions, EntityType.TRANSACTION, DataType.CSV);
        }
    }
}
