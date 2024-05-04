namespace SerializationComparer;

internal static class HomeSerializer
{
    public static string Serialize<T>(T obj, HomeJsonSerializerOptions options = null) 
        => Serializer.ToString(typeof(T), obj, !(options is null) && options.IncludeFields);

    public static T Deserialize<T>(string jsonString, HomeJsonSerializerOptions options = null) 
        => (T)Deserializer.FromString(typeof(T), jsonString, !(options is null) && options.IncludeFields);
}

internal class HomeJsonSerializerOptions
{
    public bool IncludeFields { get; set; } = false;
}