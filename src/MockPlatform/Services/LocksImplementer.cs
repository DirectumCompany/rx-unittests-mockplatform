using MockPlatform.Core;
using Sungero.Domain.Shared;

namespace MockPlatform.Services;

public class LocksImplementer : ILocksImplementer, IMockable<ILocksImplementer>
{
  public IReadOnlyCollection<LockInfo> GetCurrentUserLocksInfoByEntityType(Guid typeId)
    => IMockable<ILocksImplementer>.Mock?.GetCurrentUserLocksInfoByEntityType(typeId) ?? [];

  public LockInfo GetLockInfo(IEntity entity)
    => IMockable<ILocksImplementer>.Mock?.GetLockInfo(entity) ?? LockInfoFactory.Empty;

  public LockInfo GetLockInfo(IBinaryData body)
    => IMockable<ILocksImplementer>.Mock?.GetLockInfo(body) ?? LockInfoFactory.Empty;

  public void Lock(IEntity entity, bool isPermanent)
    => IMockable<ILocksImplementer>.Mock?.Lock(entity, isPermanent);

  public void Lock(IEntity entity, bool isPermanent, bool isExplicit)
    => IMockable<ILocksImplementer>.Mock?.Lock(entity, isPermanent, isExplicit);

  public void Lock(EntityIdentifier entityIdentifier, bool isPermanent)
    => IMockable<ILocksImplementer>.Mock?.Lock(entityIdentifier, isPermanent);

  public void Lock(EntityIdentifier entityIdentifier, bool isPermanent, bool isExplicit)
    => IMockable<ILocksImplementer>.Mock?.Lock(entityIdentifier, isPermanent, isExplicit);

  public void Lock(IBinaryData body)
    => IMockable<ILocksImplementer>.Mock?.Lock(body);

  public void Lock(IBinaryData body, bool isExplicit)
    => IMockable<ILocksImplementer>.Mock?.Lock(body, isExplicit);

  public bool TryLock(IEntity entity, bool isPermanent)
    => IMockable<ILocksImplementer>.Mock?.TryLock(entity, isPermanent) ?? true;

  public bool TryLock(IEntity entity, bool isPermanent, bool isExplicit)
    => IMockable<ILocksImplementer>.Mock?.TryLock(entity, isPermanent, isExplicit) ?? true;

  public bool TryLock(IBinaryData body)
    => IMockable<ILocksImplementer>.Mock?.TryLock(body) ?? true;

  public bool TryLock(IBinaryData body, bool isExplicit)
    => IMockable<ILocksImplementer>.Mock?.TryLock(body, isExplicit) ?? true;

  public void Unlock(IEntity entity)
    => IMockable<ILocksImplementer>.Mock?.Unlock(entity);

  public void Unlock(EntityIdentifier entityIdentifier)
    => IMockable<ILocksImplementer>.Mock?.Unlock(entityIdentifier);

  public void Unlock(IBinaryData body)
    => IMockable<ILocksImplementer>.Mock?.Unlock(body);
}
