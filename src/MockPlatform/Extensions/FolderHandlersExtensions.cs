using Sungero.Domain.Server;
using Sungero.Domain.Shared;

namespace MockPlatform.Extensions;

public static class FolderHandlersExtensions
{
  public static T WithFilter<T>(this T handlers, IFilterState filter)
    where T : FolderHandlersBaseProxy
  {
    var filterProperty = handlers.GetType().GetProperty("Filter");
    filterProperty?.SetValue(handlers, filter);
    return handlers;
  }
}
