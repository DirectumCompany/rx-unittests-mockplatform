using MockPlatform.Core;

namespace MockPlatform.Tests;

public class Encryption
{
  [Test]
  public void EncryptDecrypt()
  {
    // Arrange
    var dataToEncrypt = "123";

    using var encryptMock = MockFunctionStubs.Setup(typeof(Sungero.Core.Encryption), "Encrypt", (string data) => data);
    using var decryptMock = MockFunctionStubs.Setup(typeof(Sungero.Core.Encryption), "Decrypt", (string data) => data);

    // Act
    var encrypted = Sungero.Core.Encryption.Encrypt(dataToEncrypt);
    var decrypted = Sungero.Core.Encryption.Decrypt(encrypted);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(encrypted, Is.EqualTo(dataToEncrypt));
      Assert.That(decrypted, Is.EqualTo(dataToEncrypt));
    });
  }
}
