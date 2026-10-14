extern alias ClientBase;
using CommonLibrary.Dependencies;
using MockPlatform.Extensions;
using MockPlatform.Logging;
using MockPlatform.Services;
using MockPlatform.Services.Dialogs;
using MockPlatform.Services.Tenants;
using MockPlatform.Services.Workflow;
using Sungero.Domain;
using Sungero.Domain.Shared;
using Sungero.Metadata;
using Sungero.Metadata.Services;
using Sungero.Security;
using Sungero.Tenants;
using Sungero.Workflow;
using Sungero.Workflow.Blocks;
using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Unity;
using MetadataServiceImplementer = MockPlatform.Services.MetadataServiceImplementer;
using SignatureFactory = MockPlatform.Services.SignatureFactory;
using TaskExtensions = MockPlatform.Extensions.TaskExtensions;
using EntityViewsExtensions = MockPlatform.Extensions.EntityViewsExtensions;

namespace MockPlatform;

public static class ModuleManager
{
  private class AssemblyTypes(string moduleName, Type[] interfaceTypes, bool useServerPostfix)
  {
    public string ModuleName { get; } = moduleName;
    public Type[] Interface { get; } = interfaceTypes;
    public Type[] Server { get; } = LoadAssemblyTypes(useServerPostfix ? $"{moduleName}.Server" : moduleName);
    public Type[] Shared { get; } = LoadAssemblyTypes($"{moduleName}.Shared");
    public Type[] Client { get; } = LoadAssemblyTypes($"{moduleName}.ClientBase");
  }

  private delegate IUnityContainer OnRegister(IUnityContainer container, Type @interface, Type implementation);

  public static void RegisterModules(params string[] moduleNames)
  {
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    var domainInterfaceTypes = LoadAssemblyTypes("Sungero.Domain.Interfaces");
    foreach (var moduleName in moduleNames)
      RegisterModule(moduleName, domainInterfaceTypes);

    RegisterModule("Sungero.CoreEntities", "Sungero.Domain.Shared");
    RegisterModule("Sungero.Content", "Sungero.Content.Shared", useServerPostfix: false);
    RegisterModule("Sungero.Workflow", "Sungero.Workflow.Interfaces");

    RegisterPlatformTypes(["Sungero.CoreEntities", "Sungero.Content", "Sungero.Workflow", .. moduleNames]);
    LogManager.Configure();
  }

  private static void RegisterModule(string moduleName, string interfaceAssemblyName, bool useServerPostfix = true)
  {
    var interfaceTypes = LoadAssemblyTypes(interfaceAssemblyName);
    RegisterModule(moduleName, interfaceTypes, useServerPostfix);
  }

  private static void RegisterModule(string moduleName, Type[] interfaceTypes, bool useServerPostfix = true)
  {
    var moduleTypes = new AssemblyTypes(moduleName, interfaceTypes, useServerPostfix);
    RegisterEntityTypes(moduleTypes);
    RegisterResources(moduleTypes);
    RegisterPublicFunctions(moduleTypes);
  }

  private static void RegisterPlatformTypes(params string[] moduleNames)
  {
    ConfigureTenantRegistry();

    var customTypePropertyService = new MockCustomTypePropertyService();

    Sungero.Domain.Shared.ModuleManager.Instance.Container
      .RegisterType<IRemoteFunctionExecutor, Sungero.Domain.Server.RemoteFunctionExecutor>()
      .RegisterType<ILicenseManager, LicenseManager>()
      .RegisterType<IHyperlinksImplementer, Services.HyperlinksImplementer>()
      .RegisterType<ITaskExtensions, TaskExtensions>()
      .RegisterType<ISignatureFactory, SignatureFactory>()
      .RegisterType<ILocksImplementer, Services.LocksImplementer>()
      .RegisterType<ILockInfoImplementer, Services.LockInfoImplementer>()
      .RegisterType<IBlockPropertyValidator, MockBlockPropertyValidator>()
      .RegisterInstance<ICustomTypePropertyService>(customTypePropertyService);

    Dependency.RegisterType<IRemoteFunctionGetter, Sungero.Domain.Server.RemoteFunctionGetter>();
    Dependency.RegisterType<ClientBase::Sungero.Core.IDialogService, DialogService>();
    Dependency.RegisterType<ClientBase::Sungero.Core.IEntityViewsExtensions, EntityViewsExtensions>();
    Dependency.RegisterType<IAsyncHandlerExecutor, Services.AsyncHandlerExecutor>();
    Dependency.RegisterType<IBlockPropertyValidator, MockBlockPropertyValidator>();
    Dependency.RegisterInstance<ICustomTypePropertyService>(customTypePropertyService);
    Dependency.RegisterInstance<ITenantInfo>(new Services.TenantInfoImplementer());
    Dependency.RegisterInstance<IMetadataService>(new MetadataServiceImplementer(moduleNames));

    ExtensionInputDialogServicePatch.Apply();

    MetadataService.ConfigurationSettingsPaths = new ConfigurationSettingsPaths(AppDomain.CurrentDomain.BaseDirectory);
    CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
  }

