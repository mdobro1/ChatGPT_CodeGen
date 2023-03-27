using System;

namespace sf.systems.rentals.cars
{
    public partial class CarRentalSystem
    {
        private readonly SecurityManager securityManager;

        public CarRentalSystem(SecurityManager securityManager) : this()
        {
            this.securityManager = securityManager;
        }

        public bool Authorize(User user, string permission)
        {
            return securityManager.Authorize(user, permission);
        }

        public void EncryptData()
        {
            foreach (Customer customer in customers)
            {
                customer.PhoneNumber = securityManager.Encrypt(customer.PhoneNumber);
                customer.Address = securityManager.Encrypt(customer.Address);
                customer.Email = securityManager.Encrypt(customer.Email);
            }
        }

        public void DecryptData()
        {
            foreach (Customer customer in customers)
            {
                customer.PhoneNumber = securityManager.Decrypt(customer.PhoneNumber);
                customer.Address = securityManager.Decrypt(customer.Address);
                customer.Email = securityManager.Decrypt(customer.Email);
            }
        }
    }
}
