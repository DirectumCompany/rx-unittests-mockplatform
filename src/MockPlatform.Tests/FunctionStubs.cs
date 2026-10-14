using System.Runtime.CompilerServices;
using HarmonyLib;
using MockPlatform.Core;
using MockPlatform.Extensions;
using MockPlatform.Repositories;
using Sungero.CoreEntities;
using Sungero.ExchangeCore;
using Sungero.Parties;

namespace MockPlatform.Tests;

public class FunctionStubs
{
  [Test]
  public void StubModuleFunction()
  {
    // Arrange
    var userBase = Sungero.Company.UserBases.Create();
    using var mock = MockFunctionStubs.Setup<Sungero.Company.Shared.ModuleFunctions>(
      "SplitToIds",
      (string idsString, string[] separators) => new List<long>());

    // Act
    var result = new Sungero.Company.Server.ModuleFunctions().GetUserBasesByIds("1");

    // Assert
    Assert.That(result, Has.Count.EqualTo(0));
  }

  [Test]
  public void StubModuleVoidFunction()
  {
    // Arrange
    var substituted = Sungero.Company.UserBases.Create();
    var substitute = Sungero.Company.UserBases.Create();
    using var mock = MockFunctionStubs.Setup<Sungero.Company.Server.ModuleFunctions>(
      "CreateSystemSubstitution",
      (IUser substitutedUser, IUser substitute) => { });

    // Act
    new Sungero.Company.Server.ModuleFunctions().CreateSystemSubstitutions([substituted], substitute);

    // Assert
    Assert.That(Substitutions.GetAll().Count(), Is.EqualTo(0));

  }

  [Test]
  public void StubEntityFunction()
  {
    // Arrange
    var counterparty = Counterparties.Create();
    var businessUnitBox = BusinessUnitBoxes.Create();

    using var mock = MockFunctionStubs.SetupEntity<Sungero.ExchangeCore.Shared.BusinessUnitBoxFunctions>(
      "CheckBusinessUnitBoxActive",
      (IBusinessUnitBox _obj) => "123");

    // Act
    var result = businessUnitBox
      .GetServerFunctions<Sungero.ExchangeCore.Server.BusinessUnitBoxFunctions>()
      .UpdateExchange(counterparty, string.Empty, string.Empty, string.Empty, null);

    // Assert
    Assert.That(result, Is.EqualTo("123"));
  }

  [Test]
  public void StubEntityVoidFunction()
  {
    // Arrange
    var businessUnitBox = BusinessUnitBoxes.Create();

    using var mock = MockFunctionStubs.SetupEntity<Sungero.ExchangeCore.Server.BusinessUnitBoxFunctions>(
      "CheckExchangeConnectorLicense",
      (IBusinessUnitBox _obj) => { throw new Exception(); });

    // Act
    // Assert
    var functions = businessUnitBox.GetServerFunctions<Sungero.ExchangeCore.Server.BusinessUnitBoxFunctions>();
    Assert.Throws<Exception>(() => functions.GetClient());
  }

  [Test]
  public void SecondSetupOfSameMethodThrows()
  {
    // Arrange
    using var mock = MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 1);

