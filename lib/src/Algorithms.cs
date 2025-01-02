namespace Qwaitumin.GameCore;

public static class Algorithms
{
  public static int ScaleDownNumber(int number, int scale)
    => (int)Math.Floor(((float)number) / scale);
}