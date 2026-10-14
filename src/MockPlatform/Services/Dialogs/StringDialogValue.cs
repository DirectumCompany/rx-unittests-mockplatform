using CommonLibrary;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Тестовая реализация строкового контрола в диалоге.
/// </summary>
public class StringDialogValue(string title, bool isRequired) : IStringDialogValue
{
  private string value;
  private Action<InputDialogValueChangedEventArgs<string>> onValueChanged;

  public string Title { get; set; } = title;
  public bool IsEnabled { get; set; }
  public bool IsVisible { get; set; }
  public bool IsRequired { get; set; } = isRequired;
  public bool IsLabelVisible { get; set; }

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

  public void SetOnValueChanged(Action<InputDialogValueChangedEventArgs<string>> handler)
  {
    this.onValueChanged = handler;
  }

  public IStringDialogValue MaxLength(int maxLength) => this;

  public IStringDialogValue WithLabel(string label) => throw new NotImplementedException();

  public IStringDialogValue WithPlaceholder(string text) => this;
}
