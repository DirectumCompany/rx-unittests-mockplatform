using HarmonyLib;
using MockPlatform.Core;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Harmony-патч get_ExtensionInputDialogService: возвращает runtime-реализацию
/// IExtensionInputDialog без DI и без extern alias.
/// </summary>
public static class ExtensionInputDialogServicePatch
{
  private static bool applied;

  /// <summary>
  /// Применить патч один раз на время жизни процесса.
  /// </summary>
  public static void Apply()
  {
    if (applied)
      return;

    var extensionType = AccessTools.TypeByName("Sungero.Core.ExtensionInputDialog")
      ?? throw new InvalidOperationException("Тип Sungero.Core.ExtensionInputDialog не найден.");

    var getter = AccessTools.PropertyGetter(extensionType, "ExtensionInputDialogService")
      ?? throw new MissingMethodException(extensionType.FullName, "get_ExtensionInputDialogService");

    var prefix = new HarmonyMethod(AccessTools.Method(typeof(ExtensionInputDialogServicePatch), nameof(Prefix)));
    Patcher.Instance.Patch(getter, prefix: prefix);
    applied = true;
  }

  private static bool Prefix(ref object __result)
  {
    __result = ExtensionInputDialogFactory.Instance;
    return false;
  }
}
