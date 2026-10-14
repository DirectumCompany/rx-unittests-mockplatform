using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MockPlatform.Core;
using Moq;
using Sungero.Domain.Shared;

namespace MockPlatform.Stubs
{
  public static class RepositoryAccessRightsStubs
  {
    /// <summary>
    /// Подменить AccessRights сущности заданного типа.
    /// </summary>
    /// <remarks>
    /// Сущность обычно реализует AccessRights через несколько интерфейсов с ковариантным типом
    /// возврата (например IEntity, IRecipient, IGroup, IDatabookEntry для справочника) - на
    /// прокси-типе это отдельные методы: один - неявная реализация, остальные - явные реализации
    /// интерфейсов. Подменяем все найденные перегрузки get_AccessRights, чтобы подмена сработала
    /// независимо от статического типа переменной, через которую в коде идёт обращение к сущности.
    /// </remarks>
    public static IDisposable Setup<T>(Type repoType, Action<Mock<T>> mockInitializer)
      where T : class, IEntityAccessRights
    {
      var mock = new Mock<T>();
      mockInitializer(mock);

      // "get_AccessRights" всегда пробуем в первую очередь - для обычных репозиториев (например
      // UserBaseRepository<T>) это единственная реализация, но рефлексия по каким-то причинам
      // не находит её через GetMethods() на закрытом дженерик-типе, хотя AccessTools её находит.
      var candidateNames = new HashSet<string>(GetAccessRightsGetterNames(repoType)) { "get_AccessRights" };

      var patches = new List<IDisposable>();
      foreach (var methodName in candidateNames)
      {
        try
        {
          patches.Add(MockFunctionStubs.Setup(repoType, methodName, (Func<T>)(() => mock.Object)));
        }
        catch (MissingMethodException) when (methodName != "get_AccessRights")
        {
          // Явная реализация интерфейса обнаружена рефлексией на другом типе иерархии,
          // но на repoType её нет - пропускаем.
        }
      }

      return new CombinedDisposable(patches.ToArray());
    }

    /// <summary>
    /// Найти имена всех геттеров AccessRights, объявленных в типе или его предках.
    /// </summary>
    /// <remarks>
    /// Явные реализации интерфейсов компилируются как private-методы и не попадают в
    /// GetMethods() базового класса при запросе с типа-наследника (даже с NonPublic) -
    /// поэтому обходим иерархию типов вручную с DeclaredOnly на каждом уровне.
    /// </remarks>
    private static IEnumerable<string> GetAccessRightsGetterNames(Type type)
    {
      var names = new HashSet<string>();
      for (var current = type; current != null && current != typeof(object); current = current.BaseType)
      {
        foreach (var method in current.GetMethods(
          BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (method.Name == "get_AccessRights" || method.Name.EndsWith(".get_AccessRights"))
            names.Add(method.Name);
        }
      }

      return names;
    }

    private sealed class CombinedDisposable(IDisposable[] items) : IDisposable
    {
      public void Dispose()
      {
        foreach (var item in items)
          item.Dispose();
      }
    }
  }
}