using MockPlatform.Core;
using Moq;
using Sungero.Domain.Shared;

namespace MockPlatform.Stubs
{
  /// <summary>
  /// Общие стабы и вспомогательный код для тестов создания и настройки видов документов через
  /// Docflow.Server.ModuleInitializer.
  /// </summary>
  public static class DocumentKindStubs
  {
    public static void GetSendActionStubSetup()
    {
      MockFunctionStubs.Setup<Sungero.Docflow.Shared.ModuleFunctions>(
        nameof(Sungero.Docflow.Shared.ModuleFunctions.GetSendAction),
        (Sungero.Domain.Shared.IActionInfo action) => Mock.Of<Sungero.Docflow.IDocumentSendAction>());
    }

    /// <summary>
    /// Пересобрать ModuleInitializer.CreateDocumentKind из исходного IL, чтобы стабы на GetExternalLink/CreateExternalLink
    /// снова перехватывались, даже если JIT ранее заинлайнил их вызовы внутрь CreateDocumentKind.
    /// </summary>
    public static void RestoreCreateDocumentKindFromIL()
    {
      MockFunctionStubs.RestoreOriginalCodeFromIL<Sungero.Docflow.Server.ModuleInitializer>(
        nameof(Sungero.Docflow.Server.ModuleInitializer.CreateDocumentKind),
        typeof(string),
        typeof(string),
        typeof(Sungero.Core.Enumeration),
        typeof(Sungero.Core.Enumeration),
        typeof(bool),
        typeof(bool),
        typeof(Guid),
        typeof(Sungero.Domain.Shared.IActionInfo[]),
        typeof(bool),
        typeof(bool),
        typeof(Guid),
        typeof(bool),
        typeof(Sungero.Docflow.IMediumType));
    }
  }
}
