using Sungero.Core;
using Sungero.Workflow;
using Sungero.Workflow.Blocks;

namespace MockPlatform.Services.Workflow;

/// <summary>
/// No-op реализация валидатора свойств блоков для unit-тестов.
/// </summary>
public sealed class MockBlockPropertyValidator : IBlockPropertyValidator
{
  public void ValidateCustomPropertyValue(BlockBase block, string name, object value)
  {
  }

  public void ValidateEnumPropertyValue(Guid blockTypeId, string propertyName, string value, Enumeration? enumeration)
  {
  }
}
