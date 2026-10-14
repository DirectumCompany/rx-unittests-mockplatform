using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using CommonLibrary;
using Moq;
using Sungero.Domain.Shared;

namespace MockPlatform.Extensions;

/// <summary>
/// Расширения для работы с Mock сущностей.
/// </summary>
public static class MockExtensions
{
  /// <summary>
  /// Настроить моки состояния и метаданных сущности.
  /// </summary>
  /// <remarks>
  /// Метод является основной точкой настройки мок-сущности для тестов, которым нужен доступ к
  /// <c>entity.State</c>, <c>entity.State.Properties</c>, <c>entity.Info</c> и <c>entity.Info.Properties</c>.
  /// Сначала настраивается <c>State</c>, затем <c>Info</c>.
  ///
  /// Для <c>State</c> создаётся loose-мок конкретного интерфейса состояния сущности. У него включается
  /// базовый флаг <c>IEntityState.IsEnabled</c>, после чего через конкретный тип состояния ищется
  /// свойство <c>Properties</c>. Для <c>Properties</c> создаётся отдельный мок конкретного интерфейса
  /// свойств сущности. Все свойства этого интерфейса, реализующие <c>IPropertyStateBase</c>, получают
  /// собственные моки с включёнными <c>IsEnabled</c> и <c>IsVisible</c>. Эти же объекты складываются в
  /// коллекцию перечисления, чтобы прикладной код мог обходить <c>State.Properties</c> через
  /// <c>foreach</c>.
  ///
  /// Для свойств-коллекций дополнительно настраивается вложенный <c>Properties</c> внутри самого
  /// <c>IPropertyStateBase</c>. Это нужно для сценариев вида
  /// <c>entity.State.Properties.SomeCollection.Properties</c>, где прикладной код управляет доступностью
  /// колонок коллекции. Вложенный <c>Properties</c> создаётся тем же способом, что и верхний
  /// <c>State.Properties</c>: для каждой колонки создаётся мок состояния свойства и настраивается
  /// перечисление. Перечисление настраивается и как <c>IEnumerable&lt;IPropertyStateBase&gt;</c>, и как
  /// все найденные типизированные <c>IEnumerable&lt;TPropertyState&gt;</c>, потому что сгенерированные
  /// интерфейсы платформы могут использовать типизированный enumerator.
  ///
  /// Для <c>Info</c> создаётся loose-мок конкретного интерфейса метаданных сущности с
  /// <c>DefaultValue.Mock</c>, чтобы остальные вложенные свойства метаданных создавались Moq
  /// автоматически. Отдельно настраивается <c>Info.Properties</c>: для каждого свойства, реализующего
  /// <c>IPropertyInfo</c>, создаётся мок конкретного типа и заполняется <c>IInternalPropertyInfo.NameGuid</c>.
  /// Это даёт тестам стабильные объекты метаданных свойств без ручной настройки каждого свойства сущности.
  ///
  /// Возвраты свойств настраиваются через reflection и конкретные <c>PropertyInfo</c>. Это важно для
  /// сгенерированных интерфейсов платформы, где свойства часто переобъявлены через <c>new</c> в более
  /// специфичных интерфейсах. Настройка через базовый интерфейс в таких случаях не перехватывает вызов
  /// к конкретному свойству, поэтому мок всегда привязывается к самому специфичному найденному типу.
  /// </remarks>
  /// <typeparam name="TEntity">Тип мокаемой сущности.</typeparam>
  /// <param name="mock">Мок сущности.</param>
  public static void SetupStateAndInfo<TEntity>(this Mock<TEntity> mock)
    where TEntity : class, IEntity
  {
    mock.SetupState();
    mock.SetupInfo();
  }

  /// <summary>
  /// Настроить мок State (с Properties и перечислением свойств) для сущности.
  /// Для каждого свойства типа IPropertyStateBase создаётся мок с IsEnabled = true.
  /// Для свойств-коллекций дополнительно мокируется вложенный объект Properties.
  /// </summary>
  /// <typeparam name="TEntity">Тип мокаемой сущности.</typeparam>
  /// <param name="mock">Мок сущности.</param>
  public static void SetupState<TEntity>(this Mock<TEntity> mock)
    where TEntity : class, IEntity
  {
    CreateStateMock(mock);
  }

