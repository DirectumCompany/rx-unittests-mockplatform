using MockPlatform.Core;
using Sungero.Common;
using Sungero.Core;

namespace MockPlatform.Stubs;

public static class EncryptionStubs
{
  public static IDisposable SetupEncryption(string encryptPrefix = "")
  {
    var encrypt = MockFunctionStubs.Setup(typeof(Encryption), "Encrypt", (string data) => encryptPrefix + data);
    var decrypt = MockFunctionStubs.Setup(typeof(Encryption), "Decrypt", (string data) => data[encryptPrefix.Length..]);
    return new DisposableObject(() =>
    {
      encrypt.Dispose();
      decrypt.Dispose();
    });
  }
}
