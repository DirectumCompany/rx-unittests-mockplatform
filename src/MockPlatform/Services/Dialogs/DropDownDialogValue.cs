using CommonLibrary;

namespace MockPlatform.Services.Dialogs;

public class DropDownDialogValue(string title, bool isRequired) : IDropDownDialogValue
{
  private string value;
  private Action<InputDialogValueChangedEventArgs<string>> onValueChanged;

  public string Title { get; set; } = title;
  public int ValueIndex { get; set; }
  public bool IsEnabled { get; set; }
  public bool IsVisible { get; set; }
  public bool IsRequired { get; set; } = isRequired;
  public bool IsLabelVisible { get; set; }
  public IReadOnlyList<string> AvailableValues { get; private set; } = [];

  public string Value
  {
    get => this.value;
    set
    {
      if (Equals(this.value, value))
        return;
      var oldValue = this.value;
      this.value = value;
      this.onValueChanged?.Invoke(new InputDialogValueChangedEventArgs<string>(oldValue, value));
    }
  }

  public IDropDownDialogValue From(params string[] values)
  {
    this.AvailableValues = values ?? [];
    // Не затирать Value, уже заданный в AddSelect(..., defaultValue).
    if (this.value == null && this.AvailableValues.Count > 0)
      this.value = this.AvailableValues[0];
    return this;
  }

  public void SetOnValueChanged(Action<InputDialogValueChangedEventArgs<string>> handler)
  {
    this.onValueChanged = handler;
  }

  public IDropDownDialogValue WithLabel(string label) => throw new NotImplementedException();

  public IDropDownDialogValue WithPlaceholder(string text) => throw new NotImplementedException();
}
