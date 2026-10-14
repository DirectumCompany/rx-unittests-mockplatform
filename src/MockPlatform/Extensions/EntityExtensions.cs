using Sungero.Domain;
using Sungero.Domain.Shared;
using Sungero.Domain.Shared.Validation;

namespace MockPlatform.Extensions;

public static class EntityExtensions
{
  public static T GetServerFunctions<T>(this IEntity entity)
    => (T)((IEntityFunctions)entity).FunctionsContainer.ServerFunctions;

  public static T GetSharedFunctions<T>(this IEntity entity)
    => (T)((IEntityFunctions)entity).FunctionsContainer.SharedFunctions;

  public static T GetClientFunctions<T>(this IEntity entity)
    => (T)((IEntityFunctions)entity).FunctionsContainer.ClientFunctions;

  public static void SetId(this IEntity entity, long id)
    => ((IExtendedEntity)entity).Id = id;

  public static void AddSignature(this IEntity entity, ISignature signature)
    => ((IExtendedEntity)entity).Signatures.Add((IInternalSignature)signature);

  public static List<EntityValidationMessage> GetValidationMessages(this IEntity entity)
    => ((IValidationObject)entity).ValidationResult.Messages.ToList();
}
