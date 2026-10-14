using Sungero.CoreEntities;
using Sungero.CoreEntities.Shared;

namespace MockPlatform.Repositories;

public class UserRepositoryImplementer<T> : RecipientRepositoryImplementer<T>, IUserRepositoryImplementer<T>
  where T : class, IUser
{
  public T Current
  {
    get
    {
      if (MockCurrentUser.Current == null)
        throw new InvalidOperationException(
          "Текущий пользователь не установлен. Используйте MockCurrentUser.Set(user) в начале теста.");

      return (MockCurrentUser.Current as T)!;
    }
  }

  public IEnumerable<long> ServiceUserIds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  IUserAccessRights IUserRepositoryImplementer<T>.AccessRights => throw new NotImplementedException();

  IUserInfo IUserRepositoryImplementer<T>.Info => throw new NotImplementedException();

  public TimeSpan GetUtcOffset(IRecipient recipient) => TimeSpan.Zero;
}
