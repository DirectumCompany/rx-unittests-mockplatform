using Sungero.Core;
using Sungero.Workflow;
using Status = Sungero.Workflow.Task.Status;

namespace MockPlatform.Tests;

public class Tasks
{
  [Test]
  public void TaskStarted()
  {
    // Arrange.
    var task = SimpleTasks.Create();

    // Act.
    task.Start();

    // Assert.
    Assert.That(task.Status, Is.EqualTo(Status.InProcess));
  }

  [Test]
  public void TaskRestarted()
  {
    // Arrange.
    var task = SimpleTasks.Create();

    // Act.
    task.Restart();

    // Assert.
    Assert.That(task.Status, Is.EqualTo(Status.InProcess));
  }

  [Test]
  public void TaskAborted()
  {
    // Arrange.
    var task = SimpleTasks.Create();

    // Act.
    task.Abort();

    // Assert.
    Assert.That(task.Status, Is.EqualTo(Status.Aborted));
  }

  [Test]
  public void TaskResumed()
  {
    // Arrange.
    var task = SimpleTasks.Create();

    // Act.
    task.Resume();

    // Assert.
    Assert.That(task.Status, Is.EqualTo(Status.InProcess));
  }

  [Test]
  public void CreateAsSubtask()
  {
    // Arrange.
    var parentTask = SimpleTasks.Create();
    parentTask.Subject = "Родительская задача";

    // Act.
    var subtask = SimpleTasks.CreateAsSubtask(parentTask);

    /// Assert.
    Assert.That(subtask, Is.Not.Null);
    Assert.That(((IInternalTask)subtask).ParentTask, Is.EqualTo(parentTask));
  }
}
