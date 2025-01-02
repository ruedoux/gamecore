using Qwaitumin.GameCore;

namespace Qwaitumin.GameCoreTests;

[SimpleTestClass]
public class Vector2IntTest
{
  const int EXAMPLE_SIZE = 16;

  [SimpleTestMethod]
  public void ScaleDown_ShouldScaleDownVector_WhenCalled()
  {
    // Given
    Vector2Int vector1 = new(0, 0);
    Vector2Int vector2 = new(16, 16);
    Vector2Int vector3 = new(-16, -16);
    Vector2Int vector4 = new(-17, -17);

    // When
    Vector2Int scaledVector1 = vector1.ScaleDown(EXAMPLE_SIZE);
    Vector2Int scaledVector2 = vector2.ScaleDown(EXAMPLE_SIZE);
    Vector2Int scaledVector3 = vector3.ScaleDown(EXAMPLE_SIZE);
    Vector2Int scaledVector4 = vector4.ScaleDown(EXAMPLE_SIZE);

    // Then
    Assertions.AssertEqual(new Vector2Int(0, 0), scaledVector1);
    Assertions.AssertEqual(new Vector2Int(1, 1), scaledVector2);
    Assertions.AssertEqual(new Vector2Int(-1, -1), scaledVector3);
    Assertions.AssertEqual(new Vector2Int(-2, -2), scaledVector4);
  }

  [SimpleTestMethod]
  public void MoveArray_ShouldMoveAllVectorsInArray_WhenCalled()
  {
    // Given
    Vector2Int[] arr = new Vector2Int[] { new(0, 0), new(1, 1) };

    // When
    Vector2Int[] movedArr = Vector2Int.MoveArray(arr, new(2, 2));

    // Then
    Assertions.AssertEqual(new Vector2Int(2, 2), movedArr[0]);
    Assertions.AssertEqual(new Vector2Int(3, 3), movedArr[1]);
  }

  [SimpleTestMethod]
  public void GetInRange_ShouldReturnVectorsInRange_WhenCalled()
  {
    // Given
    Vector2Int at = new(0, 0);
    Vector2Int range9 = new(1, 1);
    Vector2Int range25 = new(2, 2);

    // When
    Vector2Int[] vectors9 = Vector2Int.GetInRange(at, range9);
    Vector2Int[] vectors25 = Vector2Int.GetInRange(at, range25);

    // Then
    Assertions.AssertEqual(9, vectors9.Length);
    Assertions.AssertEqual(25, vectors25.Length);
  }

  [SimpleTestMethod]
  public void GetInRectangle_ShouldReturnVectorsInRectangle_WhenCalled()
  {
    // Given
    Vector2Int at = new(0, 0);
    Vector2Int size = new(2, 2);
    Vector2Int[] shouldContain = new Vector2Int[] { new(0, 0), new(0, 1), new(1, 0), new(1, 1) };

    // When
    var result = Vector2Int.GetInRectangle(at, size);

    // Then
    Assertions.AssertEqual(4, result.Length);
    foreach (var element in shouldContain)
      Assertions.AssertTrue(result.Contains(element));
  }

  [SimpleTestMethod]
  public void ToIndex_ShouldConvertBackFromIndex_WhenCalled()
  {
    // Given
    const int SIZE = 5;

    // When
    // Then
    for (int x = 0; x < SIZE; x++)
    {
      for (int y = 0; y < SIZE; y++)
      {
        Assertions.AssertEqual(
          new Vector2Int(x, y),
          Vector2Int.FromIndex(new Vector2Int(x, y).ToIndex(SIZE), SIZE));
        Assertions.AssertLessThan(new Vector2Int(x, y).ToIndex(SIZE), SIZE * SIZE);
      }
    }
  }

  [SimpleTestMethod]
  public void ToIndex_ShouldCorrectlyConvertIndex_WhenCalled()
  {
    // Given
    int xSize = 2;
    int[] testArr = { 1, 2, 3, 4 };

    // When
    var tl = testArr[new Vector2Int(0, 0).ToIndex(xSize)];
    var tr = testArr[new Vector2Int(1, 0).ToIndex(xSize)];
    var bl = testArr[new Vector2Int(0, 1).ToIndex(xSize)];
    var br = testArr[new Vector2Int(1, 1).ToIndex(xSize)];

    // Then
    Assertions.AssertEqual(1, tl);
    Assertions.AssertEqual(2, tr);
    Assertions.AssertEqual(3, bl);
    Assertions.AssertEqual(4, br);
  }
}