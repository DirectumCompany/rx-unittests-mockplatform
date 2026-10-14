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
        typeof(Sungero.Docflow.Structures.Module.IDocumentKindModel),
        typeof(Sungero.Domain.Shared.IActionInfo[]));
    }

    /// <summary>
    /// Пересобрать ModuleInitializer.SetDocumentKindDefaultNameFormat из исходного IL, чтобы стаб на GetExternalLink
    /// снова перехватывался, даже если JIT ранее заинлайнил его вызов внутрь SetDocumentKindDefaultNameFormat.
    /// </summary>
    public static void RestoreSetDocumentKindDefaultNameFormatFromIL()
    {
      MockFunctionStubs.RestoreOriginalCodeFromIL<Sungero.Docflow.Server.ModuleInitializer>(
        nameof(Sungero.Docflow.Server.ModuleInitializer.SetDocumentKindDefaultNameFormat),
        typeof(Guid),
        typeof(string));
    }
  }
}
