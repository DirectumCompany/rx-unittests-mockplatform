using MockPlatform.Core;
using Moq;
using Sungero.Core;

namespace MockPlatform.Stubs;

public static class ExternalSignaturesStubs
{
  public static IDisposable SetupGetSignatureInfo()
  {
    var signatureInfo = new Mock<ISignatureInfo>();
    signatureInfo.Setup(i => i.SignatureFormat).Returns(SignatureFormat.CadesBes);

    var cadesBes = signatureInfo.As<ICadesBesSignatureInfo>();
    cadesBes.Setup(c => c.CertificateInfo.SubjectInfo).Returns(string.Empty);

    return SetupGetSignatureInfo(signatureInfo.Object);
  }

  public static IDisposable SetupGetSignatureInfo(ISignatureInfo signatureInfo)
    => MockFunctionStubs.Setup(
        typeof(ExternalSignatures),
        nameof(ExternalSignatures.GetSignatureInfo),
        (byte[] _) => signatureInfo);
}
