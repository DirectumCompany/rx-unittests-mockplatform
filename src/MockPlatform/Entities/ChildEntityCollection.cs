using System.Collections;
using Moq;
using Sungero.Domain.Shared;

namespace MockPlatform.Entities;

public class ChildEntityCollection<T> : IChildEntityCollection<T>, IInternalChildEntityCollection
  where T : class, IChildEntity
{
  private readonly List<T> data = [];
  public int Count => data.Count;

  public required IEntity RootEntity { get; set; }

  public T AddCopy(IChildEntity sourceChildEntity) => throw new NotImplementedException();

  public void AddExistentItem(IChildEntity entity) => throw new NotImplementedException();

  public T AddNew()
  {
    var item = EntityFactory.Create<T>();
    Mock.Get(item).SetupGet(i => i.RootEntity).Returns(RootEntity);
    data.Add(item);
    ((IExtendedEntity)item).Id = data.Count;
    return item;
  }

  public IChildEntity AddNew(int index)
  {
    var item = EntityFactory.Create<T>();
    Mock.Get(item).SetupGet(i => i.RootEntity).Returns(RootEntity);
    data.Insert(index, item);
    ((IExtendedEntity)item).Id = data.Count;
    return item;
  }

  public void Clear() => data.Clear();

  public IEnumerator<T> GetEnumerator() => data.GetEnumerator();

  public void Remove(IChildEntity childEntity) => data.Remove((T)childEntity);

  IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();
}
