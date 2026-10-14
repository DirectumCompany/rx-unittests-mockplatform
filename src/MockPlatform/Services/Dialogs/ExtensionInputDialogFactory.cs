using System.Reflection;
using System.Reflection.Emit;
using CommonLibrary;

namespace MockPlatform.Services.Dialogs;

/// <summary>
/// Вызов helpers InputDialog из динамической реализации IExtensionInputDialog.
/// </summary>
public static class ExtensionInputDialogInvoker
{
  /// <summary>
  /// Делегировать AddSelect / AddSelectMany в helpers mock InputDialog.
  /// </summary>
  public static object Invoke(
    string methodName,
    Type entityType,
    object dialog,
    string title,
    bool isRequired,
    object? defaultValue)
  {
    if (dialog is not InputDialog mockDialog)
      throw new NotSupportedException(
        "Entity AddSelect/AddSelectMany поддерживаются только для MockPlatform.Services.Dialogs.InputDialog.");

    var helperName = methodName == "AddSelect"
      ? nameof(InputDialog.AddNavigationSelect)
      : nameof(InputDialog.AddSelectManyEntity);

    var helper = typeof(InputDialog)
      .GetMethods(BindingFlags.Public | BindingFlags.Instance)
      .Single(m => m.Name == helperName && m.IsGenericMethodDefinition)
      .MakeGenericMethod(entityType);

    return helper.Invoke(mockDialog, [title, isRequired, defaultValue])!;
  }
}

/// <summary>
/// Фабрика runtime-типа, реализующего IExtensionInputDialog без extern alias.
/// </summary>
public static class ExtensionInputDialogFactory
{
  private static readonly Lazy<object> instance = new(CreateInstance);

  /// <summary>
  /// Получить singleton-экземпляр mock IExtensionInputDialog.
  /// </summary>
  public static object Instance => instance.Value;

  private static object CreateInstance()
  {
    var interfaceType = AccessToolsType("Sungero.Core.IExtensionInputDialog");
    var implType = BuildImplementationType(interfaceType);
    return Activator.CreateInstance(implType)!;
  }

  private static Type BuildImplementationType(Type interfaceType)
  {
    var assemblyName = new AssemblyName("MockPlatform.Dynamic.ExtensionInputDialog");
    var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
    var moduleBuilder = assemblyBuilder.DefineDynamicModule("Main");
    var typeBuilder = moduleBuilder.DefineType(
      "ExtensionInputDialogImpl",
      TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed,
      typeof(object),
      [interfaceType]);

    foreach (var interfaceMethod in interfaceType.GetMethods())
      DefineInterfaceMethod(typeBuilder, interfaceMethod);

    return typeBuilder.CreateType()
      ?? throw new InvalidOperationException("Не удалось создать тип ExtensionInputDialogImpl.");
  }

  private static void DefineInterfaceMethod(TypeBuilder typeBuilder, MethodInfo interfaceMethod)
  {
    var sourceGenericArgs = interfaceMethod.GetGenericArguments();
    var sourceParameters = interfaceMethod.GetParameters();

    var methodBuilder = typeBuilder.DefineMethod(
      interfaceMethod.Name,
      MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
      MethodAttributes.NewSlot | MethodAttributes.Virtual);

    GenericTypeParameterBuilder[] genericParams = [];
    if (sourceGenericArgs.Length > 0)
    {
      genericParams = methodBuilder.DefineGenericParameters(
        sourceGenericArgs.Select(a => a.Name).ToArray());

      for (var i = 0; i < sourceGenericArgs.Length; i++)
      {
        var source = sourceGenericArgs[i];
        genericParams[i].SetGenericParameterAttributes(source.GenericParameterAttributes);

        var constraints = source.GetGenericParameterConstraints();
        var baseConstraint = constraints.FirstOrDefault(c => c.IsClass && c != typeof(object));
        var interfaceConstraints = constraints.Where(c => c.IsInterface).ToArray();

        if (baseConstraint != null)
          genericParams[i].SetBaseTypeConstraint(baseConstraint);
        if (interfaceConstraints.Length > 0)
          genericParams[i].SetInterfaceConstraints(interfaceConstraints);
      }
    }

    Type Map(Type type)
    {
      if (type.IsGenericParameter)
        return genericParams[type.GenericParameterPosition];
      if (type.HasElementType)
      {
        var element = Map(type.GetElementType()!);
        if (type.IsArray)
          return type.GetArrayRank() == 1 ? element.MakeArrayType() : element.MakeArrayType(type.GetArrayRank());
        if (type.IsByRef)
          return element.MakeByRefType();
        if (type.IsPointer)
          return element.MakePointerType();
      }

      if (type.IsGenericType)
      {
        var args = type.GetGenericArguments().Select(Map).ToArray();
        return type.GetGenericTypeDefinition().MakeGenericType(args);
      }

      return type;
    }

    methodBuilder.SetReturnType(Map(interfaceMethod.ReturnType));
    methodBuilder.SetParameters(sourceParameters.Select(p => Map(p.ParameterType)).ToArray());

    for (var i = 0; i < sourceParameters.Length; i++)
    {
      var parameterBuilder = methodBuilder.DefineParameter(
        i + 1,
        sourceParameters[i].Attributes,
        sourceParameters[i].Name);

      if (sourceParameters[i].GetCustomAttribute<ParamArrayAttribute>() != null)
      {
        var ctor = typeof(ParamArrayAttribute).GetConstructor(Type.EmptyTypes)!;
        parameterBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, []));
      }
    }

    var invoker = typeof(ExtensionInputDialogInvoker)
      .GetMethod(nameof(ExtensionInputDialogInvoker.Invoke), BindingFlags.Public | BindingFlags.Static)!;

    var il = methodBuilder.GetILGenerator();
    il.Emit(OpCodes.Ldstr, interfaceMethod.Name);
    il.Emit(OpCodes.Ldtoken, genericParams[0]);
    il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))!);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Ldarg_2);
    il.Emit(OpCodes.Ldarg_3);
    il.Emit(OpCodes.Ldarg_S, (byte)4);
    var lastParameterType = Map(sourceParameters[3].ParameterType);
    if (lastParameterType.IsValueType)
      il.Emit(OpCodes.Box, lastParameterType);
    il.Emit(OpCodes.Call, invoker);
    il.Emit(OpCodes.Ret);

    typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethod);
  }

  private static Type AccessToolsType(string typeName)
  {
    var type = AppDomain.CurrentDomain.GetAssemblies()
      .Select(a => a.GetType(typeName, throwOnError: false))
      .FirstOrDefault(t => t != null);

    return type
      ?? throw new InvalidOperationException($"Тип {typeName} не найден.");
  }
}
