using System.Linq.Expressions;
using MockPlatform.Entities;
using MockPlatform.Extensions;
using Sungero.Domain.Shared;

namespace MockPlatform.Repositories;

public class RepositoryImplementer<T> : IRepository<T>, IRepositoryImplementer<T>
  where T : class, IEntity
{
  public IEntityAccessRights AccessRights => throw new NotImplementedException();

  public IEntityInfo Info => throw new NotImplementedException();

  public T As(IEntity castEntity) => castEntity as T;

  public T Copy(T item) => throw new NotImplementedException();

  public T Create()
  {
    var entity = EntityFactory.Create<T>();
    MockRepositories.Entities.Add(entity);
    entity.SetId(GetAll().Count());
    return entity;
  }

  public void Delete(T item) => MockRepositories.Entities.Remove(item);

  public T Get(long id) => MockRepositories.Entities.OfType<T>().FirstOrDefault(e => e.Id == id);

  public IQueryable<T> GetAll() => MockRepositories.Entities.OfType<T>().ToArray().AsQueryable();

  public IQueryable<T> GetAll(Expression<Func<T, bool>> criteria) => GetAll().Where(criteria);

  public IQueryable<T> GetAllCached() => this.GetAll();

  public IQueryable<T> GetAllCached(Expression<Func<T, bool>> criteria) => throw new NotImplementedException();

  public T GetCached(long id) => throw new NotImplementedException();

  public bool Is(IEntity comparedEntity) => comparedEntity is T;

  public IQueryable<T> Union(IQueryable<T> first, IQueryable<T> second, params IQueryable<T>[] sources) => throw new NotImplementedException();
}
