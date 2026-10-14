using Moq;
using Sungero.Content;
using BinaryData = MockPlatform.Entities.BinaryData;

namespace MockPlatform.Extensions;

public static class DocumentVersionsExtensions
{
  public static IElectronicDocumentVersions WithMockBody(this IElectronicDocumentVersions version)
  {
    var versionMock = Mock.Get(version);
    var binaryData = new BinaryData();
    versionMock.SetupGet(v => v.Body).Returns(binaryData);
    return version;
  }
}
