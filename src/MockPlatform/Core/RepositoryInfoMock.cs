using System.Reflection;
using Moq;

namespace MockPlatform.Core;

/// <summary>
/// Подмена статического implementer'а репозитория-фасады (например, OfficialDocuments, Waybills) на loose Moq-мок.
/// </summary>
/// <remarks>
/// Платформенные репозитории-фасады (OfficialDocuments, Waybills и т.п.) хранят implementer в приватном
/// статическом поле собственного базового класса и используют его напрямую в статических свойствах вида
/// <c>Info => implementer.Info</c>, без null-проверки. MockPlatform обнуляет это поле в рамках патча создания
/// сущностей (иначе там бы упал InvalidCastException), поэтому любое обращение к статическому
/// <c>ХХХ.Info</c>/<c>ХХХ.AccessRights</c> (например, в выражениях вида <c>OfficialDocuments.Info.Actions.SomeAction</c>)
/// падает с NullReferenceException. Этот класс подменяет поле implementer на loose Moq-мок с рекурсивным
/// автомокингом (DefaultValue.Mock), чтобы такие обращения возвращали автомоканные объекты вместо падения.
/// </remarks>
public static class RepositoryInfoMock
{
  /// <summary>
  /// Подменить статический implementer репозитория-фасада на автомокающийся Moq-мок.
  /// </summary>
  /// <param name="repositoryFacadeType">Тип статического класса-фасада репозитория (например, typeof(OfficialDocuments)).</param>
  /// <returns>IDisposable, восстанавливающий исходное значение поля implementer при вызове Dispose.</returns>
  public static IDisposable StubInfo(Type repositoryFacadeType)
  {
    var baseType = repositoryFacadeType.BaseType ??
      throw new InvalidOperationException($"У типа {repositoryFacadeType} нет базового класса с полем implementer.");

    var implementerField = baseType.GetField("implementer", BindingFlags.Static | BindingFlags.NonPublic) ??
      throw new InvalidOperationException($"У типа {baseType} не найдено статическое поле implementer.");

    var mockType = typeof(Mock<>).MakeGenericType(implementerField.FieldType);
    var mock = (Mock)Activator.CreateInstance(mockType, MockBehavior.Loose)!;
    mock.DefaultValue = DefaultValue.Mock;

    var originalImplementer = implementerField.GetValue(null);
    implementerField.SetValue(null, mock.Object);

    return new DisposableAction(() => implementerField.SetValue(null, originalImplementer));
  }

  private sealed class DisposableAction(Action dispose) : IDisposable
  {
    public void Dispose() => dispose();
  }
}
