using Qwaitumin.GameCore;

namespace Qwaitumin.GameCoreTests;

[SimpleTestClass]
public class Vector3IntTest
{
  const int EXAMPLE_SIZE = 16;

  [SimpleTestMethod]
  public void ScaleDown_ShouldScaleDownVector_WhenCalled()
  {
    // Given
    Vector3Int vector1 = new(0, 0, 0);
    Vector3Int vector2 = new(16, 16, 16);
    Vector3Int vector3 = new(-16, -16, -16);
    Vector3Int vector4 = new(-17, -17, -17);

    // When
    Vector3Int scaledVector1 = vector1.ScaleDown(EXAMPLE_SIZE);
    Vector3Int scaledVector2 = vector2.ScaleDown(EXAMPLE_SIZE);
    Vector3Int scaledVector3 = vector3.ScaleDown(EXAMPLE_SIZE);
    Vector3Int scaledVector4 = vector4.ScaleDown(EXAMPLE_SIZE);

    // Then
    Assertions.AssertEqual(new Vector3Int(0, 0, 0), scaledVector1);
    Assertions.AssertEqual(new Vector3Int(1, 1, 1), scaledVector2);
    Assertions.AssertEqual(new Vector3Int(-1, -1, -1), scaledVector3);
    Assertions.AssertEqual(new Vector3Int(-2, -2, -2), scaledVector4);
  }

  [SimpleTestMethod]
  public void MoveArray_ShouldMoveAllVectorsInArray_WhenCalled()
  {
    // Given
    Vector3Int[] arr = new Vector3Int[] { new(0, 0, 0), new(1, 1, 1) };

    // When
    Vector3Int[] movedArr = Vector3Int.MoveArray(arr, new(2, 2, 2));

    // Then
    Assertions.AssertEqual(new Vector3Int(2, 2, 2), movedArr[0]);
    Assertions.AssertEqual(new Vector3Int(3, 3, 3), movedArr[1]);
  }

  [SimpleTestMethod]
  public void GetInRange_ShouldReturnVectorsInRange_WhenCalled()
  {
    // Given
    Vector3Int at = new(0, 0, 0);
    Vector3Int range27 = new(1, 1, 1);
    Vector3Int range125 = new(2, 2, 2);

    // When
    Vector3Int[] vectors9 = Vector3Int.GetInRange(at, range27);
    Vector3Int[] vectors25 = Vector3Int.GetInRange(at, range125);

    // Then
    Assertions.AssertEqual(27, vectors9.Length);
    Assertions.AssertEqual(125, vectors25.Length);
  }

  [SimpleTestMethod]
  public void ToIndex_ShouldConvertBackFromIndex_WhenCalled()
  {
    // Given
    const int SIZE = 5;

    // When
    // Then
    for (int x = 0; x < SIZE; x++)
      for (int y = 0; y < SIZE; y++)
        for (int z = 0; z < SIZE; z++)
        {
          Assertions.AssertEqual(
            new Vector3Int(x, y, z), Vector3Int.FromIndex(new Vector3Int(x, y, z).ToIndex(SIZE, SIZE), SIZE, SIZE));
          Assertions.AssertLessThan(
            new Vector3Int(x, y, z).ToIndex(SIZE, SIZE), SIZE * SIZE * SIZE);
        }
  }
}