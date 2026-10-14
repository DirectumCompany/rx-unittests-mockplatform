using Moq;
using Sungero.Content;

namespace MockPlatform.Extensions;

public static class ElectronicDocumentExtensions
{
  /// <summary>
  /// Создать версию с заполнением LastVersion и HasVersion.
  /// </summary>
  /// <typeparam name="T">Тип электронного документа.</typeparam>
  /// <param name="document">Документ.</param>
  /// <returns>Тот же экземпляр документа.</returns>
  public static T WithVersion<T>(this T document) where T: class, IElectronicDocument
  {
    var version = document.Versions.AddNew();
    var documentMock = Mock.Get(document);
    documentMock.Setup(d => d.LastVersion).Returns(version);
    documentMock.Setup(d => d.HasVersions).Returns(true);

    return document;
  }
}
