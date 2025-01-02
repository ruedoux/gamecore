using Qwaitumin.GameCore;

namespace Qwaitumin.GameCoreTests;

[SimpleTestClass]
public class CacheQueueTest
{
  [SimpleTestMethod]
  public void PushValue_ShouldReturnOverflow_WhenOverflowingCache()
  {
    // Given
    int cacheSize = 1;
    TestObject[] testObjects = TestObject.GetList(cacheSize + 1);
    CacheQueue<int, TestObject> cacheQueue = new(cacheSize);

    // When
    KeyValuePair<int, TestObject> cacheEntry1 = new(0, testObjects[0]);
    KeyValuePair<int, TestObject> cacheEntry2 = new(1, testObjects[1]);
    var overflowObject1 = cacheQueue.Push(cacheEntry1);
    var overflowObject2 = cacheQueue.Push(cacheEntry2);

    // Then
    Assertions.AssertNull(overflowObject1);
    Assertions.AssertNotNull(overflowObject2);
    Assertions.AssertEqual(cacheEntry1, overflowObject2);
  }

  [SimpleTestMethod]
  public void PushValue_ShouldReturnOldestOverflow_WhenOverflowingCache()
  {
    // Given
    int cacheSize = 2;
    TestObject[] testObjects = TestObject.GetList(cacheSize + 1);
    CacheQueue<int, TestObject> cacheQueue = new(cacheSize);

    // When
    KeyValuePair<int, TestObject> cacheEntry1 = new(0, testObjects[0]);
    KeyValuePair<int, TestObject> cacheEntry2 = new(1, testObjects[1]);
    KeyValuePair<int, TestObject> cacheEntry3 = new(2, testObjects[2]);
    cacheQueue.Push(cacheEntry1);
    cacheQueue.Push(cacheEntry2);
    cacheQueue.GetValueOrDefault(0);
    var overflowObject3 = cacheQueue.Push(cacheEntry3);

    // Then
    Assertions.AssertNotNull(overflowObject3);
    Assertions.AssertEqual(cacheEntry2, overflowObject3);
  }

  [SimpleTestMethod]
  public void GetValueOrDefault_ShouldReturnValue_WhenValueInCache()
  {
    // Given
    int cacheSize = 1;
    TestObject[] testObjects = TestObject.GetList(cacheSize);
    CacheQueue<int, TestObject> cacheQueue = new(cacheSize);

    // When
    KeyValuePair<int, TestObject> cacheEntry1 = new(0, testObjects[0]);
    cacheQueue.Push(cacheEntry1);
    var getObject = cacheQueue.GetValueOrDefault(0);

    // Then
    Assertions.AssertNotNull(getObject);
    Assertions.AssertEqual(cacheEntry1.Value, getObject);
  }

  [SimpleTestMethod]
  public void GetValueOrDefault_ShouldReturnNull_WhenValueNotInCache()
  {
    // Given
    int cacheSize = 1;
    CacheQueue<int, TestObject> cacheQueue = new(cacheSize);

    // When
    var getObject = cacheQueue.GetValueOrDefault(0);

    // Then
    Assertions.AssertNull(getObject);
  }

  [SimpleTestMethod]
  public void Flush_ShouldClearAndReturnCache_WhenCalled()
  {
    // Given
    int cacheSize = 2;
    TestObject[] testObjects = TestObject.GetList(cacheSize);
    CacheQueue<int, TestObject> cacheQueue = new(cacheSize);

    // When
    KeyValuePair<int, TestObject> cacheEntry1 = new(0, testObjects[0]);
    KeyValuePair<int, TestObject> cacheEntry2 = new(1, testObjects[1]);
    cacheQueue.Push(cacheEntry1);
    cacheQueue.Push(cacheEntry2);
    var flushed = cacheQueue.Flush();
    var flushedAgain = cacheQueue.Flush();

    // Then
    Assertions.AssertNotNull(flushedAgain);
    Assertions.AssertTrue(flushedAgain.Count == 0);
    Assertions.AssertEqual(2, flushed.Count);
    Assertions.AssertTrue(flushed.Contains(cacheEntry1));
    Assertions.AssertTrue(flushed.Contains(cacheEntry2));
  }
}