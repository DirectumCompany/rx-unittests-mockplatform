using System.Linq;
using System.Reflection;
using CommonLibrary;
using MockPlatform.Core;
using Moq;
using Sungero.CoreEntities;
using Sungero.Domain;
using Sungero.Domain.Shared;
using Sungero.Domain.Shared.Validation;
using Sungero.Workflow;

namespace MockPlatform.Entities;

public static class EntityFactory
{
  public static T Create<T>()
    where T : class, IEntity
  {
    var entityMock = new Mock<T>();
    entityMock.As<IInternalEntity>();
    var entityFunctions = entityMock.As<IEntityFunctions>();
    SetupInternalAssignmentBase(entityMock);
    SetupInternalTask(entityMock);
    entityMock.SetupAllProperties();
    SetupExtendedEntity(entityMock);
    SetupValidationObject(entityMock);
    entityFunctions.SetupGet(e => e.FunctionsContainer).Returns(new EntityFunctionsContainer(entityMock.Object));
    SetupCollectionProperties(entityMock);
    SetupToString(entityMock);
    MockInstanceInitializer.Initialize(entityMock);
    return entityMock.Object;
  }

  private static void SetupValidationObject<T>(Mock<T> mock)
    where T : class, IEntity
  {
    var validationObject = mock.As<IValidationObject>();
    var validationResult = new ValidationResult(mock.Object);
    validationObject.SetupGet(v => v.ValidationResult).Returns(validationResult);
    validationObject.SetupGet(v => v.RequiredProperties).Returns([]);
    validationObject.SetupGet(v => v.PropertiesVisibleForValidation).Returns([]);
    validationObject.SetupGet(v => v.Validators).Returns([]);
  }

  private static void SetupInternalAssignmentBase<T>(Mock<T> mock)
    where T : class, IEntity
  {
    if (!typeof(IAssignment).IsAssignableFrom(typeof(T)))
      return;

    var internalAssignment = mock.As<IInternalAssignmentBase>();
    internalAssignment.SetupProperty(a => a.Task);
    internalAssignment.SetupProperty(a => a.BlockUid);
    internalAssignment.SetupProperty(a => a.MainTask);
    internalAssignment.SetupProperty(a => a.TaskStartId);
    internalAssignment.SetupProperty(a => a.IterationId);
    internalAssignment.SetupProperty(a => a.AttachmentGroupsSettings);
    internalAssignment.SetupGet(a => a.BlockId).Returns((int?)null);
    internalAssignment.SetupGet(a => a.Subtasks).Returns(Enumerable.Empty<ITask>());
  }

  private static void SetupInternalTask<T>(Mock<T> mock)
    where T : class, IEntity
  {
    if (!typeof(ITask).IsAssignableFrom(typeof(T)))
      return;

    var internalTask = mock.As<IInternalTask>();
    internalTask.SetupProperty(t => t.MainTask);
    internalTask.SetupProperty(t => t.ParentTask);
    internalTask.SetupProperty(t => t.ParentAssignment);
    internalTask.SetupProperty(t => t.StartId);
    internalTask.SetupProperty(t => t.ParentStartId);
    internalTask.SetupProperty(t => t.IterationId);
    internalTask.SetupProperty(t => t.BlockUid);
    internalTask.SetupProperty(t => t.AttachmentGroupsSettings);
    internalTask.SetupProperty(t => t.CreatedFrom);
    internalTask.SetupGet(t => t.BlockId).Returns((int?)null);
    internalTask.SetupGet(t => t.Subtasks).Returns(Enumerable.Empty<ITask>());
    internalTask.SetupGet(t => t.Assignments).Returns(Enumerable.Empty<IAssignmentBase>());
    internalTask.SetupGet(t => t.HasSubtasks).Returns(false);
    internalTask.SetupGet(t => t.HasAssignments).Returns(false);
    internalTask.SetupGet(t => t.IsAwaited).Returns(false);
    internalTask.SetupGet(t => t.UsersWhoCanCreate).Returns(Enumerable.Empty<IUser>());
  }

  private static void SetupExtendedEntity<T>(Mock<T> mock)
    where T : class, IEntity
  {
    var extendedEntity = mock.As<IExtendedEntity>();
    extendedEntity.SetupGet(e => e.TypeGuid).Returns(GetTypeGuid<T>());

    var parameters = new Dictionary<string, object>();
    extendedEntity.SetupGet(e => e.Params).Returns(parameters);

    var signatures = new List<IInternalSignature>();
    extendedEntity.SetupGet(d => d.Signatures).Returns(signatures);
  }

  /// <summary>
  /// Получить GUID типа сущности. Type.GetTypeGuid() не резолвит дочерние (child collection row) сущности
  /// (возвращает Guid.Empty), поэтому сначала используется поиск метадаты по типу через MetadataSearcher,
  /// который корректно резолвит и корневые, и дочерние сущности; GetTypeGuid() остаётся запасным вариантом.
  /// </summary>
  private static Guid GetTypeGuid<T>()
    where T : class, IEntity
  {
    var metadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(typeof(T));
    return metadata?.NameGuid ?? typeof(T).GetTypeGuid();
  }

  private static void SetupCollectionProperties<T>(Mock<T> mock)
    where T : class, IEntity
  {
    Type[] interfaces = [typeof(T), .. typeof(T).GetInterfaces()];
    var collectionProperties = interfaces
        .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        .Where(p => p.CanRead && IsChildEntityCollectionType(p.PropertyType))
        .GroupBy(p => p.Name)
        .Select(g => g.First());

    foreach (var property in collectionProperties)
    {
      // 1. Создаем MockChildEntityCollection<T> для типа элемента коллекции
      var elementType = property.PropertyType.GetGenericArguments()[0];
      var mockCollectionType = typeof(ChildEntityCollection<>).MakeGenericType(elementType);
      var mockCollection = (IInternalChildEntityCollection)Activator.CreateInstance(mockCollectionType)!;
      mockCollection.RootEntity = mock.Object;

      // 2. Создаем lambda-выражение: x => x.Property
      var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
      var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
      var funcType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
      var lambda = System.Linq.Expressions.Expression.Lambda(funcType, propertyAccess, parameter);

      // 3. Вызываем mock.Setup<TProperty>(lambda)
      var setupMethod = mock.GetType()
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .FirstOrDefault(m => m.Name == "Setup" && m.IsGenericMethod && m.GetGenericArguments().Length == 1);

      if (setupMethod == null)
        continue;

      var setup = setupMethod.MakeGenericMethod(property.PropertyType).Invoke(mock, [lambda]);

      // 4. Вызываем setup.Returns(mockCollection)
      var returnsMethod = setup?.GetType().GetMethod("Returns", [property.PropertyType]);
      returnsMethod?.Invoke(setup, [mockCollection]);
    }
  }

  private static void SetupToString<T>(Mock<T> mock) where T : class, IEntity
  {
    var nameProperty = typeof(T).GetMostSpecificProperty("Name");
    if (nameProperty == null || !nameProperty.CanRead)
      return;

    var mockObject = mock.Object;

    mock.Setup(x => x.ToString()).Returns(() => nameProperty.GetValue(mockObject)?.ToString() ?? string.Empty);
  }

  private static bool IsChildEntityCollectionType(Type propertyType)
  {
    if (!propertyType.IsGenericType)
      return false;

    var genericTypeDefinition = propertyType.GetGenericTypeDefinition();
    return genericTypeDefinition.GetInterfaces()
      .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Name.Contains("IChildEntityCollection")) ||
      genericTypeDefinition.Name.Contains("IChildEntityCollection");
  }
}
