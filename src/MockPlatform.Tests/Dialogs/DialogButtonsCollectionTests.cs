using CommonLibrary;
using MockPlatform.Services.Dialogs;

namespace MockPlatform.Tests.Dialogs;

public class DialogButtonsCollectionTests
{
  private const string SendButtonName = "Send";
  private const string CancelButtonName = "Cancel";

  [Test]
  public void AddCustom_AddsCustomButtonWithName()
  {
    // Arrange
    var buttons = new DialogButtonsCollection();

    // Act
    var button = buttons.AddCustom(SendButtonName);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(button, Is.TypeOf<CustomDialogButton>(),
        "AddCustom должен возвращать CustomDialogButton.");
      Assert.That(button.Name, Is.EqualTo(SendButtonName),
        "Имя кнопки должно совпадать с переданным.");
      Assert.That(buttons.Items, Has.Count.EqualTo(1),
        "Кнопка должна быть добавлена в Items.");
      Assert.That(buttons.Items[0], Is.SameAs(button),
        "В Items должна быть та же кнопка.");
    });
  }

  [Test]
  public void AddCancel_AddsDialogButton()
  {
    // Arrange
    var buttons = new DialogButtonsCollection();

    // Act
    var button = buttons.AddCancel();

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(button, Is.TypeOf<DialogButton>(),
        "AddCancel должен возвращать DialogButton.");
      Assert.That(button.Name, Is.EqualTo(CancelButtonName),
        "Имя кнопки Cancel должно быть Cancel.");
      Assert.That(buttons.Items, Has.Count.EqualTo(1),
        "Кнопка должна быть добавлена в Items.");
      Assert.That(buttons.Items[0], Is.SameAs(button),
        "В Items должна быть та же кнопка.");
    });
  }

  [Test]
  public void Default_CanBeAssignedToCustomButton()
  {
    // Arrange
    var buttons = new DialogButtonsCollection();
    var sendButton = buttons.AddCustom(SendButtonName);

    // Act
    buttons.Default = sendButton;

    // Assert
    Assert.That(buttons.Default, Is.SameAs(sendButton),
      "Default должен ссылаться на назначенную кнопку.");
  }

  [Test]
  public void AddCustomAndAddCancel_PreservesOrder()
  {
    // Arrange
    var buttons = new DialogButtonsCollection();

    // Act
    var sendButton = buttons.AddCustom(SendButtonName);
    var cancelButton = buttons.AddCancel();

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(buttons.Items, Has.Count.EqualTo(2),
        "Должны быть две кнопки.");
      Assert.That(buttons.Items[0], Is.SameAs(sendButton),
        "Первой должна быть кнопка Send.");
      Assert.That(buttons.Items[1], Is.SameAs(cancelButton),
        "Второй должна быть кнопка Cancel.");
    });
  }
}
