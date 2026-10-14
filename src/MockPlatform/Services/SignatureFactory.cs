using System.Security.Cryptography.X509Certificates;
using CommonLibrary;
using Moq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;

namespace MockPlatform.Services;

public class SignatureFactory : ISignatureFactory
{
  public void Import(ExternalSignatureInfo externalSignatureInfo)
  {
    Sign([externalSignatureInfo.Entity], externalSignatureInfo.SignatureType, null, string.Empty, externalSignatureInfo.ChildEntities, false, null);
  }

  public IEnumerable<IInternalSignature> Sign(IEnumerable<IEntity> entities, SignatureType signatureType, ICertificate certificate, string comment, IEnumerable<IChildEntity> childEntities, bool needSelectCertificate, IUser substitutedUser)
  {
    List<IInternalSignature> signatures = [];
    foreach (var entity in entities)
      signatures.Add(Sign(entity, signatureType, certificate, comment));

    foreach (var entity in childEntities)
      signatures.Add(Sign(entity, signatureType, certificate, comment));
    return signatures;
  }

  private static IInternalSignature Sign(IEntity entity, SignatureType signatureType, ICertificate certificate, string comment)
  {
    var signatureMock = new Mock<IInternalSignature>();
    signatureMock.Setup(s => s.SignatureType).Returns(signatureType);
    signatureMock.Setup(s => s.Comment).Returns(comment);
    signatureMock.Setup(s => s.Certificate).Returns(certificate);
    signatureMock.As<ISignature>();

    ((IExtendedEntity)entity).Signatures.Add(signatureMock.Object);
    return signatureMock.Object;
  }

  #region NotImplemented

  public bool EndorsingSignatureLocksSignedProperties => throw new NotImplementedException();

  public void CheckAccessRightsToSign(IEntity entity, SignatureType signatureType) => throw new NotImplementedException();

  public LocalizedString CheckUserAccessRightsToSign(IEnumerable<IEntity> entities, SignatureType signatureType, IUser user) => throw new NotImplementedException();

  public void ComputeAndStoreHashes(IEnumerable<IEntity> entities, IEnumerable<IChildEntity> childEntities, IEnumerable<string> hashAlgorithmsIds, int cacheLifetimeInMinutes) => throw new NotImplementedException();

  public void DeleteSignatures(IReadOnlyCollection<long> signatureIds) => throw new NotImplementedException();

  public void FillEntityDataHashesCache(IEntity entity, IReadOnlyCollection<IInternalSignature> signatures) => throw new NotImplementedException();

  public IEnumerable<SigningDataInfo> GetSigningDataHashes(IReadOnlyCollection<IEntity> entities, IReadOnlyCollection<SigningDataInfo> signingDataInfo) => throw new NotImplementedException();

  public void RequestSignatureImprovement(IEnumerable<long> signaturesIds, RequestImprovementSignatureFormat targetFormat) => throw new NotImplementedException();

  public void RequestSignatureImprovement(IEnumerable<ISignature> signatures, RequestImprovementSignatureFormat targetFormat) => throw new NotImplementedException();

  public IEnumerable<IInternalSignature> Save(IEnumerable<IInternalSignature> signatures, ICertificate certificate = null) => throw new NotImplementedException();

  public IEnumerable<IInternalSignature> Sign(IReadOnlyCollection<NewSignatureInfo> newSignatureInfos, ICertificate certificate, bool needSelectCertificate, IUser substitutedUser) => throw new NotImplementedException();

  public IDictionary<string, byte[]> Sign(ICertificate certificate, IDictionary<string, byte[]> signableData) => throw new NotImplementedException();

  public bool TryGetCertificate(byte[] dataSignature, out X509Certificate2 x509Certificate) => throw new NotImplementedException();

  public bool TryGetSigningDateTime(byte[] dataSignature, out DateTime signingDate) => throw new NotImplementedException();

  public void UpdateSignature(ISignature signature, Enumeration[] historyActions) => throw new NotImplementedException();

  public IEnumerable<SignatureValidationError> Verify(IInternalSignature signature) => throw new NotImplementedException();

  #endregion
}
