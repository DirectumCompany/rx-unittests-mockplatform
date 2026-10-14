using MockPlatform.Services.Dialogs;

namespace MockPlatform.Tests.Dialogs;

public class DropDownDialogValueTests
{
  [Test]
  public void From_WhenValueAlreadySet_DoesNotOverwriteValue()
  {
    // Arrange
    var control = new DropDownDialogValue("Result", true) { Value = "Reject" };

    // Act
    control.From("Sign", "Reject", "Clarify");

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.EqualTo("Reject"));
      Assert.That(control.AvailableValues, Is.EqualTo(new[] { "Sign", "Reject", "Clarify" }));
    });
  }

  [Test]
  public void From_WhenValueIsNull_SetsFirstAvailableValue()
  {
    // Arrange
    var control = new DropDownDialogValue("Result", true);

    // Act
    control.From("Sign", "Reject");

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.EqualTo("Sign"));
      Assert.That(control.AvailableValues, Is.EqualTo(new[] { "Sign", "Reject" }));
    });
  }

  [Test]
  public void SetOnValueChanged_WhenValueChanges_InvokesHandler()
  {
    // Arrange
    var control = new DropDownDialogValue("Result", true) { Value = "Sign" };
    string? oldValue = null;
    string? newValue = null;
    control.SetOnValueChanged(args =>
    {
      oldValue = args.OldValue;
      newValue = args.NewValue;
    });

    // Act
    control.Value = "Reject";

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(oldValue, Is.EqualTo("Sign"));
      Assert.That(newValue, Is.EqualTo("Reject"));
    });
  }

  [Test]
  public void SetOnValueChanged_WhenValueUnchanged_DoesNotInvokeHandler()
  {
    // Arrange
    var control = new DropDownDialogValue("Result", true) { Value = "Sign" };
    var called = false;
    control.SetOnValueChanged(_ => called = true);

    // Act
    control.Value = "Sign";

    // Assert
    Assert.That(called, Is.False);
  }
}
