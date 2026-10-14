using Sungero.Core;
using Sungero.Workflow;
using Sungero.Workflow.Task;

namespace MockPlatform.Extensions;

public class TaskExtensions : ITaskExtensions
{
  public void Start(ITask task) => task.Status = Status.InProcess;

  public void Restart(ITask task) => task.Status = Status.InProcess;

  public void Abort(ITask task) => task.Status = Status.Aborted;

  public void Resume(ITask task) => task.Status = Status.InProcess;

  public StateView GetProcessStagesStateView(ITask task) => StateView.Create();
}

public class SimpleTaskExtensions
{
  public static List<ISimpleTask> GetStartedTasks() => [.. SimpleTasks.GetAll(x => x.Status == Status.InProcess)];
}
