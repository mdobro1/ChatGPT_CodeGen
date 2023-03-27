using System;
using System.IO;

namespace sf.systems.rentals.cars
{
    public class MessageHandler : IMessageHandler
    {
        private const string LogFilePath = "log.txt";
        private readonly ErrorHandler errorHandler;

        public MessageHandler(ErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public void Log(string message)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(LogFilePath))
                {
                    writer.WriteLine($"{DateTime.Now.ToString()} - {message}");
                }
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(ex);
            }
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void LogPlusMessage(string message)
        {
            Log(message);
            ShowMessage(message);
        }
    }
}