  private static void ConfigureTenantRegistry()
  {
    if (TenantRegistry.Instance.IsConfigured)
      return;

    TenantRegistry.Instance
      .WithKernelSettings()
      .WithTenantSource(new MockTenantTestSource())
      .WithReload()
      .Configure(new MockTenantAppSettings());
  }

  private static void RegisterEntityTypes(AssemblyTypes types)
  {
    var moduleNamespace = $"{types.ModuleName}";
    var serverNamespace = $"{moduleNamespace}.Server";
    var entityTypes = types.Interface.Union(types.Server)
      .Where(t => t.GetInterfaces().Any(i => i.Name == "ISecurableEntity"))
      .Where(t => t.Namespace == moduleNamespace || t.Namespace == serverNamespace)
      .ToArray();

    RegisterTypes(entityTypes, (c, iface, _) => c.RegisterMockRepository(iface));
  }

  private static void RegisterResources(AssemblyTypes types)
  {
    var resources = types.Interface.Union(types.Shared)
      .Where(t => t.Name.EndsWith("Resources"))
      .Where(t => BelongsToModuleNamespace(t, types.ModuleName))
      .ToArray();

    RegisterTypes(resources);
  }

  private static void RegisterPublicFunctions(AssemblyTypes types)
  {
    var publicFunctions = types.Interface.Union(types.Server)
      .Union(types.Client)
      .Union(types.Shared)
      .Where(t => t.Name.Contains("PublicFunctions") || t.Name.Contains("InitializationFunctions"))
      .Where(t => BelongsToModuleNamespace(t, types.ModuleName))
      .ToArray();

    RegisterTypes(publicFunctions);
  }

  /// <summary>
  /// Проверить, что тип принадлежит пространству имён модуля.
  /// </summary>
  /// <param name="type">Тип.</param>
  /// <param name="moduleName">Имя модуля.</param>
  /// <returns>True, если тип принадлежит модулю.</returns>
  private static bool BelongsToModuleNamespace(Type type, string moduleName)
  {
    var moduleNamespace = moduleName;
    return type.Namespace == moduleNamespace
      || type.Namespace?.StartsWith($"{moduleNamespace}.") == true;
  }

  private static void RegisterTypes(Type[] allTypes, OnRegister? onRegister = null)
  {
    var interfaces = allTypes
      .Where(t => t.IsInterface)
      .ToArray();

    var implementations = allTypes
      .Where(t => !t.IsInterface)
      .ToArray();

    var typesToRegister = interfaces
      .SelectMany(iface => implementations, (iface, impl) => (iface, impl))
      .Where(t => t.impl.Name == t.iface.Name[1..])
      .ToArray();
    var container = Sungero.Domain.Shared.ModuleManager.Instance.Container;
    foreach (var (@interface, implementation) in typesToRegister)
    {
      container.RegisterType(@interface, implementation);
      onRegister?.Invoke(container, @interface, implementation);
    }
  }

  private static Type[] LoadAssemblyTypes(string assemblyName)
    => AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName)).GetTypes();
}