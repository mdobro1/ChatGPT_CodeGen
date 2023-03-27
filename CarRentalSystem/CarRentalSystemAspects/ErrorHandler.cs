using System;
using System.IO;

namespace sf.systems.rentals.cars
{
    public class ErrorHandler
    {
        private const string ErrorFilePath = "errors.txt";
        private readonly bool rethrow;

        public ErrorHandler(bool rethrow = false)
        {
            this.rethrow = rethrow;
        }

        public void HandleError(Exception ex)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(ErrorFilePath))
                {
                    writer.WriteLine($"{DateTime.Now.ToString()} - {ex.Message}");
                    writer.WriteLine(ex.StackTrace);
                }
            }
            catch (Exception)
            {
                // If an error occurs while writing to the error file, rethrow it
                throw;
            }

            if (rethrow)
            {
                throw ex;
            }
        }
    }
}