  /// <summary>
  /// Настроить реальное отслеживание изменений сущности через <c>State.IsChanged</c>.
  /// </summary>
  /// <remarks>
  /// В отличие от точечного <c>Setup(x => x.State.IsChanged).Returns(...)</c>, здесь снимается
  /// снапшот текущих значений простых (не коллекционных) read-write свойств сущности, и
  /// <c>IsChanged</c> вычисляется сравнением текущих значений с этим снапшотом при каждом обращении.
  /// Снапшот снимается в момент вызова этого метода, поэтому вызывать его нужно после того, как
  /// сущность приведена к состоянию, которое тест считает "исходным" (например, сразу после Arrange,
  /// до вызова тестируемого метода).
  ///
  /// Отслеживаются свойства, объявленные в самом <typeparamref name="TEntity"/> и его интерфейсах
  /// (включая переобъявленные через <c>new</c> в более специфичных интерфейсах — берётся первое
  /// найденное объявление с этим именем). Коллекционные свойства (child entity collections и
  /// прочие <c>IEnumerable</c>, кроме <c>string</c>) не отслеживаются: сравнение по ссылке для них
  /// не отражает реальных изменений содержимого, а отслеживание по содержимому не требуется в
  /// текущих сценариях использования.
  /// </remarks>
  /// <typeparam name="TEntity">Тип сущности.</typeparam>
  /// <param name="mock">Мок сущности.</param>
  public static void SetupChangeTracking<TEntity>(this Mock<TEntity> mock)
    where TEntity : class, IEntity
  {
    var stateMock = CreateStateMock(mock);
    if (stateMock == null)
      return;

    var entity = mock.Object;
    var snapshot = TakeSnapshot(typeof(TEntity), entity);

    stateMock.As<IEntityState>()
      .Setup(s => s.IsChanged)
      .Returns(() => !SnapshotsEqual(snapshot, TakeSnapshot(typeof(TEntity), entity)));
  }

  /// <summary>
  /// Снять снимок значений отслеживаемых простых (не коллекционных) read-write свойств сущности.
  /// </summary>
  /// <remarks>
  /// Используется как для <see cref="SetupChangeTracking{TEntity}"/>, так и для сравнения "по содержимому"
  /// в моках контролов диалогов (см. <c>NavigationDialogValue&lt;T&gt;</c>) — там, где нужно отличить
  /// повторное присвоение того же значения от присвоения того же объекта, но с изменившимися после
  /// <c>Reload()</c> данными.
  /// </remarks>
  /// <param name="entityType">Тип сущности.</param>
  /// <param name="entity">Экземпляр сущности.</param>
  /// <returns>Снимок: свойство → его текущее значение.</returns>
  internal static Dictionary<PropertyInfo, object> TakeSnapshot(Type entityType, object entity)
  {
    return GetTrackedProperties(entityType).ToDictionary(p => p, p => p.GetValue(entity));
  }

  /// <summary>
  /// Сравнить два снимка значений свойств сущности, снятых через <see cref="TakeSnapshot"/>.
  /// </summary>
  /// <param name="previous">Ранее снятый снимок.</param>
  /// <param name="current">Текущий снимок.</param>
  /// <returns>True, если снимки совпадают по составу и значениям свойств.</returns>
  internal static bool SnapshotsEqual(Dictionary<PropertyInfo, object>? previous, Dictionary<PropertyInfo, object> current)
  {
    if (previous == null || previous.Count != current.Count)
      return false;

    foreach (var (property, value) in previous)
    {
      if (!current.TryGetValue(property, out var currentValue) || !Equals(value, currentValue))
        return false;
    }

    return true;
  }

