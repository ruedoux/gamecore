using System.Linq;

namespace Qwaitumin.GameCoreTests;

public class TestObject
{
  public int Number { get; set; }

  public TestObject(int number)
  {
    this.Number = number;
  }

  public static TestObject[] GetList(int size)
    => Enumerable.Range(0, size).Select(i => new TestObject(i)).ToArray();

  public override bool Equals(object? obj)
  {
    if (obj is not TestObject other)
      return false;

    return Number == other.Number;
  }

  public override int GetHashCode()
    => System.HashCode.Combine(Number);
}