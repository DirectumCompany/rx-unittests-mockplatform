using MockPlatform;
using MockPlatform.Core;

[assembly: ResetMockEnvironment]

namespace MockPlatform.Tests;

[SetUpFixture]
public class Environment
{
  [OneTimeSetUp]
  public void OneTimeSetUp()
  {
    ModuleManager.RegisterModules("Sungero.Company", "Sungero.Parties", "Sungero.Commons", "Sungero.Exchange", "Sungero.ExchangeCore", "Sungero.Docflow", "Sungero.DocflowApproval");
  }

  [OneTimeTearDown]
  public void OneTimeTearDown() => Patcher.Instance.UnpatchAll();
}
