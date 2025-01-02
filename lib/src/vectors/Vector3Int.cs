using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Qwaitumin.GameCore;

public readonly struct Vector3Int
{
  public readonly int X;
  public readonly int Y;
  public readonly int Z;

  public static Vector3Int Zero => new(0, 0, 0);
  public static Vector3Int One => new(1, 1, 1);

  public readonly int ToIndex(int xMax, int yMax)
    => (Z * xMax * yMax) + (Y * xMax) + X;

  public static Vector3Int FromIndex(int index, int xMax, int yMax)
  {
    int z = index / (xMax * yMax);
    index -= z * xMax * yMax;
    int y = index / xMax;
    int x = index % xMax;
    return new Vector3Int(x, y, z);
  }

  public readonly Vector3Int ScaleDown(int scale)
      => new(Algorithms.ScaleDownNumber(X, scale), Algorithms.ScaleDownNumber(Y, scale), Algorithms.ScaleDownNumber(Z, scale));

  public static Vector3Int[] GetInRange(
    Vector3Int position, Vector3Int range)
  {
    Vector3Int[] matrix = new Vector3Int[(range.X * 2 + 1) * (range.Y * 2 + 1) * (range.Z * 2 + 1)];
    int index = 0;
    for (int x = -range.X; x < range.X + 1; x++)
      for (int y = -range.Y; y < range.Y + 1; y++)
        for (int z = -range.Z; z < range.Z + 1; z++)
          matrix[index++] = position + new Vector3Int(x, y, z);

    return matrix;
  }

  public static Vector3Int[] GetInCuboid(Vector3Int at, Vector3Int size)
  {
    Vector3Int[] arr = new Vector3Int[size.X * size.Y * size.Z];

    int index = 0;
    for (int x = 0; x < size.X; x++)
      for (int y = 0; y < size.Y; y++)
        for (int z = 0; z < size.Z; z++)
          arr[index++] = at + new Vector3Int(x, y, z);

    return arr;
  }

  public static Vector3Int[] MoveArray(Vector3Int[] arr, Vector3Int moveBy)
  {
    Vector3Int[] moved = new Vector3Int[arr.Length];
    for (int i = 0; i < arr.Length; i++)
      moved[i] = arr[i] + moveBy;
    return moved;
  }

  public Vector3Int(int x, int y, int z)
  {
    X = x;
    Y = y;
    Z = z;
  }

  public readonly void Deconstruct(out int x, out int y, out int z)
  {
    x = X;
    y = Y;
    z = Z;
  }

  public static Vector3Int operator +(Vector3Int left, Vector3Int right)
    => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

  public static Vector3Int operator -(Vector3Int left, Vector3Int right)
    => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

  public static Vector3Int operator -(Vector3Int vec)
    => new(-vec.X, -vec.Y, -vec.Z);

  public static Vector3Int operator *(Vector3Int vec, int scale)
    => new(vec.X * scale, vec.Y * scale, vec.Z * scale);

  public static Vector3Int operator *(int scale, Vector3Int vec)
    => new(vec.X * scale, vec.Y * scale, vec.Z * scale);

  public static Vector3Int operator *(Vector3Int left, Vector3Int right)
    => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

  public static Vector3Int operator /(Vector3Int vec, int divisor)
    => new(vec.X / divisor, vec.Y / divisor, vec.Z / divisor);

  public static Vector3Int operator /(Vector3Int vec, Vector3Int divisorv)
    => new(vec.X / divisorv.X, vec.Y / divisorv.Y, vec.Z / divisorv.Z);

  public static Vector3Int operator %(Vector3Int vec, int divisor)
    => new(vec.X % divisor, vec.Y % divisor, vec.Z % divisor);

  public static Vector3Int operator %(Vector3Int vec, Vector3Int divisorv)
    => new(vec.X % divisorv.X, vec.Y % divisorv.Y, vec.Z % divisorv.Z);

  public static bool operator ==(Vector3Int left, Vector3Int right)
    => left.Equals(right);

  public static bool operator !=(Vector3Int left, Vector3Int right)
    => !left.Equals(right);

  public override readonly bool Equals(object? obj)
  {
    if (obj is not Vector3Int other)
      return false;

    return X == other.X && Y == other.Y && Z == other.Z;
  }

  public override readonly int GetHashCode()
    => HashCode.Combine(X, Y, Z);

  public override readonly string ToString()
    => $"({X},{Y},{Z})";

  public static Vector3Int FromString(string text)
  {
    var parts = text.Trim('(', ')').Split(',');
    if (parts.Length != 3)
      throw new ArgumentException($"Unable to convert string to Vector3Int: {text}");
    return new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
  }
}

public class Vector3IntConverter : JsonConverter<Vector3Int>
{
  public override Vector3Int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
      throw new JsonException("Expected string token");

    var stringValue = reader!.GetString()
      ?? throw new JsonException("Token is null");

    return Vector3Int.FromString(stringValue);
  }

  public override void Write(Utf8JsonWriter writer, Vector3Int value, JsonSerializerOptions options)
    => writer.WriteStringValue(value.ToString());
}

public class Vector3IntDictionaryConverter<TValue> : JsonConverter<Dictionary<Vector3Int, TValue>>
{
  public override Dictionary<Vector3Int, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  => JsonSerialization.ReadDictionary<Vector3Int, TValue>(
      ref reader, typeToConvert, options, Vector3Int.FromString);

  public override void Write(Utf8JsonWriter writer, Dictionary<Vector3Int, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}


public class Vector3IntImmutableDictionaryConverter<TValue> : JsonConverter<ImmutableDictionary<Vector3Int, TValue>>
{
  public override ImmutableDictionary<Vector3Int, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => JsonSerialization.ReadImmutableDictionary<Vector3Int, TValue>(
      ref reader, typeToConvert, options, Vector3Int.FromString);

  public override void Write(Utf8JsonWriter writer, ImmutableDictionary<Vector3Int, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}