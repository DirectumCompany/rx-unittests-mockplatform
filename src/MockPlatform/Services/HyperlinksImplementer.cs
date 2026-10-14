using Sungero.Domain.Shared;

namespace MockPlatform.Services;

public class HyperlinksImplementer : IHyperlinksImplementer
{
  public Uri BaseUri => new("http://mock.hyperlinks/Sungero");

  public static Uri OpenedUri { get; private set; }

  public string Get(IEntity entity) => $"{BaseUri}?type=00000000-0000-0000-0000-000000000000&id={entity.Id}";

  public string Get(IEntityInfo entityInfo) => $"{BaseUri}?type=00000000-0000-0000-0000-000000000000";

  public string Get(IEntityInfo entityInfo, long id) => $"{BaseUri}?type=00000000-0000-0000-0000-000000000000&id={id}";

  public void Open(Uri url) => OpenedUri = url;

  public static void ClearOpenedUri() => OpenedUri = null;
}
