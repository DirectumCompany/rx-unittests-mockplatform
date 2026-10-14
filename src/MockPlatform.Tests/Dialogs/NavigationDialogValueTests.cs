using MockPlatform.Services.Dialogs;
using Sungero.Parties;

namespace MockPlatform.Tests.Dialogs;

public class NavigationDialogValueTests
{
  [Test]
  public void From_WhenValueAlreadySet_DoesNotOverwriteValue()
  {
    // Arrange
    var first = Counterparties.Create();
    var second = Counterparties.Create();
    var control = new NavigationDialogValue<ICounterparty>(true) { Value = second };

    // Act
    control.From(first, second);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.SameAs(second));
      Assert.That(control.AvailableValues, Is.EqualTo(new[] { first, second }));
    });
  }

  [Test]
  public void From_WhenValueIsNull_SetsFirstAvailableValue()
  {
    // Arrange
    var first = Counterparties.Create();
    var second = Counterparties.Create();
    var control = new NavigationDialogValue<ICounterparty>(false);

    // Act
    control.From(first, second);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.SameAs(first));
      Assert.That(control.AvailableValues, Is.EqualTo(new[] { first, second }));
    });
  }

  [Test]
  public void From_SubsequentCallWithNullValue_DoesNotAutoSelect()
  {
    // Arrange
    var first = Counterparties.Create();
    var second = Counterparties.Create();
    var control = new NavigationDialogValue<ICounterparty>(false);
    control.From(first);
    control.Value = null!;

    // Act
    control.From(second);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.Null);
      Assert.That(control.AvailableValues, Is.EqualTo(new[] { second }));
    });
  }

  [Test]
  public void SetOnValueChanged_WhenValueChanges_InvokesHandler()
  {
    // Arrange
    var oldCounterparty = Counterparties.Create();
    var newCounterparty = Counterparties.Create();
    var control = new NavigationDialogValue<ICounterparty>(true) { Value = oldCounterparty };
    ICounterparty? capturedOld = null;
    ICounterparty? capturedNew = null;
    control.SetOnValueChanged(args =>
    {
      capturedOld = args.OldValue;
      capturedNew = args.NewValue;
    });

    // Act
    control.Value = newCounterparty;

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(capturedOld, Is.SameAs(oldCounterparty));
      Assert.That(capturedNew, Is.SameAs(newCounterparty));
    });
  }

  [Test]
  public void SetOnValueChanged_WhenValueUnchanged_DoesNotInvokeHandler()
  {
    // Arrange
    var counterparty = Counterparties.Create();
    var control = new NavigationDialogValue<ICounterparty>(true) { Value = counterparty };
    var called = false;
    control.SetOnValueChanged(_ => called = true);

    // Act
    control.Value = counterparty;

    // Assert
    Assert.That(called, Is.False);
  }

  [Test]
  public void From_DoesNotInvokeValueChangedHandler()
  {
    // Arrange
    var first = Counterparties.Create();
    var second = Counterparties.Create();
    var control = new NavigationDialogValue<ICounterparty>(false);
    var called = false;
    control.SetOnValueChanged(_ => called = true);

    // Act
    control.From(first, second);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(called, Is.False);
      Assert.That(control.Value, Is.SameAs(first));
    });
  }

  [Test]
  public void Where_FiltersAvailableValues()
  {
    // Arrange
    var first = Counterparties.Create();
    first.Name = "Keep";
    var second = Counterparties.Create();
    second.Name = "Skip";
    var control = new NavigationDialogValue<ICounterparty>(false);
    control.From(first, second);

    // Act
    control.Where(x => x.Name == "Keep");

    // Assert
    Assert.That(control.AvailableValues, Is.EqualTo(new[] { first }));
  }
}
