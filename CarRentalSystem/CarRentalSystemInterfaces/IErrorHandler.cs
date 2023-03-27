using System;

namespace sf.systems.rentals.cars
{
    public interface IErrorHandler
    {
        void HandleError(Exception e);
    }
}
