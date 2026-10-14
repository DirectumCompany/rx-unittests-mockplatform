using System.Collections.Concurrent;
using System.Reflection;
using HarmonyLib;
using Sungero.Domain.Shared;

namespace MockPlatform.Core;

public class MockFunctionStubs : IDisposable
{
  // Ключ - хэндл метода, а не имя или MethodInfo: одноимённые методы разных типов и перегрузки стабятся независимо,
  // а хэндл не зависит от ReflectedType, поэтому метод базового класса, полученный через наследника, - тот же ключ.
  private static readonly ConcurrentDictionary<RuntimeMethodHandle, MockFunctionStubs> stubs = [];
  private static readonly ConcurrentDictionary<RuntimeMethodHandle, ILRestorePatch> codeRestores = [];

  private readonly MethodInfo method;
  private readonly Delegate handler;
  private readonly MethodInfo prefix;
  private bool disposed;

  private struct InvocationResult(object? result, bool skip)
  {
    public object? Result = result;
    public bool Skip = skip;
  }

  private MockFunctionStubs(MethodInfo method, Delegate handler)
  {
    this.method = method;
    this.handler = handler;
    var prefixMethodName = (method.IsStatic, method.ReturnType.Name) switch
    {
      (false, "Void") => nameof(PrefixVoidMethod),
      (false, not "Void") => nameof(PrefixMethod),
      (true, "Void") => nameof(PrefixStaticVoidMethod),
      (true, not "Void") => nameof(PrefixStaticMethod)
    };

    prefix = AccessTools.Method(typeof(MockFunctionStubs), prefixMethodName);
  }

  private static MockFunctionStubs Register(MethodInfo method, Delegate handler)
  {
    var stub = new MockFunctionStubs(method, handler);
    if (!stubs.TryAdd(method.MethodHandle, stub))
    {
      throw new InvalidOperationException(
        $"Method '{GetMethodDisplayName(method)}' is already stubbed. " +
        "Remove the existing stub before setting up a new one: call Dispose() on the object returned by the previous " +
        $"{nameof(Setup)}/{nameof(SetupEntity)} (or declare it with 'using').");
    }

    try
    {
      Patcher.Instance.Patch(method, prefix: new HarmonyMethod(stub.prefix));
    }
    catch
    {
      stubs.TryRemove(KeyValuePair.Create(method.MethodHandle, stub));
      throw;
    }

    return stub;
  }

  public static MockFunctionStubs Setup<TClass>(string methodName, Delegate mockImplementation)
    => Setup(typeof(TClass), methodName, mockImplementation);

  public static MockFunctionStubs Setup(Type type, string methodName, Delegate mockImplementation)
  {
    var parameterTypes = GetParameterTypesFromDelegate(mockImplementation);
    var method = AccessTools.Method(type, methodName, parameterTypes) ??
      throw new MissingMethodException(type.Name, methodName);

    return Register(method, mockImplementation);
  }

  public static MockFunctionStubs SetupEntity<TClass>(string methodName, Delegate mockImplementation)
  {
    var type = typeof(TClass);
    var parameterTypes = GetParameterTypesFromDelegate(mockImplementation);
    var method = AccessTools.Method(type, methodName, parameterTypes[1..]) ??
      throw new MissingMethodException(type.Name, methodName);

    return Register(method, mockImplementation);
  }

  /// <summary>
  /// Перепатчить метод через Harmony из исходного IL (passthrough), чтобы стабы на вызываемых из него
  /// методах снова перехватывались после JIT inlining.
  /// </summary>
  public static IDisposable RestoreOriginalCodeFromIL<TClass>(string methodName, params Type[] parameterTypes)
    => RestoreOriginalCodeFromIL(typeof(TClass), methodName, parameterTypes);

  /// <summary>
  /// Перепатчить метод через Harmony из исходного IL (passthrough), чтобы стабы на вызываемых из него
  /// методах снова перехватывались после JIT inlining.
  /// </summary>
  public static IDisposable RestoreOriginalCodeFromIL(Type type, string methodName, params Type[] parameterTypes)
  {
    var method = AccessTools.Method(type, methodName, parameterTypes) ??
      throw new MissingMethodException(type.Name, methodName);

    return RestoreOriginalCodeFromIL(method);
  }

