using CommonLibrary;
using MockPlatform.Services.Dialogs;

namespace MockPlatform.Tests.Dialogs;

public class TaskDialogTests
{
  private const string CustomButtonName = "Sign";

  [TearDown]
  public void TearDown()
  {
    TaskDialog.Clear();
  }

  [Test]
  public void Show_WithoutHandler_ReturnsOk()
  {
    // Arrange
    var dialog = TaskDialog.Create("Text", "Description", "Title");

    // Act
    var result = dialog.Show();

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(result.Name, Is.EqualTo(TaskDialog.DefaultShowButtonName),
        "Без ShowHandler Show должен вернуть кнопку OK.");
      Assert.That(TaskDialog.Current, Is.SameAs(dialog),
        "Show должен сохранить текущий диалог в Current.");
    });
  }

  [Test]
  public void Show_WithHandler_ReturnsHandlerResult()
  {
    // Arrange
    var dialog = TaskDialog.Create("Text", "Description", "Title");
    var customButton = dialog.Buttons.AddCustom(CustomButtonName);
    TaskDialog.ShowHandler = d => d.Buttons.Items.OfType<CustomDialogButton>().First();

    // Act
    var result = dialog.Show();

    // Assert
    Assert.That(result, Is.SameAs(customButton),
      "Show должен вернуть результат ShowHandler.");
  }

  [Test]
  public void Clear_ResetsCurrentAndShowHandler()
  {
    // Arrange
    var dialog = TaskDialog.Create("Text", "Description", "Title");
    TaskDialog.ShowHandler = _ => new DialogButton(TaskDialog.DefaultShowButtonName);
    dialog.Show();

    // Act
    TaskDialog.Clear();

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(TaskDialog.Current, Is.Null, "Clear должен сбросить Current.");
      Assert.That(TaskDialog.ShowHandler, Is.Null, "Clear должен сбросить ShowHandler.");
    });
  }
}
