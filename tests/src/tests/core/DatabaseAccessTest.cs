using Qwaitumin.GameCore;
using LiteDB;

namespace Qwaitumin.GameCoreTests;

[SimpleTestClass]
public class DatabaseAccessTest
{
  [SimpleTestMethod]
  public void AddEntryGetEntry_ShouldAddAndGetEntry_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath("test.db"));
    int key = 1;
    TestObject testObject = new(1);
    DatabaseAccess<int, TestObject> databaseAccess = new(db, "test");

    // When
    databaseAccess.AddEntry(key, testObject);
    var savedEntry = databaseAccess.GetEntry(key);

    // Then
    Assertions.AssertEqual(testObject, savedEntry);
  }

  [SimpleTestMethod]
  public void GetEntry_ShouldReturnNull_WhenNoEntryPresent()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath("test.db"));
    DatabaseAccess<int, TestObject> databaseAccess = new(db, "test");

    // When
    var savedEntry = databaseAccess.GetEntry(1);

    // Then
    Assertions.AssertNull(savedEntry);
  }

  [SimpleTestMethod]
  public void RemoveEntry_ShouldRemoveEntry_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using var db = new LiteDatabase(testDirectory.GetRelativePath("test.db"));
    DatabaseAccess<int, TestObject> databaseAccess = new(db, "test");
    TestObject testObject1 = new(1);

    // When
    databaseAccess.AddEntry(1, testObject1);
    databaseAccess.RemoveEntry(1);

    // Then
    Assertions.AssertNull(databaseAccess.GetEntry(1));
  }
}