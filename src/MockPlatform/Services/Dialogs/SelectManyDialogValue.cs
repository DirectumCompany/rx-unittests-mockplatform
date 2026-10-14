extern alias ClientBase;
using ClientBase::Sungero.Core;
using CommonLibrary;
using Sungero.Domain.Shared;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Тестовая реализация контрола множественного выбора в диалоге.
/// </summary>
/// <typeparam name="T">Тип выбираемых значений.</typeparam>
public class SelectManyDialogValue<T>(bool isRequired) : ISelectManyDialogValue<T>
  where T : class, IEntity
{
  public IEnumerable<T> Value { get; set; } = [];
  public bool IsEnabled { get; set; }
  public bool IsVisible { get; set; }
  public bool IsRequired { get; set; } = isRequired;
  public bool IsLabelVisible { get; set; }
  public IReadOnlyList<T> AvailableValues { get; private set; } = [];

  public ISelectManyDialogValue<T> From(params T[] values)
  {
    this.AvailableValues = values ?? [];
    // Value не трогать — задан в AddSelectMany(..., defaultValues).
    return this;
  }

  public ISelectManyDialogValue<T> From(IEnumerable<T> values)
    => this.From(values?.ToArray() ?? []);

  public ISelectManyDialogValue<T> Where(Func<T, bool> predicate) => throw new NotImplementedException();

  public ISelectManyDialogValue<T> WithLabel(string label) => throw new NotImplementedException();

  public ISelectManyDialogValue<T> WithPlaceholder(string text) => throw new NotImplementedException();

  public void SetOnValueChanged(Action<InputDialogValueChangedEventArgs<IEnumerable<T>>> handler) =>
    throw new NotImplementedException();
}
