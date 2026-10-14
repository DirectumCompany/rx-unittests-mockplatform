using MockPlatform;

[assembly: ResetMockEnvironment]

namespace UnitTests
{
  [SetUpFixture]
  public class CompanyEnvironment
  {
    [OneTimeSetUp]
    public static void Initialize() => ModuleManager.RegisterModules("Sungero.Company", "Sungero.Commons", "Sungero.Docflow", "Sungero.Parties");
  }
}
