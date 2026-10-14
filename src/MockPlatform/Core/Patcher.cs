using HarmonyLib;

namespace MockPlatform.Core;

public class Patcher
{
  private static readonly Lazy<Harmony> patcher = new(() => new Harmony("test.patch"));
  public static Harmony Instance => patcher.Value;
}