  private static Mock CreateStateMock<TEntity>(Mock<TEntity> mock)
    where TEntity : class, IEntity
  {
    var stateProperty = typeof(TEntity).GetMostSpecificProperty("State");
    if (stateProperty == null)
      return null;

    var stateType = stateProperty.PropertyType;
    var stateMock = CreateLooseMock(stateType);
    stateMock.As<IEntityState>().SetupProperty(s => s.IsEnabled, true);

    var propertiesProperty = stateType.GetMostSpecificProperty("Properties");
    if (propertiesProperty != null)
    {
      var propertiesObj = CreatePropertyStatesMock(propertiesProperty.PropertyType);
      SetupMockReturn(stateMock, stateType, propertiesProperty, propertiesObj);
    }

    SetupMockReturn(mock, typeof(TEntity), stateProperty, stateMock.Object);
    return stateMock;
  }

  internal static IReadOnlyList<PropertyInfo> GetTrackedProperties(Type entityType)
  {
    return new[] { entityType }.Concat(entityType.GetInterfaces())
      .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
      .Where(IsTrackableProperty)
      .GroupBy(p => p.Name)
      .Select(g => g.First())
      .ToList();
  }

  private static bool IsTrackableProperty(PropertyInfo property)
  {
    if (property.Name is "State" or "Info")
      return false;
    if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length > 0)
      return false;

