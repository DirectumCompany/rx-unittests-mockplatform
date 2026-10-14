extern alias ClientBase;

using System.Globalization;
using ClientBase::Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using MockPlatform.Core;

namespace MockPlatform.Extensions;

public class EntityViewsExtensions : IEntityViewsExtensions, IMockable<IEntityViewsExtensions>
{
  public void Show(IEntity entity)
  {
    return;
  }

  public void Show(IEntity entity, string title)
  {
    return;
  }

  public void Show(Type entityType, long id)
  {
    return;
  }

  public void Show(Type entityType)
  {
    return;
  }

  public void Show<T>(IEnumerable<T> entities) where T : class, IEntity
  {
    return;
  }

  public void Show<T>(IEnumerable<T> entities, string title) where T : class, IEntity
  {
    return;
  }

  public void ShowModal(IEntity entity)
  {
    return;
  }

  public void ShowModal(IEntity entity, string title)
  {
    return;
  }

  public void ShowModal(IEntity entity, string title, ModalWidth modalWidth)
  {
    return;
  }

  public void ShowModal(IEntity entity, string title, ModalWidth modalWidth, ModalHeight modalHeight)
  {
    return;
  }

  public void ShowModal<T>(IEnumerable<T> entities) where T : class, IEntity
  {
    return;
  }

  public void ShowModal<T>(IEnumerable<T> entities, string title) where T : class, IEntity
  {
    return;
  }

  public T ShowSelect<T>(IEnumerable<T> entities) where T : class, IEntity    
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelect<T>(entities, GetSelectString<T>(), null);

  public T ShowSelect<T>(IEnumerable<T> entities, string title) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelect<T>(entities, title, null);

  public T ShowSelect<T>(IEnumerable<T> entities, string title, string searchText) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelect<T>(entities, title, searchText);

  public T ShowSelectInFlatList<T>(IEnumerable<T> entities) where T : class, IRecipient
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectInFlatList<T>(entities, GetSelectString<T>(), null);

  public T ShowSelectInFlatList<T>(IEnumerable<T> entities, string title) where T : class, IRecipient
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectInFlatList<T>(entities, title, null);

  public T ShowSelectInFlatList<T>(IEnumerable<T> entities, string title, string searchText) where T : class, IRecipient
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectInFlatList<T>(entities, title, searchText);

  public IEnumerable<T> ShowSelectMany<T>(IEnumerable<T> entities) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectMany<T>(entities, GetSelectString<T>(), null);

  public IEnumerable<T> ShowSelectMany<T>(IEnumerable<T> entities, string title) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectMany<T>(entities, title, null);

  public IEnumerable<T> ShowSelectMany<T>(IEnumerable<T> entities, string title, string searchText) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectMany<T>(entities, title, searchText) ?? Enumerable.Empty<T>();

  public IQueryable<T> ShowSelectMany<T>(IQueryable<T> query) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectMany<T>(query, GetSelectString<T>(), null);

  public IQueryable<T> ShowSelectMany<T>(IQueryable<T> query, string title) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectMany<T>(query, title, null);

  public IQueryable<T> ShowSelectMany<T>(IQueryable<T> query, string title, string searchText) where T : class, IEntity
    => IMockable<IEntityViewsExtensions>.Mock?.ShowSelectMany<T>(query, title, searchText) ?? Enumerable.Empty<T>().AsQueryable();

  public void ShowHistory(EntityIdentifier entityIdentifier)
  {
    return;
  }

  private static string GetSelectString<T>() where T : class, IEntity
  {
    string accusativeName = typeof(T).GetEntityMetadata().GetAccusativeDisplayName();
    return $"Select {accusativeName.ToLower(CultureInfo.InvariantCulture)}";
  }  
}