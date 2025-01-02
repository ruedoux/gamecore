using Qwaitumin.GameCore;
using LiteDB;

namespace Qwaitumin.GameCoreTests;

[SimpleTestClass]
public class CachedDatabaseAccessTest
{
  private readonly string dbFileName = "test.db";

  [SimpleTestMethod]
  public void GetEntryCached_ShouldReturnNull_WhenNoEntryPresent()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath(dbFileName));
    CachedDatabaseAccess<int, TestObject> databaseAccess = new(db, "test", 1);

    // When
    var savedEntry = databaseAccess.GetEntry(1);

    // Then
    Assertions.AssertNull(savedEntry);
  }

  [SimpleTestMethod]
  public void AddEntryGetEntry_ShouldAddAndGetEntry_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath(dbFileName));
    int key = 1;
    TestObject testObject = new(1);
    CachedDatabaseAccess<int, TestObject> databaseAccess = new(db, "test", 1);

    // When
    databaseAccess.AddEntry(key, testObject);
    var savedEntry = databaseAccess.GetEntry(key);

    // Then
    Assertions.AssertEqual(testObject, savedEntry);
  }

  [SimpleTestMethod]
  public void AddEntryGetEntry_ShouldAddAndGetEntryEvenWhenCacheOverflowed_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath(dbFileName));
    int key1 = 1;
    int key2 = 2;
    int key3 = 3;
    TestObject testObject1 = new(1);
    TestObject testObject2 = new(2);
    TestObject testObject3 = new(3);
    CachedDatabaseAccess<int, TestObject> databaseAccess = new(db, "test", 1);

    // When
    databaseAccess.AddEntry(key1, testObject1);
    databaseAccess.AddEntry(key2, testObject2);
    databaseAccess.AddEntry(key3, testObject3);
    var savedEntry1 = databaseAccess.GetEntry(key1);
    var savedEntry2 = databaseAccess.GetEntry(key2);
    var savedEntry3 = databaseAccess.GetEntry(key3);

    // Then
    Assertions.AssertEqual(testObject1, savedEntry1);
    Assertions.AssertEqual(testObject2, savedEntry2);
    Assertions.AssertEqual(testObject3, savedEntry3);
  }

  [SimpleTestMethod]
  public void FlushCache_ShouldSaveAllCache_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath(dbFileName));
    CachedDatabaseAccess<int, TestObject> databaseAccess = new(db, "test", 2);
    TestObject testObject1 = new(1);
    TestObject testObject2 = new(2);

    // When
    databaseAccess.AddEntry(1, testObject1);
    databaseAccess.AddEntry(2, testObject2);
    databaseAccess.FlushCache();

    // Then
    CachedDatabaseAccess<int, TestObject> databaseAccessAfterFlush = new(db, "test", 2);
    Assertions.AssertEqual(testObject1, databaseAccessAfterFlush.GetEntry(1));
    Assertions.AssertEqual(testObject2, databaseAccessAfterFlush.GetEntry(2));
  }

  [SimpleTestMethod]
  public void RemoveEntry_ShouldRemoveEntry_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath(dbFileName));
    CachedDatabaseAccess<int, TestObject> databaseAccess = new(db, "test", 2);
    TestObject testObject1 = new(1);

    // When
    databaseAccess.AddEntry(1, testObject1);
    databaseAccess.RemoveEntry(1);

    // Then
    Assertions.AssertNull(databaseAccess.GetEntry(1));
  }
}