using CommonLibrary;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Контрол ввода пароля в диалоге.
/// </summary>
public class PasswordStringDialogValue(string title, bool isRequired) : IPasswordStringDialogValue
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

  public IPasswordStringDialogValue MaxLength(int maxLength) => this;

  public IPasswordStringDialogValue WithLabel(string label) => throw new NotImplementedException();

  public IPasswordStringDialogValue WithPlaceholder(string text) => this;
}
