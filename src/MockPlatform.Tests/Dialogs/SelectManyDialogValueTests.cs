using MockPlatform.Services.Dialogs;
using Sungero.Docflow;

namespace MockPlatform.Tests.Dialogs;

public class SelectManyDialogValueTests
{
  [Test]
  public void From_DoesNotOverwriteSelectedValue()
  {
    // Arrange
    var selected = OfficialDocuments.Create();
    var other = OfficialDocuments.Create();
    var control = new SelectManyDialogValue<IOfficialDocument>(false)
    {
      Value = [selected]
    };

    // Act
    control.From(selected, other);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.EqualTo(new[] { selected }));
      Assert.That(control.AvailableValues, Is.EqualTo(new[] { selected, other }));
    });
  }

  [Test]
  public void From_WhenValueEmpty_KeepsEmptySelection()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    var control = new SelectManyDialogValue<IOfficialDocument>(false);

    // Act
    control.From(document);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(control.Value, Is.Empty);
      Assert.That(control.AvailableValues, Is.EqualTo(new[] { document }));
    });
  }
}
