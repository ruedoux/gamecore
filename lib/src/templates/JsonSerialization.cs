using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Qwaitumin.GameCore;

public static class JsonSerialization
{
  public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(
    ref Utf8JsonReader reader,
    Type typeToConvert,
    JsonSerializerOptions options,
    Func<string, TKey> keyDeserialization) where TKey : notnull
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException("Expected start object token");

    var dictionary = new Dictionary<TKey, TValue>();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
        return dictionary;

      string keyString = reader.GetString() ?? throw new JsonException();
      reader.Read();

      TValue value = JsonSerializer.Deserialize(ref reader, typeInfo)
        ?? throw new JsonException("Value deserialization results in null");
      dictionary.Add(keyDeserialization(keyString), value);
    }

    throw new JsonException("Object did not end with end token");
  }

  public static ImmutableDictionary<TKey, TValue> ReadImmutableDictionary<TKey, TValue>(
    ref Utf8JsonReader reader,
    Type typeToConvert,
    JsonSerializerOptions options,
    Func<string, TKey> keyDeserialization) where TKey : notnull
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException("Expected start object token");

    var dictionary = new Dictionary<TKey, TValue>();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
        return dictionary.ToImmutableDictionary();

      string keyString = reader.GetString() ?? throw new JsonException();
      reader.Read();
      TValue value = JsonSerializer.Deserialize(ref reader, typeInfo)
        ?? throw new JsonException("Value deserialization results in null");
      dictionary.Add(keyDeserialization(keyString), value);
    }

    throw new JsonException("Object did not end with end token");
  }

  public static void Write<TKey, TValue>(
    Utf8JsonWriter writer,
    Dictionary<TKey, TValue> value,
    JsonSerializerOptions options,
    Func<TKey, string> KeySerialization) where TKey : notnull
  {
    writer.WriteStartObject();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    foreach (var kvp in value)
    {
      writer.WritePropertyName(KeySerialization(kvp.Key));
      JsonSerializer.Serialize(writer, kvp.Value, typeInfo);
    }

    writer.WriteEndObject();
  }

  public static void Write<TKey, TValue>(
    Utf8JsonWriter writer,
    ImmutableDictionary<TKey, TValue> value,
    JsonSerializerOptions options,
    Func<TKey, string> KeySerialization) where TKey : notnull
  {
    writer.WriteStartObject();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    foreach (var kvp in value)
    {
      writer.WritePropertyName(KeySerialization(kvp.Key));
      JsonSerializer.Serialize(writer, kvp.Value, typeInfo);
    }

    writer.WriteEndObject();
  }
}