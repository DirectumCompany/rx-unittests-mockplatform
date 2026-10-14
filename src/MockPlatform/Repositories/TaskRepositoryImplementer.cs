using MockPlatform.Core;
using Sungero.Workflow;
using Sungero.Workflow.Shared;

namespace MockPlatform.Repositories;

public class TaskRepositoryImplementer<T> : RepositoryImplementer<T>, ITaskRepositoryImplementer<T>, IMockable<ITaskRepositoryImplementer<T>>
  where T : class, ITask
{
  ITaskAccessRights ITaskRepositoryImplementer<T>.AccessRights 
    => IMockable<ITaskRepositoryImplementer<T>>.Mock?.AccessRights ?? throw new NotImplementedException();

  ITaskInfo ITaskRepositoryImplementer<T>.Info
    => IMockable<ITaskRepositoryImplementer<T>>.Mock?.Info ?? throw new NotImplementedException();
}
