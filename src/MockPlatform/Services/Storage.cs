using Sungero.Common.Server;

namespace MockPlatform.Services;

public class Storage : IStorage
{
  public Storage(Dictionary<string, object> initialData) => data = initialData;

  public Storage()
  {
  }

  public Dictionary<string, object> data = new();

  public int Count => data.Count;

  public void AddOrUpdate(string key, object value)
  {
    data[key] = value;
  }

  public void AddOrUpdate(string key, object value, TimeSpan? timeout)
  {
    data[key] = value;
  }

  public bool TryGetValue<T>(string key, out T value)
  {
    object valueFromStorage;
    if (data.TryGetValue(key, out valueFromStorage))
    {
      value = (T)valueFromStorage;
      return true;
    }

    value = default;
    return false;
  }

  #region NotImplemented

  public void AddOrUpdate(IDictionary<string, object> values)
  {
    throw new NotImplementedException();
  }

  public void AddOrUpdate(IDictionary<string, object> values, TimeSpan? timeout)
  {
    throw new NotImplementedException();
  }

  public void AddOrUpdate(IDictionary<string, object> values, TimeSpan? timeout, DateTime? updateTime)
  {
    throw new NotImplementedException();
  }

  public void Clear()
  {
    throw new NotImplementedException();
  }

  public void ClearInCurrentOrNewConnection()
  {
    throw new NotImplementedException();
  }

  public bool Contains(string key)
  {
    throw new NotImplementedException();
  }

  public IDictionary<string, T> GetAll<T>()
  {
    throw new NotImplementedException();
  }

  public IDictionary<string, T> GetAllExcept<T>(params string[] excludeKeys)
  {
    throw new NotImplementedException();
  }

  public IEnumerable<string> GetKeys(int maxCount)
  {
    throw new NotImplementedException();
  }

  public DateTime? GetLastUpdate(string key)
  {
    throw new NotImplementedException();
  }

  public IDictionary<string, DateTime?> GetLastUpdates(IEnumerable<string> keys)
  {
    throw new NotImplementedException();
  }

  public void Remove(string key)
  {
    throw new NotImplementedException();
  }

  public bool TryGetValueAndLastUpdate<T>(string key, out T value, out DateTime lastUpdate)
  {
    throw new NotImplementedException();
  }

  #endregion
}
