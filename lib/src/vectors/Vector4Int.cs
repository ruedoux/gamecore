using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
namespace Qwaitumin.GameCore;

public readonly struct Vector4Int
{
  public readonly int X;
  public readonly int Y;
  public readonly int Z;
  public readonly int W;

  public static Vector4Int Zero => new(0, 0, 0, 0);
  public static Vector4Int One => new(1, 1, 1, 1);

  public readonly int ToIndex(int xMax, int yMax, int zMax)
    => (W * xMax * yMax * zMax) + (Z * xMax * yMax) + (Y * xMax) + X;

  public static Vector4Int FromIndex(int index, int xMax, int yMax, int zMax)
  {
    int w = index / (xMax * yMax * zMax);
    index -= w * xMax * yMax * zMax;
    int z = index / (xMax * yMax);
    index -= z * xMax * yMax;
    int y = index / xMax;
    int x = index % xMax;
    return new Vector4Int(x, y, z, w);
  }

  public readonly Vector4Int ScaleDown(int scale)
      => new(Algorithms.ScaleDownNumber(X, scale), Algorithms.ScaleDownNumber(Y, scale), Algorithms.ScaleDownNumber(Z, scale), Algorithms.ScaleDownNumber(W, scale));

  public static Vector4Int[] MoveArray(Vector4Int[] arr, Vector4Int moveBy)
  {
    Vector4Int[] moved = new Vector4Int[arr.Length];
    for (int i = 0; i < arr.Length; i++)
      moved[i] = arr[i] + moveBy;
    return moved;
  }

  public Vector4Int(int x, int y, int z, int w)
  {
    X = x;
    Y = y;
    Z = z;
    W = w;
  }

  public readonly void Deconstruct(out int x, out int y, out int z, out int w)
  {
    x = X;
    y = Y;
    z = Z;
    w = W;
  }

  public static Vector4Int operator +(Vector4Int left, Vector4Int right)
    => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);

  public static Vector4Int operator -(Vector4Int left, Vector4Int right)
    => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

  public static Vector4Int operator -(Vector4Int vec)
    => new(-vec.X, -vec.Y, -vec.Z, -vec.W);

  public static Vector4Int operator *(Vector4Int vec, int scale)
    => new(vec.X * scale, vec.Y * scale, vec.Z * scale, vec.W * scale);

  public static Vector4Int operator *(int scale, Vector4Int vec)
    => new(vec.X * scale, vec.Y * scale, vec.Z * scale, vec.W * scale);

  public static Vector4Int operator *(Vector4Int left, Vector4Int right)
    => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);

  public static Vector4Int operator /(Vector4Int vec, int divisor)
    => new(vec.X / divisor, vec.Y / divisor, vec.Z / divisor, vec.W / divisor);

  public static Vector4Int operator /(Vector4Int vec, Vector4Int divisorv)
    => new(vec.X / divisorv.X, vec.Y / divisorv.Y, vec.Z / divisorv.Z, vec.W / divisorv.W);

  public static Vector4Int operator %(Vector4Int vec, int divisor)
    => new(vec.X % divisor, vec.Y % divisor, vec.Z % divisor, vec.W % divisor);

  public static Vector4Int operator %(Vector4Int vec, Vector4Int divisorv)
    => new(vec.X % divisorv.X, vec.Y % divisorv.Y, vec.Z % divisorv.Z, vec.W % divisorv.W);

  public static bool operator ==(Vector4Int left, Vector4Int right)
    => left.Equals(right);

  public static bool operator !=(Vector4Int left, Vector4Int right)
    => !left.Equals(right);

  public override readonly bool Equals(object? obj)
  {
    if (obj is not Vector4Int other)
      return false;

    return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
  }

  public override readonly int GetHashCode()
    => HashCode.Combine(X, Y, Z, W);

  public override readonly string ToString()
    => $"({X},{Y},{Z},{W})";

  public static Vector4Int FromString(string text)
  {
    var parts = text.Trim('(', ')').Split(',');
    if (parts.Length != 4)
      throw new ArgumentException($"Unable to convert string to Vector4Int: {text}");
    return new Vector4Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
  }
}

public class Vector4IntConverter : JsonConverter<Vector4Int>
{
  public override Vector4Int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
      throw new JsonException("Expected string token");

    var stringValue = reader!.GetString()
      ?? throw new JsonException("Token is null");

    return Vector4Int.FromString(stringValue);
  }

  public override void Write(Utf8JsonWriter writer, Vector4Int value, JsonSerializerOptions options)
    => writer.WriteStringValue(value.ToString());
}


public class Vector4IntDictionaryConverter<TValue> : JsonConverter<Dictionary<Vector4Int, TValue>>
{
  public override Dictionary<Vector4Int, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  => JsonSerialization.ReadDictionary<Vector4Int, TValue>(
      ref reader, typeToConvert, options, Vector4Int.FromString);

  public override void Write(Utf8JsonWriter writer, Dictionary<Vector4Int, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}


public class Vector4IntImmutableDictionaryConverter<TValue> : JsonConverter<ImmutableDictionary<Vector4Int, TValue>>
{
  public override ImmutableDictionary<Vector4Int, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => JsonSerialization.ReadImmutableDictionary<Vector4Int, TValue>(
      ref reader, typeToConvert, options, Vector4Int.FromString);

  public override void Write(Utf8JsonWriter writer, ImmutableDictionary<Vector4Int, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}