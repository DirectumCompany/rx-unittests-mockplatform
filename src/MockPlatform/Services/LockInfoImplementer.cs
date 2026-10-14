using CommonLibrary;
using MockPlatform.Core;
using Sungero.Domain.Shared;

namespace MockPlatform.Services;

public class LockInfoImplementer : ILockInfoImplementer, IMockable<ILockInfoImplementer>
{
  public LocalizedString GetLockedMessage(LockInfo lockInfo)
    => IMockable<ILockInfoImplementer>.Mock?.GetLockedMessage(lockInfo) ?? new LocalizedString();

  public bool IsLockedByMe(LockInfo lockInfo)
    => IMockable<ILockInfoImplementer>.Mock?.IsLockedByMe(lockInfo) ?? false;

  public string GetLoginName(long loginId)
    => IMockable<ILockInfoImplementer>.Mock?.GetLoginName(loginId) ?? string.Empty;
}
