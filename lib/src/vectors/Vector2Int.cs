using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Qwaitumin.GameCore;

public readonly struct Vector2Int
{
  public readonly int X;
  public readonly int Y;

  public static Vector2Int Zero => new(0, 0);
  public static Vector2Int One => new(1, 1);

  public static Vector2Int TopLeft => new(-1, -1);
  public static Vector2Int Top => new(0, -1);
  public static Vector2Int TopRight => new(1, -1);
  public static Vector2Int Right => new(1, 0);
  public static Vector2Int BottomRight => new(1, 1);
  public static Vector2Int Bottom => new(0, 1);
  public static Vector2Int BottomLeft => new(-1, 1);
  public static Vector2Int Left => new(-1, 0);

  public readonly int ToIndex(int xSize)
    => (Y * xSize) + X;

  public static Vector2Int FromIndex(int index, int xSize)
  {
    int y = index / xSize;
    int x = index % xSize;
    return new Vector2Int(x, y);
  }

  public readonly Vector2Int ScaleDown(int scale)
    => new(Algorithms.ScaleDownNumber(X, scale), Algorithms.ScaleDownNumber(Y, scale));

  public static Vector2Int[] GetInRange(
      Vector2Int position, Vector2Int range)
  {
    Vector2Int[] matrix = new Vector2Int[(range.X * 2 + 1) * (range.Y * 2 + 1)];
    int index = 0;
    for (int x = -range.X; x < range.X + 1; x++)
      for (int y = -range.Y; y < range.Y + 1; y++)
        matrix[index++] = position + new Vector2Int(x, y);

    return matrix;
  }

  public static Vector2Int[] GetInRectangle(Vector2Int at, Vector2Int size)
  {
    Vector2Int[] arr = new Vector2Int[size.X * size.Y];

    int index = 0;
    for (int x = 0; x < size.X; x++)
      for (int y = 0; y < size.Y; y++)
        arr[index++] = at + new Vector2Int(x, y);

    return arr;
  }

  public static Vector2Int[] MoveArray(Vector2Int[] arr, Vector2Int moveBy)
  {
    Vector2Int[] moved = new Vector2Int[arr.Length];
    for (int i = 0; i < arr.Length; i++)
      moved[i] = arr[i] + moveBy;
    return moved;
  }

  public Vector2Int(int x, int y)
  {
    X = x;
    Y = y;
  }

  public readonly void Deconstruct(out int x, out int y)
  {
    x = X;
    y = Y;
  }

  public static Vector2Int operator +(Vector2Int left, Vector2Int right)
    => new(left.X + right.X, left.Y + right.Y);

  public static Vector2Int operator -(Vector2Int left, Vector2Int right)
    => new(left.X - right.X, left.Y - right.Y);

  public static Vector2Int operator -(Vector2Int vec)
    => new(-vec.X, -vec.Y);

  public static Vector2Int operator *(Vector2Int vec, int scale)
    => new(vec.X * scale, vec.Y * scale);

  public static Vector2Int operator *(int scale, Vector2Int vec)
    => new(vec.X * scale, vec.Y * scale);

  public static Vector2Int operator *(Vector2Int left, Vector2Int right)
    => new(left.X * right.X, left.Y * right.Y);

  public static Vector2Int operator /(Vector2Int vec, int divisor)
    => new(vec.X / divisor, vec.Y / divisor);

  public static Vector2Int operator /(Vector2Int vec, Vector2Int divisorv)
    => new(vec.X / divisorv.X, vec.Y / divisorv.Y);

  public static Vector2Int operator %(Vector2Int vec, int divisor)
    => new(vec.X % divisor, vec.Y % divisor);

  public static Vector2Int operator %(Vector2Int vec, Vector2Int divisorv)
    => new(vec.X % divisorv.X, vec.Y % divisorv.Y);

  public static bool operator ==(Vector2Int left, Vector2Int right)
    => left.Equals(right);

  public static bool operator !=(Vector2Int left, Vector2Int right)
    => !left.Equals(right);

  public override readonly bool Equals(object? obj)
  {
    if (obj is not Vector2Int other)
      return false;

    return X == other.X && Y == other.Y;
  }

  public override readonly int GetHashCode()
    => HashCode.Combine(X, Y);

  public override readonly string ToString()
    => $"({X},{Y})";

  public static Vector2Int FromString(string text)
  {
    var parts = text.Trim('(', ')').Split(',');
    if (parts.Length != 2)
      throw new ArgumentException($"Unable to convert string to Vector2Int: {text}");
    return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
  }
}

public class Vector2IntConverter : JsonConverter<Vector2Int>
{
  public override Vector2Int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
      throw new JsonException("Expected string token");

    var stringValue = reader!.GetString()
      ?? throw new JsonException("Token is null");

    return Vector2Int.FromString(stringValue);
  }

  public override void Write(Utf8JsonWriter writer, Vector2Int value, JsonSerializerOptions options)
    => writer.WriteStringValue(value.ToString());
}

public class Vector2IntDictionaryConverter<TValue> : JsonConverter<Dictionary<Vector2Int, TValue>>
{
  public override Dictionary<Vector2Int, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  => JsonSerialization.ReadDictionary<Vector2Int, TValue>(
      ref reader, typeToConvert, options, Vector2Int.FromString);

  public override void Write(Utf8JsonWriter writer, Dictionary<Vector2Int, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}


public class Vector2IntImmutableDictionaryConverter<TValue> : JsonConverter<ImmutableDictionary<Vector2Int, TValue>>
{
  public override ImmutableDictionary<Vector2Int, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => JsonSerialization.ReadImmutableDictionary<Vector2Int, TValue>(
      ref reader, typeToConvert, options, Vector2Int.FromString);

  public override void Write(Utf8JsonWriter writer, ImmutableDictionary<Vector2Int, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}