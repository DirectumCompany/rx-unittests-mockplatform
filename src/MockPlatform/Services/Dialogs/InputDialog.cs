using CommonLibrary;
using CommonLibrary.Dialogs;
using Sungero.Domain.Shared;

namespace MockPlatform.Services.Dialogs;

public class InputDialog(string title, string text) : IInputDialog
{
  public static InputDialog Current { get; private set; }
  public List<IDialogControl> Controls { get; } = [];

  public string Title { get; set; } = title;
  public string Text { get; set; } = text;
  public int? Height { get; set; }
  public int? Width { get; set; }

  public DialogButtonsCollection Buttons { get; } = new();

  IDialogButtonsCollection IInputDialog.Buttons => this.Buttons;

  public bool IsCanceled { get; set; }
  public string HelpCode { get; set; }

  public IBooleanDialogValue AddBoolean(string title)
    => this.AddBoolean(title, false);

  public IBooleanDialogValue AddBoolean(string title, bool defaultValue)
  {
    var control = new BooleanDialogValue { Value = defaultValue };
    Controls.Add(control);
    return control;
  }

  public IDateDialogValue AddDate(string title, bool isRequired) => throw new NotImplementedException();

  public IDateDialogValue AddDate(string title, bool isRequired, DateTime? defaultValue) => throw new NotImplementedException();

  public IDoubleDialogValue AddDouble(string title, bool isRequired) => throw new NotImplementedException();

  public IDoubleDialogValue AddDouble(string title, bool isRequired, double? defaultValue) => throw new NotImplementedException();

  public IFileSelectDialogValue AddFileSelect(string title, bool isRequired) => throw new NotImplementedException();

  public IFileSelectManyDialogValue AddFileSelectMany(string title, bool isRequired) => throw new NotImplementedException();

  public IHyperlinkDialogValue AddHyperlink(string title) => throw new NotImplementedException();

  public IIdentifierDialogValue AddIdentifier(string title, bool isRequired) => throw new NotImplementedException();

  public IIdentifierDialogValue AddIdentifier(string title, bool isRequired, long? defaultValue) => throw new NotImplementedException();

  public IIntegerDialogValue AddInteger(string title, bool isRequired) => throw new NotImplementedException();

  public IIntegerDialogValue AddInteger(string title, bool isRequired, int? defaultValue) => throw new NotImplementedException();

  public IMultilineStringDialogValue AddMultilineString(string title, bool isRequired)
    => this.AddMultilineString(title, isRequired, null);

  public IMultilineStringDialogValue AddMultilineString(string title, bool isRequired, string defaultValue)
  {
    var control = new MultilineStringDialogValue(title, isRequired) { Value = defaultValue };
    Controls.Add(control);
    return control;
  }

  public IPasswordStringDialogValue AddPasswordString(string title, bool isRequired)
    => this.AddPasswordString(title, isRequired, null);

  public IPasswordStringDialogValue AddPasswordString(string title, bool isRequired, string defaultValue)
  {
    var control = new PasswordStringDialogValue(title, isRequired) { Value = defaultValue };
    Controls.Add(control);
    return control;
  }

  public IProgressBarDialogValue AddProgressBar() => throw new NotImplementedException();

  public IDropDownDialogValue AddSelect(string title, bool isRequired)
  {
    var control = new DropDownDialogValue(title, isRequired);
    Controls.Add(control);
    return control;
  }

  public IDropDownDialogValue AddSelect(string title, bool isRequired, int defaultIndex)
  {
    var control = new DropDownDialogValue(title, isRequired) { ValueIndex = defaultIndex };
    Controls.Add(control);
    return control;
  }

  public IDropDownDialogValue AddSelect(string title, bool isRequired, string defaultValue)
  {
    var control = new DropDownDialogValue(title, isRequired) { Value = defaultValue };
    Controls.Add(control);
    return control;
  }

  /// <summary>
  /// Создать контрол навигационного выбора сущности.
  /// </summary>
  /// <typeparam name="T">Тип выбираемой сущности.</typeparam>
  /// <param name="title">Заголовок контрола.</param>
  /// <param name="isRequired">Признак обязательности.</param>
  /// <param name="defaultValue">Значение по умолчанию.</param>
  /// <returns>Контрол навигационного выбора.</returns>
  public NavigationDialogValue<T> AddNavigationSelect<T>(string title, bool isRequired, T defaultValue)
    where T : class, IEntity
  {
    var control = new NavigationDialogValue<T>(isRequired, title) { Value = defaultValue };
    Controls.Add(control);
    return control;
  }

  /// <summary>
  /// Создать контрол множественного выбора сущностей.
  /// </summary>
  /// <typeparam name="T">Тип выбираемых сущностей.</typeparam>
  /// <param name="title">Заголовок контрола.</param>
  /// <param name="isRequired">Признак обязательности.</param>
  /// <param name="defaultValues">Значения по умолчанию.</param>
  /// <returns>Контрол множественного выбора.</returns>
  public SelectManyDialogValue<T> AddSelectManyEntity<T>(string title, bool isRequired, params T[] defaultValues)
    where T : class, IEntity
  {
    var control = new SelectManyDialogValue<T>(isRequired)
    {
      Value = defaultValues ?? []
    };
    Controls.Add(control);
    return control;
  }

  public IStringDialogValue AddString(string title, bool isRequired)
    => this.AddString(title, isRequired, null);

  public IStringDialogValue AddString(string title, bool isRequired, string defaultValue)
  {
    var control = new StringDialogValue(title, isRequired) { Value = defaultValue };
    Controls.Add(control);
    return control;
  }

  private Action<InputDialogButtonClickEventArgs> onButtonClick;

  public void SetOnButtonClick(Action<InputDialogButtonClickEventArgs> handler)
  {
    this.onButtonClick = handler;
  }

  /// <summary>
  /// Смоделировать нажатие кнопки диалога для тестов — вызывает обработчик, установленный через SetOnButtonClick.
  /// </summary>
  /// <param name="button">Нажатая кнопка.</param>
  /// <param name="notAllRequiredControlsFilled">Признак того, что не все обязательные контролы заполнены.</param>
  /// <param name="validationResult">Результат валидации, в который обработчик добавляет ошибки. Если не указан, создается новый.</param>
  /// <returns>Аргументы события нажатия кнопки после обработки.</returns>
  public InputDialogButtonClickEventArgs TriggerButtonClick(DialogButton button, bool notAllRequiredControlsFilled = false,
    InputDialogValidationResult validationResult = null)
  {
    var args = new InputDialogButtonClickEventArgs(validationResult ?? new InputDialogValidationResult(), button, notAllRequiredControlsFilled);
    if (this.onButtonClick == null)
      throw new MissingMethodException($"Handler for {button.Name} not specified");

    this.onButtonClick.Invoke(args);
    return args;
  }

  public void SetOnRefresh(Action<InputDialogRefreshEventArgs> handler) => throw new NotImplementedException();

  public DialogButton Show()
  {
    Current = this;
    return DialogButtons.Ok;
  }
}
