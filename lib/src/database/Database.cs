using LiteDB;

namespace Qwaitumin.GameCore;

public abstract class Database : IDisposable
{
  public readonly string FilePath;
  public bool IsOpen { protected set; get; } = false;
  public bool IsDisposed { private set; get; } = false;
  private LiteDatabase liteDatabase;
  private readonly ReaderWriterLockSlim _lock = new();
  private readonly object _disposeLock = new();

  protected Database(string filePath)
  {
    FilePath = filePath;
    liteDatabase = new(FilePath);
    Open();
  }

  public DatabaseAccess<K, V> CreateAccess<K, V>(string tableName)
    where K : notnull
    where V : notnull
    => new(liteDatabase, tableName);

  public CachedDatabaseAccess<K, V> CreateAccess<K, V>(string tableName, int cacheSize)
    where K : notnull
    where V : notnull
    => new(liteDatabase, tableName, cacheSize);

  public T PerformRead<T>(Func<T> func)
  {
    ThrowIfDisposed();
    ThrowIfClosed();

    return LockRead(() => func());
  }

  public void PerformWrite(Action action)
  {
    ThrowIfDisposed();
    ThrowIfClosed();

    LockWrite(() => action());
  }

  public void Open()
  {
    ThrowIfDisposed();
    if (IsOpen)
      return;

    LockWrite(() =>
    {
      liteDatabase = new(FilePath);
      IsOpen = true;
    });
    ReloadRepositiories();
  }

  public void Close()
  {
    ThrowIfDisposed();
    LockWrite(() =>
    {
      liteDatabase.Dispose();
      IsOpen = false;
    });
  }

  public void Save(string path)
  {
    bool wasOpen = IsOpen;
    Close();
    File.Copy(FilePath, path, true);
    if (wasOpen)
      Open();
  }

  public void Delete()
  {
    Close();
    File.Delete(FilePath);
    Dispose();
  }

  public void Dispose()
  {
    lock (_disposeLock)
    {
      if (IsDisposed)
        return;

      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }

  protected abstract void ReloadRepositiories();

  protected virtual void Dispose(bool disposing)
  {
    Close();
    IsDisposed = true;
    _lock.Dispose();
  }

  protected void LockWrite(Action action)
  {
    _lock.EnterWriteLock();
    try
    {
      action();
    }
    finally
    {
      _lock.ExitWriteLock();
    }
  }

  protected T LockRead<T>(Func<T> action)
  {
    _lock.EnterReadLock();
    try
    {
      return action();
    }
    finally
    {
      _lock.ExitReadLock();
    }
  }

  protected void ThrowIfDisposed()
  {
    if (IsDisposed)
      throw new ObjectDisposedException($"Object has been disposed, path: {FilePath}");
  }

  protected void ThrowIfClosed()
  {
    if (!IsOpen)
      throw new InvalidOperationException($"Object has been closed, path: {FilePath}");
  }

  ~Database()
  {
    Dispose(false);
  }
}