using MockPlatform.Core;
using Sungero.CoreEntities;
using Sungero.CoreEntities.Shared;

namespace MockPlatform.Repositories;

public class GroupRepositoryImplementer<T> : RecipientRepositoryImplementer<T>, IGroupRepositoryImplementer<T>, IMockable<IGroupRepositoryImplementer<T>>
  where T : class, IGroup
{
  IGroupAccessRights IGroupRepositoryImplementer<T>.AccessRights => throw new NotImplementedException();

  IGroupInfo IGroupRepositoryImplementer<T>.Info => throw new NotImplementedException();

  public IEnumerable<IUser> GetAllUsersInGroup(IGroup group)
    => IMockable<IGroupRepositoryImplementer<T>>.Mock?.GetAllUsersInGroup(group) ?? GetDefaultAllUsersInGroup(group);

  private IEnumerable<IUser> GetDefaultAllUsersInGroup(IGroup group)
  {
    List<IUser> users = [];
    foreach (var link in group.RecipientLinks)
    {
      if (link.Member is IUser user)
        users.Add(user);
      else if (link.Member is IGroup subGroup)
        users.AddRange(GetAllUsersInGroup(subGroup));
    }
    return users;
  }
}