  /// <summary>
  /// Перепатчить метод через Harmony из исходного IL (passthrough), чтобы стабы на вызываемых из него
  /// методах снова перехватывались после JIT inlining.
  /// </summary>
  public static IDisposable RestoreOriginalCodeFromIL(MethodInfo method)
  {
    if (stubs.ContainsKey(method.MethodHandle))
    {
      throw new InvalidOperationException(
        $"Cannot call {nameof(RestoreOriginalCodeFromIL)} on '{GetMethodDisplayName(method)}' " +
        "because the method is already stubbed via Setup.");
    }

    if (codeRestores.TryGetValue(method.MethodHandle, out var existing))
      existing.Dispose();

    return ILRestorePatch.Apply(method);
  }

  public static void ClearAll()
  {
    var allStubs = stubs.Values.ToArray();
    foreach (var stub in allStubs)
      stub.Dispose();

    var allRestores = codeRestores.Values.ToArray();
    foreach (var restore in allRestores)
      restore.Dispose();

    codeRestores.Clear();
  }

  private static Type[] GetParameterTypesFromDelegate(Delegate @delegate) => @delegate
    .GetMethodInfo()
    .GetParameters()
    .Select(p => p.ParameterType)
    .ToArray();

  private static bool PrefixStaticMethod(MethodInfo __originalMethod, object[] __args, ref object __result)
  {
    var result = InvokeMockMethod(__originalMethod, null, __args);
    __result = result.Result!;
    return result.Skip;
  }

  private static bool PrefixStaticVoidMethod(MethodInfo __originalMethod, object[] __args)
  {
    var result = InvokeMockMethod(__originalMethod, null, __args);
    return result.Skip;
  }

  private static bool PrefixMethod(MethodInfo __originalMethod, object __instance, object[] __args, ref object __result)
  {
    var result = InvokeMockMethod(__originalMethod, __instance, __args);
    __result = result.Result!;
    return result.Skip;
  }

  private static bool PrefixVoidMethod(MethodInfo __originalMethod, object __instance, object[] __args)
  {
    var result = InvokeMockMethod(__originalMethod, __instance, __args);
    return result.Skip;
  }

  /// <summary>
  /// Prefix только для пересборки entry point из IL — оригинал всегда выполняется.
  /// </summary>
  private static bool PrefixPassthrough() => true;

  private static InvocationResult InvokeMockMethod(MethodInfo originalMethod, object? instance, object[] args)
  {
    if (!stubs.TryGetValue(originalMethod.MethodHandle, out var stub))
      return new InvocationResult(null, true);

    var mockMethod = stub.handler.GetMethodInfo();
    object[] processedArgs = instance is EntityBaseRuntimeProxy proxy ?
      [proxy.Entity, .. args] :
      [.. args];
    try
    {
      var result = mockMethod.Invoke(stub.handler.Target, processedArgs);
      return new InvocationResult(result, false);
    }
    catch (TargetInvocationException ex)
    {
      throw ex.InnerException ?? ex;
    }
  }

  public void Dispose()
  {
    if (disposed)
      return;

    disposed = true;
    stubs.TryRemove(KeyValuePair.Create(method.MethodHandle, this));
    Patcher.Instance.Unpatch(method, prefix);
  }

  private static string GetMethodDisplayName(MethodInfo method)
  {
    var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
    return $"{method.DeclaringType?.FullName}.{method.Name}({parameters})";
  }

  private sealed class ILRestorePatch : IDisposable
  {
    private readonly MethodInfo method;
    private readonly MethodInfo prefix;
    private bool disposed;

    private ILRestorePatch(MethodInfo method)
    {
      this.method = method;
      prefix = AccessTools.Method(typeof(MockFunctionStubs), nameof(PrefixPassthrough));
    }

    public static ILRestorePatch Apply(MethodInfo method)
    {
      var patch = new ILRestorePatch(method);
      codeRestores[method.MethodHandle] = patch;
      try
      {
        Patcher.Instance.Patch(method, prefix: new HarmonyMethod(patch.prefix));
      }
      catch
      {
        codeRestores.TryRemove(KeyValuePair.Create(method.MethodHandle, patch));
        throw;
      }

      return patch;
    }

    public void Dispose()
    {
      if (disposed)
        return;

      disposed = true;
      codeRestores.TryRemove(KeyValuePair.Create(method.MethodHandle, this));
      Patcher.Instance.Unpatch(method, prefix);
    }
  }
}
