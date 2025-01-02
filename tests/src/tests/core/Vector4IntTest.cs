using Qwaitumin.GameCore;

namespace Qwaitumin.GameCoreTests;

[SimpleTestClass]
public class Vector4IntTest
{
  const int EXAMPLE_SIZE = 16;

  [SimpleTestMethod]
  public void ScaleDown_ShouldScaleDownVector_WhenCalled()
  {
    // Given
    Vector4Int vector1 = new(0, 0, 0, 0);
    Vector4Int vector2 = new(16, 16, 16, 16);
    Vector4Int vector3 = new(-16, -16, -16, -16);
    Vector4Int vector4 = new(-17, -17, -17, -17);

    // When
    Vector4Int scaledVector1 = vector1.ScaleDown(EXAMPLE_SIZE);
    Vector4Int scaledVector2 = vector2.ScaleDown(EXAMPLE_SIZE);
    Vector4Int scaledVector3 = vector3.ScaleDown(EXAMPLE_SIZE);
    Vector4Int scaledVector4 = vector4.ScaleDown(EXAMPLE_SIZE);

    // Then
    Assertions.AssertEqual(new Vector4Int(0, 0, 0, 0), scaledVector1);
    Assertions.AssertEqual(new Vector4Int(1, 1, 1, 1), scaledVector2);
    Assertions.AssertEqual(new Vector4Int(-1, -1, -1, -1), scaledVector3);
    Assertions.AssertEqual(new Vector4Int(-2, -2, -2, -2), scaledVector4);
  }

  [SimpleTestMethod]
  public void MoveArray_ShouldMoveAllVectorsInArray_WhenCalled()
  {
    // Given
    Vector4Int[] arr = new Vector4Int[] { new(0, 0, 0, 0), new(1, 1, 1, 1) };

    // When
    Vector4Int[] movedArr = Vector4Int.MoveArray(arr, new(2, 2, 2, 2));

    // Then
    Assertions.AssertEqual(new Vector4Int(2, 2, 2, 2), movedArr[0]);
    Assertions.AssertEqual(new Vector4Int(3, 3, 3, 3), movedArr[1]);
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
          for (int w = 0; w < SIZE; w++)
          {
            Assertions.AssertEqual(
              new Vector4Int(x, y, z, w),
              Vector4Int.FromIndex(new Vector4Int(x, y, z, w).ToIndex(SIZE, SIZE, SIZE), SIZE, SIZE, SIZE));
            Assertions.AssertLessThan(
              new Vector4Int(x, y, z, w).ToIndex(SIZE, SIZE, SIZE),
              SIZE * SIZE * SIZE * SIZE);
          }
  }
}