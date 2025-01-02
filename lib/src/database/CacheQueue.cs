namespace Qwaitumin.GameCore;


public class CacheQueue<K, V>
  where K : notnull
{
  private readonly int cacheSize;
  private readonly Dictionary<K, LinkedListNode<KeyValuePair<K, V>>> cacheMap = new();
  private readonly LinkedList<KeyValuePair<K, V>> lruList = new();

  public CacheQueue(int cacheSize)
  {
    Assertions.AssertEqualOrMoreThan(cacheSize, 1);
    this.cacheSize = cacheSize;
  }

  public V? GetValueOrDefault(K key)
  {
    if (cacheMap.TryGetValue(key, out var node))
    {
      lruList.Remove(node);
      lruList.AddLast(node);
      return node.Value.Value;
    }
    return default;
  }

  public KeyValuePair<K, V>? Push(KeyValuePair<K, V> entry)
  {
    KeyValuePair<K, V>? overflowEntry = null;

    if (cacheMap.TryGetValue(entry.Key, out var existingNode))
    {
      lruList.Remove(existingNode);
    }
    else if (cacheMap.Count >= cacheSize)
    {
      overflowEntry = RemoveOldestEntry();
    }

    LinkedListNode<KeyValuePair<K, V>> node = new(entry);
    lruList.AddLast(node);
    cacheMap[entry.Key] = node;
    return overflowEntry;
  }

  public bool RemoveEntry(K key)
  {
    if (cacheMap.TryGetValue(key, out var existingNode))
      lruList.Remove(existingNode);
    return cacheMap.Remove(key);
  }

  public List<KeyValuePair<K, V>> Flush()
  {
    List<KeyValuePair<K, V>> flushedMemory = lruList.ToList();
    cacheMap.Clear();
    lruList.Clear();
    return flushedMemory;
  }

  private KeyValuePair<K, V>? RemoveOldestEntry()
  {
    LinkedListNode<KeyValuePair<K, V>>? firstNode = lruList.First;
    if (firstNode is null)
      return null;

    lruList.RemoveFirst();
    cacheMap.Remove(firstNode.Value.Key);
    return firstNode.Value;
  }
}