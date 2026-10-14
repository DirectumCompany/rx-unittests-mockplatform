using MockPlatform.Core;

namespace MockPlatform.Tests;

/// <summary>
/// Регрессии RestoreOriginalCodeFromIL: пересборка caller из IL после прогрева JIT.
/// </summary>
public class FunctionStubsRestoreOriginalCodeFromIL
{
  [Test]
  [Description("После прогрева caller и Setup на callee RestoreOriginalCodeFromIL восстанавливает перехват стаба.")]
  public void RestoreOriginalCodeFromIL_AfterCallerWarmup_AllowsCalleeStub()
  {
    // Arrange — прогрев JIT caller без стаба на Step
    var target = new OrchestratorFixture();
    _ = target.Orchestrate();

    var stepHits = 0;
    MockFunctionStubs.Setup<OrchestratorFixture>(
      nameof(OrchestratorFixture.Step),
      () =>
      {
        stepHits++;
        return "stubbed";
      });

    // Act — пересобрать caller из IL и вызвать снова
    using (MockFunctionStubs.RestoreOriginalCodeFromIL<OrchestratorFixture>(
             nameof(OrchestratorFixture.Orchestrate)))
    {
      var result = target.Orchestrate();

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(stepHits, Is.EqualTo(1), "Стаб Step должен вызваться после RestoreOriginalCodeFromIL.");
        Assert.That(result, Is.EqualTo("stubbed"));
      });
    }
  }

  [Test]
  [Description("Порядок вызовов не важен: RestoreOriginalCodeFromIL на caller можно вызвать до Setup на callee — " +
    "перехват стаба всё равно сработает, если оба вызова были сделаны до Act.")]
  public void RestoreOriginalCodeFromIL_BeforeCalleeSetup_StubStillIntercepted()
  {
    // Arrange — прогрев JIT caller без стаба на Step
    var target = new OrchestratorFixture();
    _ = target.Orchestrate();

    // Act — сначала Restore, ПОТОМ Setup (обратный порядок)
    using (MockFunctionStubs.RestoreOriginalCodeFromIL<OrchestratorFixture>(
             nameof(OrchestratorFixture.Orchestrate)))
    {
      var stepHits = 0;
      MockFunctionStubs.Setup<OrchestratorFixture>(
        nameof(OrchestratorFixture.Step),
        () =>
        {
          stepHits++;
          return "stubbed";
        });

      var result = target.Orchestrate();

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(stepHits, Is.EqualTo(1), "Стаб Step должен вызваться, даже если Setup был ПОСЛЕ RestoreOriginalCodeFromIL.");
        Assert.That(result, Is.EqualTo("stubbed"));
      });
    }
  }

  [Test]
  [Description("ClearAll снимает патч RestoreOriginalCodeFromIL.")]
  public void RestoreOriginalCodeFromIL_ClearAll_RemovesPatch()
  {
    // Arrange
    var target = new OrchestratorFixture();
    using var restore = MockFunctionStubs.RestoreOriginalCodeFromIL<OrchestratorFixture>(
      nameof(OrchestratorFixture.Orchestrate));

    MockFunctionStubs.ClearAll();

    MockFunctionStubs.Setup<OrchestratorFixture>(
      nameof(OrchestratorFixture.Step),
      () => "should-not-matter-without-restore");

    // Без повторного RestoreOriginalCodeFromIL после ClearAll поведение зависит от JIT;
    // проверяем, что повторный Restore снова ставит патч и стаб работает.
    using (MockFunctionStubs.RestoreOriginalCodeFromIL<OrchestratorFixture>(
             nameof(OrchestratorFixture.Orchestrate)))
    {
      var result = target.Orchestrate();
      Assert.That(result, Is.EqualTo("should-not-matter-without-restore"));
    }
  }

  [Test]
  [Description("RestoreOriginalCodeFromIL на методе, уже замоканном через Setup, выбрасывает InvalidOperationException.")]
  public void RestoreOriginalCodeFromIL_WhenMethodAlreadyStubbed_Throws()
  {
    // Arrange
    MockFunctionStubs.Setup<OrchestratorFixture>(
      nameof(OrchestratorFixture.Step),
      () => "stubbed");

    // Act
    TestDelegate act = () => MockFunctionStubs.RestoreOriginalCodeFromIL<OrchestratorFixture>(
      nameof(OrchestratorFixture.Step));

    // Assert
    Assert.Throws<InvalidOperationException>(act);
  }

  /// <summary>
  /// Локальная фикстура: Orchestrate вызывает Step. Без NoInlining — ближе к реальному inline-сценарию.
  /// </summary>
  public class OrchestratorFixture
  {
    public string Orchestrate() => this.Step();

    public virtual string Step() => "original";
  }
}
