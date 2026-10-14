using MockPlatform.Extensions;
using MockPlatform.Repositories;
using Sungero.Docflow;

namespace MockPlatform.Tests;

public class DocumentVersions
{
  [Test]
  public void WriteAndReadVersionData()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew().WithMockBody();

    var documentData = new byte[4096];
    new Random(42).NextBytes(documentData);
    var documentDataStream = new MemoryStream(documentData);
    var resultBuffer = new byte[documentData.Length];

    // Act
    version.Body.Write(documentDataStream);
    using var result = version.Body.Read();
    result.Read(resultBuffer);

    // Assert
    CollectionAssert.AreEqual(documentData, resultBuffer);
  }
}