using CommonLibrary.Reflection;
using System.Reflection;
using Sungero.Metadata;
using Sungero.Metadata.Services;
using System.Runtime.Loader;

namespace MockPlatform.Services;

public class MetadataServiceImplementer : IMetadataService
{
  private readonly string[] moduleNames;
  private ModuleList moduleList = new();
  public ModuleList ModuleList => moduleList;

  public IEnumerable<SpecialFolderMetadata> AllSpecialFolders => throw new NotImplementedException();

  public MetadataServiceImplementer(params string[] moduleNames)
  {
    this.moduleNames = moduleNames;
    RefreshModuleList();
  }

  public void RefreshModuleList()
  {
    string[] sharedAssembliesNames = ["Sungero.Reporting", .. moduleNames];
    var sharedAssemblies = sharedAssembliesNames
      .Select(name => AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName($"{name}.Shared")));

    moduleList = new ModuleList();

    foreach (var assembly in sharedAssemblies)
    {
      var moduleMetadata = GetModuleMetadata(new ReflectionAssemblyMetadata(assembly));
      if (moduleMetadata != null && !moduleMetadata.IsLayerModule() && moduleList.Modules.All(m => !m.Equals(moduleMetadata)))
        moduleList.Modules.Add(moduleMetadata);
    }
  }

  private static ModuleMetadata? GetModuleMetadata(ReflectionAssemblyMetadata assemblyMetadata)
  {
    var moduleResourceName = string.Format("{0}.{1}", assemblyMetadata.AssemblyName, MetadataConstants.ModuleMetadataFileName);
    var moduleMetadataResource = assemblyMetadata.GetManifestResourceNames()
      .SingleOrDefault(r => r.Equals(moduleResourceName, StringComparison.OrdinalIgnoreCase));
    if (moduleMetadataResource is null)
      return null;

    if (!assemblyMetadata.TryGetAssemblyResource(moduleMetadataResource, out var metadataFile))
      return null;

    using var stream = new MemoryStream(metadataFile);
    var moduleMetadata = MetadataSerializer.Instance.Deserialize<ModuleMetadata>(stream);
    if (moduleMetadata != null)
      moduleMetadata.SharedAssembly = assemblyMetadata;

    return moduleMetadata;
  }
}
