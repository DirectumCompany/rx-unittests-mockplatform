using Sungero.Metadata.Services;

namespace MockPlatform.Services
{
  internal class ConfigurationSettingsPaths : IConfigurationSettingsPaths
  {
    public string DomainModulesPath { get; init; }

    public string LogConfigFilePath => throw new NotImplementedException();

    public string DomainConfigFilePath => throw new NotImplementedException();

    public string DevelopmentConfigFilePath => throw new NotImplementedException();

    public string AppliedModulesPath { get; init; }

    public IReadOnlyList<string> GetLocations() => [DomainModulesPath, AppliedModulesPath];

    public ConfigurationSettingsPaths(string baseDirectory)
    {
      DomainModulesPath = baseDirectory;
      AppliedModulesPath = baseDirectory;
    }
  }
}
