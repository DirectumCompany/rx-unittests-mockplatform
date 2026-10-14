extern alias IsolatedArea;
using System.Runtime.Loader;
using HarmonyLib;
using IsolatedArea::Sungero.IsolatedArea.Client;
using IsolatedArea::Sungero.IsolatedArea.Shared;
using IsolatedArea::Sungero.IsolatedArea.Shared.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MockPlatform.Services;

public class IsolatedAreaProxy : IIsolatedAreaProxy
{
  public FunctionResult Invoke(string assemblyPath, FunctionInvokeArgs args, bool isRepeatingRequest)
  {
    var type = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath).GetType(args.FullTypeName);
    var convertedArgs = ConvertFieldsTypes(args);
    var method = AccessTools.Method(type, convertedArgs.MethodName, [.. convertedArgs.ArgTypes]);
    var result = method.Invoke(null, [.. convertedArgs.ArgValues]);
    if (method.ReturnType == typeof(void))
      return FunctionResult.Done;

    var functionResult = new FunctionResult
    {
      Value = result,
      Type = method.ReturnType
    };
    if (functionResult.Value is Stream)
      return functionResult;

    return ConvertFieldsTypes(functionResult);
  }

  public bool Ping() => throw new NotImplementedException();

  private static T ConvertFieldsTypes<T>(T obj)
  {
    var settings = new JsonSerializerSettings
    {
      TypeNameHandling = TypeNameHandling.Auto,
      DateParseHandling = DateParseHandling.DateTimeOffset,
      ContractResolver = new DefaultContractResolver(),
      SerializationBinder = new IsolatedJsonSerializationBinder(),
      Converters = [new IsolatedTypeJsonConverter()]
    };
    var json = JsonConvert.SerializeObject(obj, settings);
    return JsonConvert.DeserializeObject<T>(json, settings)!;
  }
}
