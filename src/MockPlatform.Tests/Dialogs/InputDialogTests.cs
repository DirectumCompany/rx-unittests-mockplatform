extern alias ClientBase;

using ClientBase::Sungero.Core;
using CommonLibrary;
using MockPlatform.Services.Dialogs;
using Sungero.Docflow;
using Sungero.Parties;

namespace MockPlatform.Tests.Dialogs;

public class InputDialogTests
{
  [Test]
  public void AddSelect_WithDefaultString_AddsDropDownControl()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");

    // Act
    var control = dialog.AddSelect("Result", true, "Sign");

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control, Is.TypeOf<DropDownDialogValue>());
      Assert.That(control.Value, Is.EqualTo("Sign"));
      Assert.That(control.IsRequired, Is.True);
      Assert.That(dialog.Controls, Has.Count.EqualTo(1));
      Assert.That(dialog.Controls[0], Is.SameAs(control));
    });
  }

  [Test]
  public void AddSelect_WithDefaultStringAndFrom_PreservesDefaultValue()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");

    // Act
    var control = dialog
      .AddSelect("Result", true, "Reject")
      .From("Sign", "Reject");

    // Assert
    var dropDown = (DropDownDialogValue)control;
    Assert.Multiple(() =>
    {
      Assert.That(dropDown.Value, Is.EqualTo("Reject"));
      Assert.That(dropDown.AvailableValues, Is.EqualTo(new[] { "Sign", "Reject" }));
    });
  }

  [Test]
  public void AddMultilineString_AddsControlWithDefaultValue()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");

    // Act
    var control = dialog.AddMultilineString("Comment", false, "text");

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control, Is.TypeOf<MultilineStringDialogValue>());
      Assert.That(control.Value, Is.EqualTo("text"));
      Assert.That(control.IsRequired, Is.False);
      Assert.That(dialog.Controls, Has.Count.EqualTo(1));
    });
  }

  [Test]
  public void AddString_AddsControlWithDefaultValue()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");

    // Act
    var control = dialog.AddString("Result", true, "Signed");

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control, Is.TypeOf<StringDialogValue>());
      Assert.That(((StringDialogValue)control).Title, Is.EqualTo("Result"));
      Assert.That(control.Value, Is.EqualTo("Signed"));
      Assert.That(control.IsRequired, Is.True);
      Assert.That(dialog.Controls, Has.Count.EqualTo(1));
      Assert.That(dialog.Controls[0], Is.SameAs(control));
    });
  }

  [Test]
  public void AddString_WithoutDefaultValue_AddsControlWithNullValue()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");

    // Act
    var control = dialog.AddString("Name", false);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control, Is.TypeOf<StringDialogValue>());
      Assert.That(control.Value, Is.Null);
      Assert.That(control.IsRequired, Is.False);
      Assert.That(dialog.Controls, Has.Count.EqualTo(1));
    });
  }

  [Test]
  public void AddBoolean_AddsControlWithDefaultValue()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");

    // Act
    var control = dialog.AddBoolean("NeedSign", true);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control, Is.TypeOf<BooleanDialogValue>());
      Assert.That(control.Value, Is.True);
      Assert.That(dialog.Controls, Has.Count.EqualTo(1));
      Assert.That(dialog.Controls[0], Is.SameAs(control));
    });
  }

  [Test]
  public void AddNavigationSelect_AddsControlWithDefaultEntity()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");
    var counterparty = Counterparties.Create();

    // Act
    var control = dialog.AddNavigationSelect("Counterparty", true, counterparty);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.SameAs(counterparty));
      Assert.That(control.IsRequired, Is.True);
      Assert.That(dialog.Controls, Has.Count.EqualTo(1));
      Assert.That(dialog.Controls[0], Is.SameAs(control));
    });
  }

  [Test]
  public void AddSelectManyEntity_AddsControlWithDefaultEntities()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");
    var document = OfficialDocuments.Create();

    // Act
    var control = dialog.AddSelectManyEntity("Addenda", false, document);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.EqualTo(new[] { document }));
      Assert.That(control.IsRequired, Is.False);
      Assert.That(dialog.Controls, Has.Count.EqualTo(1));
    });
  }

  [Test]
  public void ExtensionAddSelect_ForEntity_UsesMockNavigationControl()
  {
    // Arrange
    IInputDialog dialog = new InputDialog("Title", "Text");
    var counterparty = Counterparties.Create();

    // Act
    var control = dialog.AddSelect("Counterparty", true, counterparty);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control, Is.TypeOf<NavigationDialogValue<ICounterparty>>());
      Assert.That(control.Value, Is.SameAs(counterparty));
      Assert.That(((InputDialog)dialog).Controls, Has.Count.EqualTo(1));
    });
  }

  [Test]
  public void ExtensionAddSelectMany_ForEntity_UsesMockSelectManyControl()
  {
    // Arrange
    IInputDialog dialog = new InputDialog("Title", "Text");
    var selected = OfficialDocuments.Create();
    var allowed = OfficialDocuments.Create();

    // Act
    var control = dialog
      .AddSelectMany("Addenda", false, selected)
      .From(selected, allowed);

    // Assert
    var selectMany = (SelectManyDialogValue<IOfficialDocument>)control;
    Assert.Multiple(() =>
    {
      Assert.That(selectMany.Value, Is.EqualTo(new[] { selected }));
      Assert.That(selectMany.AvailableValues, Is.EqualTo(new[] { selected, allowed }));
      Assert.That(((InputDialog)dialog).Controls, Has.Count.EqualTo(1));
    });
  }

  [Test]
  public void Show_SetsCurrentDialog()
  {
    // Arrange
    var dialog = new InputDialog("Title", "Text");

    // Act
    dialog.Show();

    // Assert
    Assert.That(InputDialog.Current, Is.SameAs(dialog));
  }

  [Test]
  public void Buttons_AddCustomAndCancel_WorksThroughIInputDialog()
  {
    // Arrange
    const string sendButtonName = "Send";
    const string cancelButtonName = "Cancel";
    IInputDialog dialog = new InputDialog("Title", "Text");

    // Act
    var sendButton = dialog.Buttons.AddCustom(sendButtonName);
    dialog.Buttons.Default = sendButton;
    dialog.Buttons.AddCancel();

    // Assert
    var buttons = ((InputDialog)dialog).Buttons;
    Assert.Multiple(() =>
    {
      Assert.That(sendButton, Is.TypeOf<CustomDialogButton>(),
        "AddCustom должен возвращать CustomDialogButton.");
      Assert.That(sendButton.Name, Is.EqualTo(sendButtonName),
        "Имя кнопки Send должно совпадать с переданным.");
      Assert.That(buttons.Default, Is.SameAs(sendButton),
        "Default должен быть кнопкой Send.");
      Assert.That(buttons.Items, Has.Count.EqualTo(2),
        "Должны быть кнопки Send и Cancel.");
      Assert.That(buttons.Items[0], Is.SameAs(sendButton),
        "Первой должна быть кнопка Send.");
      Assert.That(buttons.Items[1].Name, Is.EqualTo(cancelButtonName),
        "Второй должна быть кнопка Cancel.");
    });
  }
}
