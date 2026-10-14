using CommonLibrary;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Коллекция кнопок диалога для mock-окружения.
/// </summary>
public class DialogButtonsCollection : IDialogButtonsCollection
{
  /// <summary>
  /// Добавленные кнопки в порядке создания.
  /// </summary>
  public List<DialogButton> Items { get; } = [];

  /// <inheritdoc />
  public DialogButton Default { get; set; }

  /// <inheritdoc />
  public CustomDialogButton AddCustom(string name)
  {
    var button = new CustomDialogButton(name);
    Items.Add(button);
    return button;
  }

  /// <inheritdoc />
  public DialogButton AddOk() => this.AddNamedButton("OK");

  /// <inheritdoc />
  public DialogButton AddCancel() => this.AddNamedButton("Cancel");

  /// <inheritdoc />
  public DialogButton AddYes() => this.AddNamedButton("Yes");

  /// <inheritdoc />
  public DialogButton AddYesToAll() => this.AddNamedButton("YesToAll");

  /// <inheritdoc />
  public DialogButton AddNo() => this.AddNamedButton("No");

  /// <inheritdoc />
  public DialogButton AddRetry() => this.AddNamedButton("Retry");

  /// <inheritdoc />
  public DialogButton AddAbort() => this.AddNamedButton("Abort");

  /// <inheritdoc />
  public DialogButton AddIgnore() => this.AddNamedButton("Ignore");

  /// <inheritdoc />
  public void AddYesNo()
  {
    this.AddYes();
    this.AddNo();
  }

  /// <inheritdoc />
  public void AddOkCancel()
  {
    this.AddOk();
    this.AddCancel();
  }

  /// <inheritdoc />
  public void AddRetryAbortIgnore()
  {
    this.AddRetry();
    this.AddAbort();
    this.AddIgnore();
  }

  private DialogButton AddNamedButton(string name)
  {
    var button = new DialogButton(name);
    Items.Add(button);
    return button;
  }
}
