namespace EasyCards.Services;

public interface IJsonDeserializer
{
    T Deserialize<T>(string json);
}
