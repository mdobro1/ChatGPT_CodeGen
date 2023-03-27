using System;
using System.Collections.Generic;
using System.Text;

namespace sf.systems.rentals.cars
{
    public partial class CarRentalSystem
    {
        public void Log(string message)
        {
            messageHandler.Log(message);
        }

        public void ShowMessage(string message)
        {
            messageHandler.ShowMessage(message);
        }

        public void LogAndShowMessage(string message)
        {
            messageHandler.Log(message);
            messageHandler.ShowMessage(message);
        }

        public void HandleError(Exception ex)
        {
            errorHandler.HandleError(ex);
        }
    }
}
