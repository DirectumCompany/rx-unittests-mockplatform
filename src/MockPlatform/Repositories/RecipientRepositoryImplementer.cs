using Sungero.CoreEntities;
using Sungero.CoreEntities.Shared;

namespace MockPlatform.Repositories;

public class RecipientRepositoryImplementer<T> : RepositoryImplementer<T>, IRecipientRepositoryImplementer<T>
  where T : class, IRecipient
{
  public IEnumerable<long> OwnRecipientIds => throw new NotImplementedException();

  public IEnumerable<long> AllRecipientIds => throw new NotImplementedException();

  public IEnumerable<Guid> AllRecipientSids => throw new NotImplementedException();

  public IEnumerable<long> DirectSubstitutionRecipientIds => throw new NotImplementedException();

  public IEnumerable<long> UserAndDirectSubstitutionRecipientIds => throw new NotImplementedException();

  IRecipientAccessRights IRecipientRepositoryImplementer<T>.AccessRights => throw new NotImplementedException();

  IRecipientInfo IRecipientRepositoryImplementer<T>.Info => throw new NotImplementedException();

  public IEnumerable<long> AllRecipientIdsFor(IRecipient recipient) => throw new NotImplementedException();

  public IEnumerable<Guid> AllRecipientSidsFor(IRecipient recipient) => throw new NotImplementedException();

  public IEnumerable<long> DirectSubstitutionRecipientIdsFor(IRecipient recipient) => throw new NotImplementedException();

  public IEnumerable<long> OwnRecipientIdsFor(IRecipient recipient) => recipient is null ? [] : [recipient.Id];

  public IEnumerable<long> UserAndDirectSubstitutionRecipientIdsFor(IRecipient recipient) => throw new NotImplementedException();
}
