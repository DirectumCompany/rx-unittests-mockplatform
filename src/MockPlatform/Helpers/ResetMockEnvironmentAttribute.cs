using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace MockPlatform;

/// <summary>
/// Атрибут для автоматического сброса mock-окружения перед каждым тестом.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public class ResetMockEnvironmentAttribute : Attribute, ITestAction
{
  public void BeforeTest(ITest test) => MockTestEnvironment.Reset();

  public void AfterTest(ITest test) { }

  public ActionTargets Targets => ActionTargets.Test;
}
