using System;
using System.Collections.Generic;
using System.Text;

namespace sf.systems.rentals.cars
{
    public delegate CarRentalContext CarRentalContextAction(CarRentalContext rentalContext);
    public class CarRentalContext
    {
        public string Command { get; set; }
        public string CustomerID { get; set; }
        public string CarID { get; set; }
        public DateTime RentFrom { get; set; }
        public DateTime RentTo { get; set; }
        public CarRentalSystem CarRentalSystem { get; set; }
        public Transaction RentalTransaction { get; set; }
        public bool ActionCompleted { get; set; }
        public static DateTime ParseDate(string strDate, DateTime defaultDate)
        {
            DateTime resultDate = default;
            if (!DateTime.TryParse(strDate, out resultDate))
                resultDate = defaultDate;
            return resultDate;
        }
    }
}