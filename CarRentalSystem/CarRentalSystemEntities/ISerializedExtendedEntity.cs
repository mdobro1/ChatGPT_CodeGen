namespace sf.systems.rentals.cars
{
    public interface ISerializedExtendedEntity<T> : ISerializedEntity<T>
    {
        T DeserializeHandler(string data, DataType dataType, IEntitiesList entitiesList);
    }
}
