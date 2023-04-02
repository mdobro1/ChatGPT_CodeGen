namespace sf.systems.rentals.cars
{
    public interface ICar
    {
        double DailyPrice { get; }
        string Id { get; }
        string Make { get; }
        string Model { get; }
        bool Rented { get; }
        int Year { get; }

        Car DeserializeHandler(string data, DataType dataType);
        string Serialize(DataType dataType);
    }
}