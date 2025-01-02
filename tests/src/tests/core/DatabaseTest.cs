using Qwaitumin.GameCore;

namespace Qwaitumin.GameCoreTests;

class DatabaseMock : Database
{
  private DatabaseAccess<int, int> access = null!;

  public DatabaseMock(string filePath) : base(filePath) { }

  public void Add(int key, int value)
    => PerformWrite(() => access.AddEntry(key, value));

  public int? Get(int key)
    => PerformRead(() => access.GetEntry(key));

  protected override void ReloadRepositiories()
  {
    access = CreateAccess<int, int>("access");
  }
}

[SimpleTestClass]
public class DatabaseTest
{
  const int VALUE = 1;
  const int KEY = 0;
  const string DB_NAME = "test.db";

  [SimpleTestMethod]
  public void Constuctor_ShouldCreateDatabase_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));

    // When
    // Then
    Assertions.AssertTrue(File.Exists(testDirectory.GetRelativePath(DB_NAME)));
  }

  [SimpleTestMethod]
  public void Constructor_ShouldAllowWritingToDatabase_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));

    // When
    db.Add(KEY, VALUE);
    var value = db.Get(KEY);

    // Then
    Assertions.AssertTrue(db.IsOpen);
    Assertions.AssertEqual(VALUE, value);
  }

  [SimpleTestMethod]
  public void Open_ShouldAllowWritingToDatabase_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));

    // When
    db.Open();
    db.Add(KEY, VALUE);
    var value = db.Get(KEY);

    // Then
    Assertions.AssertTrue(db.IsOpen);
    Assertions.AssertEqual(VALUE, value);
  }

  [SimpleTestMethod]
  public void Close_ShouldNotAllowWritingToDatabase_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));

    // When
    db.Close();

    // Then
    Assertions.AssertFalse(db.IsOpen);
    Assertions.AssertThrows<InvalidOperationException>(() => db.Add(KEY, VALUE));
    Assertions.AssertThrows<InvalidOperationException>(() => db.Get(KEY));
  }

  [SimpleTestMethod]
  public void Open_ShouldAllowWritingToDatabase_WhenClosedBefore()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));

    // When
    db.Close();
    db.Open();
    db.Add(KEY, VALUE);
    var value = db.Get(KEY);

    // Then
    Assertions.AssertTrue(db.IsOpen);
    Assertions.AssertEqual(VALUE, value);
  }

  [SimpleTestMethod]
  public void Save_ShouldCopyDatabase_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));
    string dbSaveName = "saved.db";

    // When
    db.Add(KEY, VALUE);
    db.Save(testDirectory.GetRelativePath(dbSaveName));
    using DatabaseMock dbSaved = new(testDirectory.GetRelativePath(dbSaveName));
    var savedValue = dbSaved.Get(KEY);

    // Then
    Assertions.AssertEqual(VALUE, savedValue);
  }

  [SimpleTestMethod]
  public void Delete_ShouldDeleteDatabase_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));

    // When
    db.Delete();

    // Then
    Assertions.AssertFalse(File.Exists(testDirectory.GetRelativePath(DB_NAME)));
  }

  [SimpleTestMethod]
  public void Access_ShouldCorrectlyWriteAndRead_WhenCalledAsync()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    using DatabaseMock db = new(testDirectory.GetRelativePath(DB_NAME));

    // When
    // Then
    List<Task> tasks = new();
    foreach (var i in Enumerable.Range(1, 20))
    {
      tasks.Add(new Task(() =>
      {
        var key = -i;
        var value = i;

        db.Add(key, value);
        var getValue = db.Get(key);

        Assertions.AssertEqual(db.Get(key), getValue);
      }));
    }

    foreach (var task in tasks)
      task.Start();

    Task.WhenAll(tasks).Wait();
  }
}