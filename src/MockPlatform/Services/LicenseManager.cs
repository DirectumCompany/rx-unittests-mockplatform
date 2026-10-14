using Sungero.Domain.Shared;
using Sungero.Domain.Shared.Exceptions;
using Sungero.Licensing;
using Sungero.Security;

namespace MockPlatform.Services;

public class LicenseManager : ILicenseManager
{
  public LicenseException BaseCheck() => null;

  public bool IsModuleAvailableByLicense(Guid moduleId) => true;

  #region NotImplemented

  public bool IsLicenseExists => throw new NotImplementedException();

  public bool IsLicenseExistsAndLicenseSignatureValid => throw new NotImplementedException();

  public LicensingType? LicensingType => throw new NotImplementedException();

  public bool IsRestrictedMode => throw new NotImplementedException();

  public bool IsInDeploymentMode => throw new NotImplementedException();

  public bool CanAddUser(bool isSystem) => throw new NotImplementedException();

  public bool CanClientRegister() => throw new NotImplementedException();

  public void FullCheck() => throw new NotImplementedException();

  public void FullCheckForCurrentTenant() => throw new NotImplementedException();

  public IEnumerable<LicenseException> FullCheckForLicenseKey(LicenseKey license) => throw new NotImplementedException();

  public AuthorizationDecision GetAuthorizationDecisionWithCheckLicense(IEnumerable<AuthorizationDecision> originalAuthorizationDecisions, int operationSet, IEnumerable<ISecureObject> secureObjects, Authentication authentication) => throw new NotImplementedException();

  public int GetConnectedNonSystemUsersCount(IReadOnlyCollection<long> excludedLoginIds) => throw new NotImplementedException();

  public IEnumerable<LicenseException> GetLicenseValidationExceptions() => throw new NotImplementedException();

  public string GetLicenseValidationMessageByModule(Guid moduleId, LicenseKey license) => throw new NotImplementedException();

  public OperationMask GetOperationMaskWithCheckLicense(OperationMask originalOperationMask, IEnumerable<ISecureObject> secureObjects, Authentication authentication) => throw new NotImplementedException();

  public IQueryable<T> GetQueryWithExcludedUnavailableTypes<T>(Type[] typeList, IQueryable<T> initialQuery) => throw new NotImplementedException();

  public bool IsEntityAvailableByLicense(Guid entityTypeId) => throw new NotImplementedException();

  public bool IsLicenseExpired(LicenseKey license) => throw new NotImplementedException();

  public bool IsMaxUsersCountExceeded(bool isSystem) => throw new NotImplementedException();

  public bool IsModuleAvailable(IEnumerable<ISecureObject> secureObjects) => throw new NotImplementedException();

  public bool IsModuleValidByLicense(Guid moduleId) => throw new NotImplementedException();

  public bool IsModuleValidForCurrentUserByLicense(Guid moduleId) => throw new NotImplementedException();

  public bool IsOperationSetAllowedByLicense(int operationSet, IEnumerable<ISecureObject> secureObjects, Authentication authentication) => throw new NotImplementedException();

  public bool IsOperationSetAllowedByLicense(int operationSet, Guid entityType, Authentication authentication) => throw new NotImplementedException();

  public bool NeedDeleteClient() => throw new NotImplementedException();

  public bool TryGetModulesWithNotAllowedOperationSet(int operationSet, Guid entityType, Authentication authentication, out IEnumerable<ModuleInfo> securableModules) => throw new NotImplementedException();

  #endregion
}