using MockPlatform.Services.Dialogs;

namespace MockPlatform.Tests.Dialogs;

public class StringDialogValueTests
{
  [Test]
  public void Constructor_SetsTitleAndIsRequired()
  {
    // Arrange
    // Act
    var control = new StringDialogValue("Result", true);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Title, Is.EqualTo("Result"));
      Assert.That(control.IsRequired, Is.True);
    });
  }

  [Test]
  public void SetOnValueChanged_WhenValueChanges_InvokesHandler()
  {
    // Arrange
    var control = new StringDialogValue("Result", false) { Value = "old" };
    string? oldValue = null;
    string? newValue = null;
    control.SetOnValueChanged(args =>
    {
      oldValue = args.OldValue;
      newValue = args.NewValue;
    });

    // Act
    control.Value = "new";

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(oldValue, Is.EqualTo("old"));
      Assert.That(newValue, Is.EqualTo("new"));
    });
  }

  [Test]
  public void SetOnValueChanged_WhenValueUnchanged_DoesNotInvokeHandler()
  {
    // Arrange
    var control = new StringDialogValue("Result", false) { Value = "text" };
    var called = false;
    control.SetOnValueChanged(_ => called = true);

    // Act
    control.Value = "text";

    // Assert
    Assert.That(called, Is.False);
  }
}
