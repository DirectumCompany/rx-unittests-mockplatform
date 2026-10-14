using MockPlatform.Repositories;
using Moq;
using Sungero.Content.Shared;
using Sungero.Core;
using Sungero.Docflow;

namespace MockPlatform.Tests;

public class History
{
  private const string ConvertToPdfOperation = Sungero.Docflow.Constants.OfficialDocument.Operation.ConvertToPdf;

  /// <summary>
  /// Замокать свойство History у документа и проверить, что прикладной код
  /// корректно вызывает History.Write.
  /// </summary>
  [Test]
  public void MockDocumentHistoryWriteCall()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var version = document.Versions.AddNew();
    version.Number = 1;

    var historyMock = new Mock<IDocumentHistories>();
    Mock.Get(document)
      .Setup(d => d.History)
      .Returns(historyMock.Object);

    var operation = new Enumeration(ConvertToPdfOperation);

    // Act
    document.History.Write(operation, operation, string.Empty, version.Number);

    // Assert
    historyMock.Verify(
      h => h.Write(
        It.Is<Enumeration?>(e => e.HasValue && e.Value.Value == ConvertToPdfOperation),
        It.Is<Enumeration?>(e => e.HasValue && e.Value.Value == ConvertToPdfOperation),
        string.Empty,
        version.Number),
      Times.Once);
  }
}
