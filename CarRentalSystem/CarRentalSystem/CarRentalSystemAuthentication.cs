using System;

namespace sf.systems.rentals.cars
{
    public partial class CarRentalSystem
    {
        private readonly AuthenticationManager authenticationManager;
        private User currentUser;

        public CarRentalSystem(AuthenticationManager authenticationManager) : this()
        {
            this.authenticationManager = authenticationManager;
        }

        public bool Login(string username, string password, UserRole userRole)
        {
            bool success = authenticationManager.Login(username, password);
            if (success)
            {
                currentUser = new User(System.Guid.NewGuid().ToString(), username, userRole);
                LogAndShowMessage("Login successful.");
            }
            else
            {
                LogAndShowMessage("Login failed.");
            }
            return success;
        }

        public void Logout()
        {
            authenticationManager.Logout();
            currentUser = null;
            LogAndShowMessage("Logout successful.");
        }

        public bool IsLoggedIn()
        {
            return currentUser != null;
        }

        public User GetCurrentUser()
        {
            return currentUser;
        }
    }
}
