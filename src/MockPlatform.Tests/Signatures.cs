using MockPlatform.Repositories;
using Sungero.Core;
using Sungero.Docflow;
using Sungero.Domain.Shared;

namespace MockPlatform.Tests;

public class Signatures
{
  [Test]
  public void Acquaint()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew();

    // Act
    Sungero.Core.Signatures.Acquaint(version, string.Empty);
    var signature = (IInternalSignature)Sungero.Core.Signatures.Get(version).First();

    // Assert
    Assert.That(signature.SignatureType, Is.EqualTo(SignatureType.Acquaint));
  }

  [Test]
  public void Approve()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew();

    // Act
    Sungero.Core.Signatures.Approve(version, string.Empty);
    var signature = (IInternalSignature)Sungero.Core.Signatures.Get(version).First();

    // Assert
    Assert.That(signature.SignatureType, Is.EqualTo(SignatureType.Approval));
  }

  [Test]
  public void Endorse()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew();

    // Act
    Sungero.Core.Signatures.Endorse(version, string.Empty);
    var signature = (IInternalSignature)Sungero.Core.Signatures.Get(version).First();

    // Assert
    Assert.That(signature.SignatureType, Is.EqualTo(SignatureType.Endorsing));
  }

  [Test]
  public void Import()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew();

    // Act
    Sungero.Core.Signatures.Import(document, SignatureType.Acquaint, "123", [], version);
    var documentSignature = (IInternalSignature)Sungero.Core.Signatures.Get(version).First();
    var versionSignature = (IInternalSignature)Sungero.Core.Signatures.Get(version).First();

    // Assert
    Assert.That(documentSignature.SignatureType, Is.EqualTo(SignatureType.Acquaint));
    Assert.That(versionSignature.SignatureType, Is.EqualTo(SignatureType.Acquaint));
  }

  [Test]
  public void NotEndorse()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew();

    // Act
    Sungero.Core.Signatures.NotEndorse(version, string.Empty);
    var signature = (IInternalSignature)Sungero.Core.Signatures.Get(version).First();

    // Assert
    Assert.That(signature.SignatureType, Is.EqualTo(SignatureType.NotEndorsing));
  }

  [Test]
  public void PersonalSign()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew();

    // Act
    Sungero.Core.Signatures.PersonalSign(version, string.Empty);
    var signature = (IInternalSignature)Sungero.Core.Signatures.Get(version).First();

    // Assert
    Assert.That(signature.SignatureType, Is.EqualTo(SignatureType.PersonalSign));
  }
}
