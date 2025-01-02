using System.Drawing;

namespace Qwaitumin.GameCore;


public struct ColorWrapper
{
  public byte Red = default;
  public byte Green = default;
  public byte Blue = default;
  public byte Alpha = default;

  public ColorWrapper() { }

  public ColorWrapper(Color color)
  {
    Red = color.R;
    Green = color.G;
    Blue = color.B;
    Alpha = color.A;
  }

  public ColorWrapper(byte red, byte green, byte blue, byte alpha)
  {
    Red = red;
    Green = green;
    Blue = blue;
    Alpha = alpha;
  }

  public readonly Color Get()
    => Color.FromArgb(Alpha, Red, Green, Blue);

  public override readonly string ToString()
    => $"({Red}, {Green}, {Blue}, {Alpha})";

  public override readonly bool Equals(object? obj)
  {
    if (obj is not ColorWrapper other)
      return false;

    return Red == other.Red
      && Green == other.Green
      && Blue == other.Blue
      && Alpha == other.Alpha;
  }
  public override readonly int GetHashCode()
    => HashCode.Combine(Red, Green, Blue, Alpha);

  public static bool operator ==(ColorWrapper left, ColorWrapper right)
    => left.Equals(right);

  public static bool operator !=(ColorWrapper left, ColorWrapper right)
    => !left.Equals(right);
}