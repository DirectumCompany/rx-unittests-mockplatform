using CommonLibrary;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Тестовая реализация контрола логического значения в диалоге.
/// </summary>
public class BooleanDialogValue : IBooleanDialogValue
{
  private bool? value;
  private Action<InputDialogValueChangedEventArgs<bool?>>? onValueChanged;

  public bool IsEnabled { get; set; }
  public bool IsVisible { get; set; }
  public bool IsRequired { get; set; }

  public bool? Value
  {
    get => this.value;
    set
    {
      if (Equals(this.value, value))
        return;
      var oldValue = this.value;
      this.value = value;
      this.onValueChanged?.Invoke(new InputDialogValueChangedEventArgs<bool?>(oldValue, value));
    }
  }

  public void SetOnValueChanged(Action<InputDialogValueChangedEventArgs<bool?>> handler)
  {
    this.onValueChanged = handler;
  }
}
