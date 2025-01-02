using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Qwaitumin.GameCore;

public abstract class JsonSerializable
{
  protected JsonTypeInfo jsonTypeInfo;

  protected JsonSerializable(JsonTypeInfo jsonTypeInfo)
  {
    this.jsonTypeInfo = jsonTypeInfo;
  }

  public static T? FromJsonString<T>(string jsonString, JsonTypeInfo<T> jsonType)
    where T : JsonSerializable
      => JsonSerializer.Deserialize(jsonString, jsonType);

  public string ToJsonString()
      => JsonSerializer.Serialize(this, jsonTypeInfo);

  public static T LoadObjectFromFile<T>(string filePath, JsonTypeInfo<T> jsonType)
  {
    string jsonString = File.ReadAllText(filePath);
    var deserializedObject = JsonSerializer.Deserialize(jsonString, jsonType);
    Assertions.AssertNotNull(deserializedObject);
    return deserializedObject;
  }

  public override string ToString() => ToJsonString();
}
