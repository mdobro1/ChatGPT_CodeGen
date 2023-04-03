using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public partial class CarRentalSystem : IEntitiesList
    {
        private readonly List<Customer> customers;
        private readonly List<Car> availableCars;
        private readonly List<Car> rentedCars;
        private readonly List<Transaction> currentTransactions;
        private readonly List<Transaction> archiveTransactions;

        private readonly ErrorHandler errorHandler;
        private readonly MessageHandler messageHandler;

        public CarRentalSystem()
        {
            customers = new List<Customer>();
            availableCars = new List<Car>();
            rentedCars = new List<Car>();
            currentTransactions = new List<Transaction>();
            archiveTransactions = new List<Transaction>();

            errorHandler = new ErrorHandler(true);
            dataManager = new DataManager(errorHandler, new MessageHandler(errorHandler));
            messageHandler = new MessageHandler(errorHandler);

            if (File.Exists(SecurityManager.KeyFile))
                securityManager = new SecurityManager(File.ReadAllBytes(SecurityManager.KeyFile));
            else
                securityManager = new SecurityManager();

            authenticationManager = new AuthenticationManager(securityManager);
        }

        public CarRentalSystem(DataManager dataManager, SecurityManager securityManager) : this()
        {
            this.dataManager = dataManager;
            this.securityManager = securityManager;
            this.authenticationManager = new AuthenticationManager(securityManager);
        }

        public Customer RegisterCustomer(string id, string name, string phoneNumber, string address, string email)
        {
            var seekCustomer = LookupCustomer(id);

            if (seekCustomer == null)
            {
                Customer customer = new Customer(id, name, phoneNumber, address, email);
                customers.Add(customer);
                return customer;
            }
            else
            {
                return seekCustomer;
            }
        }

        public bool DeleteCar(string id)
        {
            var seekCar = LookupAvaliableCar(id);

            if (seekCar == null)
            {
                seekCar = LookupRentedCar(id);

                if (seekCar == null)
                    errorHandler.HandleError(new InvalidOperationException($"The specified car (ID={id}) does not exist in the system!"));
                else
                    errorHandler.HandleError(new InvalidOperationException($"The specified car (ID={id}) is rented and can't be deleted!"));

                return false;
            }
            else
            {
                this.availableCars.Remove(seekCar);
                return true;
            }
        }

        public bool DeleteCustomer(string id)
        {
            var seekCustomer = LookupCustomer(id);

            if (seekCustomer == null)
            {
                return false;
            }
            else
            {
                this.customers.Remove(seekCustomer);
                return true;
            }
        }

        public Car AddCar(string idCar, string make, string model, int year, double dailyPrice)
        {
            var seekCar = LookupCar(idCar);

            if (seekCar == null)
            {
                Car newCar = new Car(idCar, make, model, year, dailyPrice, false);
                availableCars.Add(newCar);
                return newCar;
            }
            else
                return seekCar;
        }

        public Transaction RentCar(string customerId, string carId, DateTime rentalDate, DateTime returnDate)
        {
            var car = LookupCar(carId);
            if (!availableCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException($"The specified car (ID={carId}) is not available for rental."));
            }

            var customer = LookupCustomer(customerId);

            if (customer != null)
            {
                return Transaction.OpenTransaction(this,customer.Id, car.Id, rentalDate, returnDate);
            }
            else
            {
                messageHandler.LogPlusMessage($"The specified customer with ID: {customerId} has not been found!");
                return new Transaction();
            }
        }

        public Transaction ReturnCar(Customer customer, Car car)
        {
            if (!rentedCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified car has not been rented by the specified customer."));
            }

            Transaction transaction = currentTransactions.Find(t => !t.IsClosed && t.Customer.Id == customer.Id && t.Car.Id == car.Id);
            if (transaction == null)
            {
                errorHandler.HandleError(new InvalidOperationException("The specified transaction could not be found."));
            }

            transaction.CloseTransaction(this);

            return transaction;
        }

        public Transaction ReturnCar(Customer customer)
        {
            if (customer != null)
            {
                var seekCarId = (from item in ListCustomerTransactions(customer)
                                 where item.Customer != null && item.Customer.Id == customer.Id
                                 select item.Car.Id).FirstOrDefault();

                var seekCar = (from item in ListRentedCars()
                               where item.Id == seekCarId
                               select item).FirstOrDefault();

                if (seekCar != null)
                    return ReturnCar(customer, seekCar);
                else
                {
                    LogAndShowMessage($"Customer with ID:{customer.Id} has no rented cars!");
                }
            }
            else
                LogAndShowMessage("Customer has not been found!");

            return new Transaction();
        }

        public List<Car> ListAvailableCars()
        {
            return new List<Car>(availableCars);
        }
        public List<Customer> ListRegisteredCustomers()
        {
            return new List<Customer>(customers);
        }

        public List<Car> ListRentedCars()
        {
            return new List<Car>(rentedCars);
        }

        public List<Transaction> ListCustomerTransactions(Customer customer)
        {
            return currentTransactions.FindAll(t => t.Customer.Id == customer.Id);
        }

        public Customer LookupCustomer(string customerId)
        {
            // validate registered customers list
            var customers = GetRegisteredCustomers();
            if (customers == null) throw new ArgumentNullException("No registered customers!");
            // lookup customer           
            var customer = customers.Find(c => string.Equals(c.Id, customerId, StringComparison.InvariantCultureIgnoreCase));
            return customer;
        }

        public Car LookupCar(string carId)
        {
            Car car = LookupAvaliableCar(carId);

            if (car == null)
            {
                car = LookupRentedCar(carId);
            }

            return car;
        }

        public Car LookupRentedCar(string carId)
        {
            Car car;
            var rentedCars = GetRentedCars();
            if (rentedCars == null) throw new ArgumentNullException("rentedCars");

            car = rentedCars.Find(c => string.Equals(c.Id, carId, StringComparison.InvariantCultureIgnoreCase));
            return car;
        }

        public Car LookupAvaliableCar(string carId)
        {
            // validate avaliable cars list
            var avaliableCars = GetAvaliableCars();
            if (avaliableCars == null) throw new ArgumentNullException("avaliableCars");

            // lookup car
            var car = availableCars.Find(c => string.Equals(c.Id, carId, StringComparison.InvariantCultureIgnoreCase));
            return car;
        }

        public List<Car> GetAvaliableCars()
        {
            return availableCars;
        }

        public Car GetCar(string carId)
        {
            return availableCars.Find(
                c => string.Equals(c.Id, carId, StringComparison.InvariantCultureIgnoreCase));
        }

        public Car GetFirstAvailiableCar(CarRentalSystem carRentalSystem)
        {
            return carRentalSystem.ListAvailableCars().FirstOrDefault();
        }

        public List<Customer> GetRegisteredCustomers()
        {
            return customers;
        }

        public List<Car> GetRentedCars()
        {
            return rentedCars;

        }

        public List<Car> GetRentedCars(Customer customer)
        {
            var result = new List<Car>();

            var customerCurrentTransactions = 
                currentTransactions.FindAll(tx => tx.Customer.Id == customer.Id);

            foreach (var customerTransaction in customerCurrentTransactions)
            {
                result.Add(customerTransaction.Car);
            }
    
            return result;
        }

        public void ArchiveTransaction(Transaction transaction)
        {
            archiveTransactions.Add(transaction);
            currentTransactions.Remove(transaction);
        }

        public void RentCar(Car car)
        {
            availableCars.Remove(car);
            rentedCars.Add(car);
        }

        public void ReturnCar(Car car)
        {
            rentedCars.Remove(car);
            availableCars.Add(car);
        }

        public void NewTransaction(Transaction transaction)
        {
            currentTransactions.Add(transaction);
        }
    }
}
