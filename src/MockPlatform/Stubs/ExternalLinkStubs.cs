using MockPlatform.Core;
using Moq;
using Sungero.Domain.Shared;

namespace MockPlatform.Stubs
{
  public static class ExternalLinkStubs
  {
    /// <summary>
    /// Застабить GetExternalLink. Если entity не передан - link считается не существующим для любого ИД
    /// (например, для проверки создания сущности при инициализации). Если передан - link существует
    /// только для initializationGuid, для остальных ИД тоже не существует.
    /// </summary>
    public static IDisposable SetupGetExternalLink(Guid initializationGuid = default, IEntity entity = null)
    {
      var externalLink = entity == null ? null : Mock.Of<IExternalLink>(l => l.EntityId == entity.Id);

      return MockFunctionStubs.Setup<Sungero.Docflow.Server.ModuleFunctions>(
        nameof(Sungero.Docflow.Server.ModuleFunctions.GetExternalLink),
        (Guid entityType, Guid entityId) => entityId == initializationGuid ? externalLink : null);
    }

    /// <summary>
    /// Застабить CreateExternalLink. Если передан словарь - в него по ИД инициализации запоминается
    /// созданная сущность, иначе стаб работает как no-op.
    /// </summary>
    public static IDisposable SetupCreateExternalLink<TEntity>(Dictionary<Guid, TEntity> createdEntities = null)
      where TEntity : class, IEntity
    {
      return MockFunctionStubs.Setup<Sungero.Docflow.Server.ModuleFunctions>(
        nameof(Sungero.Docflow.Server.ModuleFunctions.CreateExternalLink),
        (IEntity entity, Guid entityId) =>
        {
          if (createdEntities != null)
            createdEntities[entityId] = (TEntity)entity;
        });
    }
  }
}