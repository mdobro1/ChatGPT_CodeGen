using System;
using System.IO;
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
            Customer customer = new Customer(id, name, phoneNumber, address, email);
            customers.Add(customer);
            return customer;
        }

        public void RentCar(Customer customer, Car car, DateTime rentalDate, DateTime returnDate)
        {
            if (!availableCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified car is not available for rental."));
            }

            if (customer.RentedCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified customer has already rented the specified car."));
            }

            double totalPrice = car.DailyPrice * (returnDate - rentalDate).TotalDays;
            Transaction transaction = new Transaction(Guid.NewGuid().ToString(), customer, car, rentalDate, returnDate, totalPrice);
            transactions.Add(transaction);
            customer.RentCar(car);
            car.Rent();
            availableCars.Remove(car);
            rentedCars.Add(car);
        }

        public void ReturnCar(Customer customer, Car car, DateTime returnDate)
        {
            if (!rentedCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified car has not been rented by the specified customer."));
            }

            if (!customer.RentedCars.Contains(car))
            {
                errorHandler.HandleError(new InvalidOperationException("The specified customer has not rented the specified car."));
            }

            Transaction transaction = transactions.Find(t => t.Customer == customer && t.Car == car && t.ReturnDate == null);
            if (transaction == null)
            {
                errorHandler.HandleError(new InvalidOperationException("The specified transaction could not be found."));
            }

            transaction.ReturnDate = returnDate;
            car.Return();
            rentedCars.Remove(car);
            availableCars.Add(car);
            customer.ReturnCar(car);
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
            return transactions.FindAll(t => t.Customer == customer);
        }
    }
}
