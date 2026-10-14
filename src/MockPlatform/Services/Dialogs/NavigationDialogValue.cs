extern alias ClientBase;
using System.Linq.Expressions;
using System.Reflection;
using ClientBase::Sungero.Core;
using CommonLibrary;
using MockPlatform.Extensions;
using Sungero.Core;
using Sungero.Domain.Shared;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Тестовая реализация контрола навигационного выбора в диалоге.
/// </summary>
/// <typeparam name="T">Тип выбираемого значения.</typeparam>
public class NavigationDialogValue<T>(bool isRequired, string? title = null) : INavigationDialogValue<T>
  where T : class, IEntity
{
  private T? value;
  private Dictionary<PropertyInfo, object>? lastSnapshot;
  private Action<InputDialogValueChangedEventArgs<T>>? onValueChanged;

  public T Value
  {
    get => this.value!;
    set
    {
      if (!this.HasChanged(value))
        return;

      var oldValue = this.value;
      this.AcceptValue(value);
      this.onValueChanged?.Invoke(new InputDialogValueChangedEventArgs<T>(oldValue, value));
    }
  }

  public string? Title { get; } = title;
  public string EmptyText { get; set; }
  public bool IsEnabled { get; set; }
  public bool IsVisible { get; set; }
  public bool IsRequired { get; set; } = isRequired;
  public bool IsLabelVisible { get; set; }
  public IReadOnlyList<T> AvailableValues { get; private set; } = [];
  public string? Placeholder { get; private set; }

  public INavigationDialogValue<T> From(params T[] values)
  {
    var isInitialPopulation = this.AvailableValues.Count == 0;
    this.AvailableValues = values ?? [];
    // Не затирать Value, уже заданный в AddSelect(..., defaultValue).
    // Автовыбор первого — только при первичной инициализации (как при создании контрола),
    // чтобы последующий From после очистки Value (Null) не подставлял значение сам.
    if (isInitialPopulation && this.value == null && this.AvailableValues.Count > 0)
      this.AcceptValue(this.AvailableValues[0]);
    return this;
  }

  public INavigationDialogValue<T> From(IEnumerable<T> values)
    => this.From(values?.ToArray() ?? []);

  public INavigationDialogValue<T> Where(Expression<Func<T, bool>> predicate)
  {
    var compiled = predicate.Compile();
    this.AvailableValues = this.AvailableValues.Where(compiled).ToList();
    return this;
  }

  public INavigationDialogValue<T> WithLabel(string label) => throw new NotImplementedException();

  public INavigationDialogValue<T> WithLookupMode(LookupMode lookupMode) => throw new NotImplementedException();

  public INavigationDialogValue<T> WithPlaceholder(string text)
  {
    this.Placeholder = text;
    return this;
  }

  public void SetOnValueChanged(Action<InputDialogValueChangedEventArgs<T>> handler)
  {
    this.onValueChanged = handler;
  }

  /// <summary>
  /// Определить, отличается ли новое значение от текущего.
  /// </summary>
  /// <remarks>
  /// Если ссылка другая — значение точно изменилось. Если ссылка та же (например, сущность обновили
  /// на месте через Reload()) — сравниваем снимки отслеживаемых свойств, чтобы отличить реальное
  /// изменение данных от повторного присвоения того же неизменного значения.
  /// </remarks>
  private bool HasChanged(T newValue)
  {
    if (!ReferenceEquals(this.value, newValue))
      return true;

    if (newValue == null)
      return false;

    var newSnapshot = MockExtensions.TakeSnapshot(typeof(T), newValue);
    return !MockExtensions.SnapshotsEqual(this.lastSnapshot, newSnapshot);
  }

  /// <summary>
  /// Принять новое значение и снять его снимок для последующих сравнений.
  /// </summary>
  private void AcceptValue(T newValue)
  {
    this.value = newValue;
    this.lastSnapshot = newValue == null ? null : MockExtensions.TakeSnapshot(typeof(T), newValue);
  }
}