    // Act
    // Assert
    var ex = Assert.Throws<InvalidOperationException>(() =>
      MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 2));
    Assert.That(ex!.Message, Does.Contain($"{typeof(FirstStubTarget).FullName}.{nameof(FirstStubTarget.Get)}(Int32)"));
    Assert.That(ex.Message, Does.Contain("Dispose()"));
    Assert.That(FirstStubTarget.Get(0), Is.EqualTo(1));
  }

  [Test]
  public void SetupAfterDisposeReplacesStub()
  {
    // Arrange
    var first = MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 1);
    first.Dispose();

    // Act
    using var second = MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 2);

    // Assert
    Assert.That(FirstStubTarget.Get(0), Is.EqualTo(2));
  }

  [Test]
  public void RepeatedDisposeDoesNotRemoveNewStub()
  {
    // Arrange
    var first = MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 1);
    first.Dispose();
    using var second = MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 2);

    // Act
    first.Dispose();

    // Assert
    Assert.That(FirstStubTarget.Get(0), Is.EqualTo(2));
  }

  [Test]
  public void SameNameMethodsOfDifferentTypesAreStubbedIndependently()
  {
    // Arrange
    using var firstMock = MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 1);
    using var secondMock = MockFunctionStubs.Setup(typeof(SecondStubTarget), nameof(SecondStubTarget.Get), (int value) => 2);

    // Act
    // Assert
    Assert.That(FirstStubTarget.Get(0), Is.EqualTo(1));
    Assert.That(SecondStubTarget.Get(0), Is.EqualTo(2));
  }

  [Test]
  public void ClearAllUnpatchesSameNameMethods()
  {
    // Arrange
    MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 1);
    MockFunctionStubs.Setup(typeof(SecondStubTarget), nameof(SecondStubTarget.Get), (int value) => 2);

    // Act
    MockFunctionStubs.ClearAll();

    // Assert
    Assert.That(GetPrefixCount(typeof(FirstStubTarget)), Is.Zero);
    Assert.That(GetPrefixCount(typeof(SecondStubTarget)), Is.Zero);
    Assert.That(FirstStubTarget.Get(10), Is.EqualTo(10));
    Assert.That(SecondStubTarget.Get(20), Is.EqualTo(20));
  }

  [Test]
  public void FailedPatchDoesNotLeaveStubRegistered()
  {
    // Arrange
    // Harmony не патчит абстрактный метод и бросает ArgumentException.
    Assert.That(
      () => MockFunctionStubs.Setup(typeof(AbstractStubTarget), nameof(AbstractStubTarget.Get), (int value) => 1),
      Throws.TypeOf<ArgumentException>());

    // Act
    // Assert
    // Если бы после неудачного патча стаб остался зарегистрированным, здесь было бы InvalidOperationException "already stubbed".
    Assert.That(
      () => MockFunctionStubs.Setup(typeof(AbstractStubTarget), nameof(AbstractStubTarget.Get), (int value) => 1),
      Throws.TypeOf<ArgumentException>());
  }

  [Test]
  public void FailedRestoreOriginalCodeFromILDoesNotBreakClearAll()
  {
    // Arrange
    var methodThroughDerived = typeof(DerivedRestoreTarget).GetMethod(nameof(BaseRestoreTarget.Get))!;
    // Harmony не патчит метод, полученный через наследника, и бросает ArgumentException.
    Assert.That(() => MockFunctionStubs.RestoreOriginalCodeFromIL(methodThroughDerived), Throws.TypeOf<ArgumentException>());

    // Act
    // Assert
    Assert.That(() => MockFunctionStubs.ClearAll(), Throws.Nothing);
  }

  [Test]
  public void RestoreOriginalCodeFromILOnStubbedMethodThrows()
  {
    // Arrange
    using var mock = MockFunctionStubs.Setup(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), (int value) => 1);

    // Act
    // Assert
    var ex = Assert.Throws<InvalidOperationException>(() =>
      MockFunctionStubs.RestoreOriginalCodeFromIL(typeof(FirstStubTarget), nameof(FirstStubTarget.Get), typeof(int)));
    Assert.That(ex!.Message, Does.Contain($"{typeof(FirstStubTarget).FullName}.{nameof(FirstStubTarget.Get)}(Int32)"));
  }

  private static int GetPrefixCount(Type type) =>
    Harmony.GetPatchInfo(AccessTools.Method(type, nameof(FirstStubTarget.Get)))?.Prefixes.Count ?? 0;

  private static class FirstStubTarget
  {
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int Get(int value) => value;
  }

  private static class SecondStubTarget
  {
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int Get(int value) => value;
  }

  private abstract class AbstractStubTarget
  {
    public abstract int Get(int value);
  }

  private class BaseRestoreTarget
  {
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int Get(int value) => value;
  }

  private class DerivedRestoreTarget : BaseRestoreTarget
  {
  }
}