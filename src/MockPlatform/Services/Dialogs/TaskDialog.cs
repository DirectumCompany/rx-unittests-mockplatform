extern alias ClientBase;
using CommonLibrary;
using ClientBase::Sungero.Core;

namespace MockPlatform.Services.Dialogs;

public class TaskDialog(string text, string description, MessageType type, string title) : ITaskDialog
{
  public static TaskDialog? Current { get; private set; }

  /// <summary>
  /// Обработчик результата Show. Если задан — возвращает его результат вместо DialogButtons.Ok.
  /// </summary>
  public static Func<TaskDialog, DialogButton>? ShowHandler { get; set; }

  /// <summary>
  /// Имя кнопки, которую возвращает Show без ShowHandler.
  /// </summary>
  public static string DefaultShowButtonName => DialogButtons.Ok.Name;

  /// <summary>
  /// Создать TaskDialog с типом Question без зависимости вызывающего кода от MessageType.
  /// </summary>
  public static TaskDialog Create(string text, string description = "", string title = "")
    => new(text, description, MessageType.Question, title);

  public string Text { get; } = text;
  public string Description { get; } = description;
  public MessageType Type { get; } = type;
  public string Title { get; } = title;

  public DialogButtonsCollection Buttons { get; } = new();

  IDialogButtonsCollection ITaskDialog.Buttons => this.Buttons;

  public IHyperlinkDialogValue AddHyperlink(string title) => throw new NotImplementedException();

  public DialogButton Show()
  {
    Current = this;
    return ShowHandler?.Invoke(this) ?? DialogButtons.Ok;
  }

  public static void Clear()
  {
    Current = null;
    ShowHandler = null;
  }
}
