using System.Collections.Concurrent;
using Sungero.Core;
using Sungero.Domain.Shared;
using Sungero.Workflow;
using Sungero.Workflow.Blocks;

namespace MockPlatform.Services.Workflow;

/// <summary>
/// In-memory реализация сервиса пользовательских свойств блоков для unit-тестов.
/// </summary>
public sealed class MockCustomTypePropertyService : ICustomTypePropertyService
{
  private readonly ConcurrentDictionary<BlockBase, BlockState> blockStates = new();

  public void SetCustomProperty(BlockBase block, string name, object value)
    => GetState(block).Properties[name] = value;

  public void SetCustomNavigationProperty(BlockBase block, string name, IEntity value)
    => GetState(block).NavigationProperties[name] = value;

  public void SetCustomNavigationCollectionProperty(BlockBase block, string name, IEnumerable<IEntity> value)
    => GetState(block).NavigationCollections[name] = value?.ToList();

  public TProp GetCustomProperty<TProp>(BlockBase block, string name)
    => GetState(block).Properties.TryGetValue(name, out var value) && value is TProp typedValue
      ? typedValue
      : default!;

  public TProp GetCustomNavigationProperty<TProp>(BlockBase block, string name)
    where TProp : IEntity
    => GetState(block).NavigationProperties.TryGetValue(name, out var value) && value is TProp typedValue
      ? typedValue
      : default!;

  public object GetCustomNavigationProperty(BlockBase block, string name, Type type)
    => GetState(block).NavigationProperties.TryGetValue(name, out var value) ? value : null!;

  public IEnumerable<TProp> GetCustomNavigationCollectionPropertyValue<TProp>(BlockBase block, string name)
    where TProp : IEntity
    => GetOrCreateNavigationCollection(block, name)?.OfType<TProp>();

  public IEnumerable<object> GetCustomNavigationCollectionPropertyValue(BlockBase block, string name, Type type)
    => GetOrCreateNavigationCollection(block, name).Where(type.IsInstanceOfType);

  public IEnumerable<IEntity> GetCustomNavigationCollectionPropertyValue(CustomTypeProperty property, Type type)
    => property.Value is IEnumerable<IEntity> entities
      ? entities.Where(type.IsInstanceOfType)
      : [];

  public Enumeration? GetCustomEnumProperty(BlockBase block, string name)
  {
    object value;
    GetState(block).Properties.TryGetValue(name, out value);
    return value != null ? new Enumeration(value.ToString()) : null;
  }

  public bool IsCustomPropertyExist(BlockBase block, string name)
    => GetState(block).Properties.ContainsKey(name);

  private BlockState GetState(BlockBase block) => blockStates.GetOrAdd(block, _ => new BlockState());

  private List<IEntity> GetOrCreateNavigationCollection(BlockBase block, string name)
  {
    var state = GetState(block);
    if (!state.NavigationCollections.TryGetValue(name, out var collection))
    {
      collection = [];
      state.NavigationCollections[name] = collection;
    }
    return collection;
  }

  private sealed class BlockState
  {
    public Dictionary<string, object> Properties { get; } = [];

    public Dictionary<string, IEntity> NavigationProperties { get; } = [];

    public Dictionary<string, List<IEntity>> NavigationCollections { get; } = [];
  }
}
