using System;
using CommonLibrary;
using MockPlatform.Core;
using Sungero.Domain.Shared;

namespace MockPlatform.Services;

public class LockInfoImplementer : ILockInfoImplementer, IMockable<ILockInfoImplementer>
{
  public LocalizedString GetLockedMessage(LockInfo lockInfo)
    => IMockable<ILockInfoImplementer>.Mock?.GetLockedMessage(lockInfo) ?? new LocalizedString();

  public bool LockedByMe(long loginId)
    => IMockable<ILockInfoImplementer>.Mock?.LockedByMe(loginId) ?? false;

  public bool LockedByMe(Guid sessionId, string complexTraceId)
    => IMockable<ILockInfoImplementer>.Mock?.LockedByMe(sessionId, complexTraceId) ?? false;

  public bool LockedByOther(long loginId)
    => IMockable<ILockInfoImplementer>.Mock?.LockedByOther(loginId) ?? false;

  public bool LockedByOther(Guid sessionId, string complexTraceId)
    => IMockable<ILockInfoImplementer>.Mock?.LockedByOther(sessionId, complexTraceId) ?? false;
}
