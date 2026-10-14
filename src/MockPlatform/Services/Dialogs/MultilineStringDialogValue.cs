using CommonLibrary;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Тестовая реализация контрола многострочного текста в диалоге.
/// </summary>
public class MultilineStringDialogValue(string title, bool isRequired) : IMultilineStringDialogValue
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

  public IMultilineStringDialogValue MaxLength(int maxLength) => this;

  public IMultilineStringDialogValue WithLabel(string label) => throw new NotImplementedException();

  public IMultilineStringDialogValue RowsCount(int rowsCount) => throw new NotImplementedException();

  public IMultilineStringDialogValue WithRowsCount(int rowsCount) => throw new NotImplementedException();

  public IMultilineStringDialogValue WithPlaceholder(string text) => this;
}
