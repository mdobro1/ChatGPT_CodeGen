using System;
using System.Collections.Generic;
using System.IO;


namespace sf.systems.rentals.cars
{
    public class DataManager
    {
        private const string DataFolderPath = "data";

        private readonly ErrorHandler errorHandler;
        private readonly MessageHandler messageHandler;

        public DataManager(ErrorHandler errorHandler, MessageHandler messageHandler)
        {
            this.errorHandler = errorHandler;
            this.messageHandler = messageHandler;
        }

        public List<T> ReadData<T>(EntityType entityType, DataType dataType) 
            where T : ISerializedEntity<T>, new()
        {
            string filePath = GetFilePath(entityType, dataType);
            List<T> dataList = new List<T>();

            if (File.Exists(filePath))
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        T data = Deserialize<T>(line, dataType);
                        if (data != null)
                        {
                            dataList.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(ex);
            }

            return dataList;
        }

        public bool WriteData<T>(List<T> dataList, EntityType entityType, DataType dataType) 
            where T : ISerializedEntity<T>, new()
        {
            string filePath = GetFilePath(entityType, dataType);

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (T data in dataList)
                    {
                        string line = Serialize<T>(data, dataType);
                        writer.WriteLine(line);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(ex);
                return false;
            }
        }

        private string GetFilePath(EntityType entityType, DataType dataType)
        {
            string fileName = $"{entityType.ToString().ToLower()}.{dataType.ToString().ToLower()}";
            return Path.Combine(DataFolderPath, fileName);
        }

        private string Serialize<T>(T data, DataType dataType) where T : ISerializedEntity<T>
        {
            if (data == null) throw new ArgumentNullException("data");

            switch (dataType)
            {
                case DataType.CSV:
                case DataType.JSON:
                    return data.Serialize(dataType);
                default:
                    throw new ArgumentException($"Invalid data type: {dataType}");
            }
        }

        private T Deserialize<T>(string data, DataType dataType) 
            where T : ISerializedEntity<T>, new()
        {
            if (data == null) throw new ArgumentNullException("data");

            switch (dataType)
            {
                case DataType.CSV:
                case DataType.JSON:
                    return new T().DeserializeHandler(data, dataType);
                default:
                    throw new ArgumentException($"Invalid data type: {dataType}");
            }
        }

        public static List<string> ReadLinesFromFile(string filePath)
        {
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while reading the file: " + ex.Message);
            }

            return lines;
        }
        public static void WriteLinesToFile(string filePath, List<string> lines)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (string line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
            }
        }
    }
}