    return !typeof(IEnumerable).IsAssignableFrom(property.PropertyType) || property.PropertyType == typeof(string);
  }

  /// <summary>
  /// Настроить мок Info для сущности.
  /// Все вложенные свойства (Actions, Properties и т.п.) мокируются автоматически через DefaultValue.Mock.
  /// </summary>
  /// <typeparam name="TEntity">Тип мокаемой сущности.</typeparam>
  /// <param name="mock">Мок сущности.</param>
  public static void SetupInfo<TEntity>(this Mock<TEntity> mock)
    where TEntity : class, IEntity
  {
    var infoProperty = typeof(TEntity).GetMostSpecificProperty("Info");
    if (infoProperty == null)
      return;

    var infoType = infoProperty.PropertyType;
    var infoMock = CreateLooseMock(infoType);
    infoMock.DefaultValue = DefaultValue.Mock;

    var propertiesInfoProperty = infoType.GetMostSpecificProperty("Properties");
    if (propertiesInfoProperty != null)
    {
      var propertiesInfoType = propertiesInfoProperty.PropertyType;
      var propertiesInfoMock = CreateLooseMock(propertiesInfoType);
      foreach (var propInfo in GetPropertiesOfType<IPropertyInfo>(propertiesInfoType))
      {
        var propInfoMock = CreateLooseMock(propInfo.PropertyType);
        propInfoMock.As<IInternalPropertyInfo>().SetupGet(p => p.NameGuid).Returns(Guid.NewGuid());
        SetupMockReturn(propertiesInfoMock, propertiesInfoType, propInfo, propInfoMock.Object);
      }
      SetupMockReturn(infoMock, infoType, propertiesInfoProperty, propertiesInfoMock.Object);
    }

    SetupMockReturn(mock, typeof(TEntity), infoProperty, infoMock.Object);
  }

  private static Mock CreateLooseMock(Type type)
  {
    var mockType = typeof(Mock<>).MakeGenericType(type);
    return (Mock)Activator.CreateInstance(mockType, MockBehavior.Loose)!;
  }

  private static IPropertyStateBase CreatePropertyStateMock(Type propertyStateType)
  {
    var propMock = CreateLooseMock(propertyStateType);
    propMock.As<IPropertyStateBase>().SetupProperty(p => p.IsEnabled, true);
    propMock.As<IPropertyStateBase>().SetupProperty(p => p.IsVisible, true);
    propMock.As<IPropertyStateBase>().SetupProperty(p => p.IsRequired, false);

    var nestedPropertiesProperty = propertyStateType.GetMostSpecificProperty("Properties");
    if (nestedPropertiesProperty != null)
    {
      // Для property-state коллекций (например IncomingDocumentTypeProcessingActions)
      // вложенный Properties должен быть итерируемым, иначе код в foreach падает на null.
      var nestedPropertiesObj = CreatePropertyStatesMock(nestedPropertiesProperty.PropertyType);
      SetupMockReturn(propMock, propertyStateType, nestedPropertiesProperty, nestedPropertiesObj);
    }

    return (IPropertyStateBase)propMock.Object;
  }

  private static object CreatePropertyStatesMock(Type propertyStatesType)
  {
    var propertyStatesMock = CreateLooseMock(propertyStatesType);
    var propertyStateObjects = new List<IPropertyStateBase>();
    foreach (var propInfo in GetPropertiesOfType<IPropertyStateBase>(propertyStatesType))
    {
      var propertyStateObj = CreatePropertyStateMock(propInfo.PropertyType);
      SetupMockReturn(propertyStatesMock, propertyStatesType, propInfo, propertyStateObj);
      propertyStateObjects.Add(propertyStateObj);
    }

    propertyStatesMock
      .As<IEnumerable<IPropertyStateBase>>()
      .Setup(e => e.GetEnumerator())
      .Returns(() => propertyStateObjects.GetEnumerator());
    SetupTypedPropertiesEnumeration(propertyStatesMock, propertyStatesType, propertyStateObjects);

    return propertyStatesMock.Object;
  }

  private static void SetupTypedPropertiesEnumeration(Mock propertiesMock, Type propertiesType, IReadOnlyCollection<IPropertyStateBase> propertyStateObjects)
  {
    // Некоторые сгенерированные интерфейсы свойств перечисляются как IEnumerable<TSpecificPropertyState>,
    // поэтому настраиваем перечисление для всех generic-вариантов, а не только для базового IPropertyStateBase.
    var enumerableElementTypes = new[] { propertiesType }.Concat(propertiesType.GetInterfaces())
      .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
      .Select(type => type.GetGenericArguments().Single())
      .Distinct();

    var setupMethod = typeof(MockExtensions).GetMethod(nameof(SetupTypedPropertiesEnumerationCore), BindingFlags.NonPublic | BindingFlags.Static)!;
    foreach (var elementType in enumerableElementTypes)
      setupMethod.MakeGenericMethod(elementType).Invoke(null, [propertiesMock, propertyStateObjects]);
  }

  private static void SetupTypedPropertiesEnumerationCore<TPropertyState>(Mock propertiesMock, IReadOnlyCollection<IPropertyStateBase> propertyStateObjects)
  {
    var typedPropertyStateObjects = propertyStateObjects.OfType<TPropertyState>().ToList();
    propertiesMock
      .As<IEnumerable<TPropertyState>>()
      .Setup(e => e.GetEnumerator())
      .Returns(() => typedPropertyStateObjects.GetEnumerator());
  }

  /// <remarks>
  /// Reflection неизбежна: State/Properties/каждое свойство объявлены через new в конкретном
  /// интерфейсе (IXxxState, IXxxPropertyStates), поэтому настройка должна идти именно через
  /// тот PropertyInfo, который был обнаружен по специфичному типу, — настройка через базовый
  /// IEntityState/IEntityPropertyStates не перехватывает вызовы к new-члену.
  /// </remarks>
  private static void SetupMockReturn(Mock targetMock, Type mockType, PropertyInfo property, object returnValue)
  {
    var parameter = Expression.Parameter(mockType, "x");
    var lambda = Expression.Lambda(
      typeof(Func<,>).MakeGenericType(mockType, property.PropertyType),
      Expression.Property(parameter, property),
      parameter);

    var setupMethod = typeof(Mock<>)
      .MakeGenericType(mockType)
      .GetMethods(BindingFlags.Public | BindingFlags.Instance)
      .Single(m => m.Name == "Setup" && m.IsGenericMethod && m.GetGenericArguments().Length == 1);

    var setup = setupMethod.MakeGenericMethod(property.PropertyType).Invoke(targetMock, [lambda]);
    setup?.GetType().GetMethod("Returns", [property.PropertyType])?.Invoke(setup, [returnValue]);
  }

  private static IEnumerable<PropertyInfo> GetPropertiesOfType<TInterface>(Type containerType)
  {
    return new[] { containerType }.Concat(containerType.GetInterfaces())
      .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
      .Where(p => p.GetIndexParameters().Length == 0 && typeof(TInterface).IsAssignableFrom(p.PropertyType))
      .GroupBy(p => p.Name)
      .Select(g => g.First());
  }
}
