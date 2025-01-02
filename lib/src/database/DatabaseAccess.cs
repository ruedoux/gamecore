using LiteDB;

namespace Qwaitumin.GameCore;

class DatabaseEntry<V>
  where V : notnull
{
  [BsonId] public BsonValue Key { get; set; } = BsonValue.Null;
  public V? Value { get; set; } = default!;

  public DatabaseEntry() { }

  public DatabaseEntry(BsonValue key, V value)
  {
    Key = key;
    Value = value;
  }
}

public interface IDatabaseAccess<in K, V>
  where K : notnull
  where V : notnull
{
  public bool AddEntry(K key, V value);
  public V? GetEntry(K key);
  public bool RemoveEntry(K key);
}

public class DatabaseAccess<K, V> : IDatabaseAccess<K, V>
  where K : notnull
  where V : notnull
{
  private readonly LiteDatabase liteDatabase;
  private readonly string collectionName;

  public DatabaseAccess(LiteDatabase liteDatabase, string collectionName)
  {
    this.liteDatabase = liteDatabase;
    this.collectionName = collectionName;
  }

  public bool AddEntry(K key, V value)
  {
    BsonValue bsonKey = BsonMapper.Global.Serialize(key);
    var collection = liteDatabase.GetCollection<DatabaseEntry<V>>(collectionName);
    return collection.Upsert(new DatabaseEntry<V>(bsonKey, value));
  }

  public V? GetEntry(K key)
  {
    BsonValue bsonKey = BsonMapper.Global.Serialize(key);
    var collection = liteDatabase.GetCollection<DatabaseEntry<V>>(collectionName);
    var entry = collection.FindById(bsonKey);
    return entry == null ? default : entry.Value;
  }

  public bool RemoveEntry(K key)
  {
    BsonValue bsonKey = BsonMapper.Global.Serialize(key);
    var collection = liteDatabase.GetCollection<DatabaseEntry<V>>(collectionName);
    return collection.Delete(bsonKey);
  }

  public KeyValuePair<K, V?>[] GetAllEntries()
  {
    var collection = liteDatabase.GetCollection<DatabaseEntry<V>>(collectionName);
    return collection.FindAll().Select(
      entry => new KeyValuePair<K, V?>(BsonMapper.Global.Deserialize<K>(entry.Key), entry.Value)).ToArray();
  }

  public K[] GetAllKeys()
  {
    var collection = liteDatabase.GetCollection<DatabaseEntry<V>>(collectionName);
    return collection.FindAll().Select(
      entry => BsonMapper.Global.Deserialize<K>(entry.Key)).ToArray();
  }
}

public class CachedDatabaseAccess<K, V> : IDatabaseAccess<K, V>
  where K : notnull
  where V : notnull
{
  private readonly DatabaseAccess<K, V> databaseAccess;
  private readonly CacheQueue<K, V> cache;

  public CachedDatabaseAccess(LiteDatabase liteDatabase, string collectionName, int cacheSize)
  {
    cache = new(cacheSize);
    databaseAccess = new(liteDatabase, collectionName);
  }

  public bool AddEntry(K key, V value)
  {
    var cacheOverflow = cache.Push(new(key, value));
    if (cacheOverflow is not null)
      return databaseAccess.AddEntry(cacheOverflow.Value.Key, cacheOverflow.Value.Value);
    return true;
  }

  public V? GetEntry(K key)
  {
    var cacheEntry = cache.GetValueOrDefault(key);
    if (cacheEntry is not null)
      return cacheEntry;

    var databaseEntry = databaseAccess.GetEntry(key);
    if (databaseEntry is not null)
      AddEntry(key, databaseEntry);

    return databaseEntry;
  }

  public bool RemoveEntry(K key)
    => cache.RemoveEntry(key) && databaseAccess.RemoveEntry(key);

  public void FlushCache()
  {
    foreach (var pair in cache.Flush())
      databaseAccess.AddEntry(pair.Key, pair.Value);
  }
}