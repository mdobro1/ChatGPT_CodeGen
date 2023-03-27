using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public partial class CarRentalSystem
    {
        private readonly List<Customer> customers;
        private readonly List<Car> availableCars;
        private readonly List<Car> rentedCars;
        private readonly List<Transaction> transactions;

        private readonly ErrorHandler errorHandler;
        private readonly MessageHandler messageHandler;

        public CarRentalSystem()
        {
            customers = new List<Customer>();
            availableCars = new List<Car>();
            rentedCars = new List<Car>();
            transactions = new List<Transaction>();
            
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
            var seekCustomer =
                (from item in customers
                 where String.Equals(item.Id.Trim(), id.Trim(), StringComparison.OrdinalIgnoreCase)
                 select item).FirstOrDefault();

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

        public void AddCar(string idCar, string make, string model, int year, double dailyPrice)
        {
            var seekCar =
                (from item in availableCars
                 where String.Equals(item.Id.Trim(), idCar.Trim(), StringComparison.OrdinalIgnoreCase)
                 select item).FirstOrDefault();

            if (seekCar == null)
            {
                Car newCar = new Car(idCar, make, model, year, dailyPrice, false);
                availableCars.Add(newCar);
            }
        }

        public void RentCar(Customer customer, Car car, DateTime rentalDate, DateTime returnDate)
        {
            if (!availableCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified car is not available for rental."));
            }

            var seekRentCustomer = transactions.FindAll(item => !item.IsClosed && item.Customer.Id == customer.Id).FirstOrDefault();

            if (seekRentCustomer == null)
            {
                double totalPrice = car.DailyPrice * (returnDate - rentalDate).TotalDays;
                Transaction transaction = new Transaction(Guid.NewGuid().ToString(), customer, car, rentalDate, returnDate, totalPrice);
                transactions.Add(transaction);
                customer.RentCar(car);
                car.Rent();
                availableCars.Remove(car);
                rentedCars.Add(car);
            }
            else
            {
                messageHandler.LogPlusMessage($"The specified customer with ID: {customer.Id} has already rented a car!");
            }

        }

        public void ReturnCar(Customer customer, Car car)
        {
            if (!rentedCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified car has not been rented by the specified customer."));
            }

            /*
            if (!customer.RentedCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified customer has not rented the specified car."));
            }*/

            Transaction transaction = transactions.Find(t => !t.IsClosed && t.Customer.Id == customer.Id && t.Car.Id == car.Id);
            if (transaction == null)
            {
                errorHandler.HandleError(new InvalidOperationException("The specified transaction could not be found."));
            }

            transaction.CloseTransaction();
            car.Return();
            rentedCars.Remove(car);
            availableCars.Add(car);
            customer.ReturnCar(car);
        }

        public void ReturnCar(Customer customer)
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
                    ReturnCar(customer, seekCar);
                else
                    LogAndShowMessage($"Car with ID:{seekCarId} has not been rented!");
            }
            else
                LogAndShowMessage("Customer has not been found!");
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
            return transactions.FindAll(t => t.Customer.Id == customer.Id);
        }
    }
}
