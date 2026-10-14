using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MockPlatform.Core;
using MockPlatform.Repositories;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using Sungero.Workflow;
using Unity;

namespace MockPlatform.Extensions;

public static class UnityContainerExtensions
{
  public static IUnityContainer RegisterMockRepository(this IUnityContainer container, Type entityInterface)
  {
    var interfaceAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Sungero.Domain.Interfaces");
    var targetType = interfaceAssembly.GetTypes()
      .FirstOrDefault(t => t.FullName == $"{entityInterface.Namespace}.Shared.{entityInterface.Name?[1..]}Repository`1")?
      .MakeGenericType(entityInterface);
    if (targetType is not null)
      PatchImplementerToNull(targetType);

    var implementerType = GetRepositoryImplementerType(entityInterface);
    var implementerInterface = typeof(IRepositoryImplementer<>).MakeGenericType(entityInterface);
    // Type.GetMethod(name) неоднозначен, если в Kernel есть несколько перегрузок RegisterRepository -
    // явно выбираем generic-перегрузку с двумя типовыми параметрами, под которую написан вызов ниже.
    typeof(EntityUnityContainerExtensions)
      .GetMethods()
      .SingleOrDefault(m => m.Name == nameof(EntityUnityContainerExtensions.RegisterRepository) &&
                            m.IsGenericMethodDefinition &&
                            m.GetGenericArguments().Length == 2)?
      .MakeGenericMethod(entityInterface, implementerInterface)
      .Invoke(null, [container, implementerType.MakeGenericType(entityInterface)]);

    return container;
  }

  private static Type GetRepositoryImplementerType(Type entityInterface)
  {
    return entityInterface switch
    {
      Type t when t.IsAssignableTo(typeof(IGroup)) => typeof(GroupRepositoryImplementer<>),
      Type t when t.IsAssignableTo(typeof(IUser)) => typeof(UserRepositoryImplementer<>),
      Type t when t.IsAssignableTo(typeof(IRecipient)) => typeof(RecipientRepositoryImplementer<>),
      Type t when t.IsAssignableTo(typeof(ITask)) => typeof(TaskRepositoryImplementer<>),
      _ => typeof(RepositoryImplementer<>)
    };
  }

  private static void PatchImplementerToNull(Type? targetType)
  {
    var typeInitializer = AccessTools.GetDeclaredConstructors(targetType)
        .FirstOrDefault(c => c.IsStatic && c.GetParameters().Length == 0);

    if (typeInitializer is not null)
      Patcher.Instance.Patch(typeInitializer, transpiler: new HarmonyMethod(typeof(UnityContainerExtensions), nameof(Transpiler)));
  }

  private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    var codes = new List<CodeInstruction>(instructions);

    for (int i = 0; i < codes.Count; i++)
    {
      // Найти установку поля:
      // 1. Инструкция загрузки значения (вызов BaseImplementer)
      // 2. Инструкция каста
      // 3. Установка поля имплементера

      if (i + 2 < codes.Count &&
          codes[i + 2].opcode == OpCodes.Stsfld &&
          codes[i + 2].operand is FieldInfo field &&
          field.Name == "implementer")
      {
        // Заменить загрузку значения на null
        codes[i] = new CodeInstruction(OpCodes.Ldnull);
        // Убрать каст
        codes.RemoveAt(i + 1);
        break;
      }
    }

    return codes.AsEnumerable();
  }
}
