extern alias ClientBase;

using ClientBase::Sungero.Core;
using CommonLibrary;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;

namespace MockPlatform.Services.Dialogs;

public class DialogService : IDialogService
{
  public IInputDialog CreateInputDialog(string title, string text)
    => new InputDialog(title, text);

  public ITaskDialog CreateTaskDialog(string text, string description, MessageType type, string title)
    => new TaskDialog(text, description, type, title);

  public void NotifyMessage(string text)
    => ShowMessage(text, string.Empty, MessageType.Information, string.Empty);

  public void ShowMessage(string text, string description, MessageType type, string title)
    => CreateTaskDialog(text, description, type, title).Show();

  public ISelectTypeDialog CreateSelectTypeDialog(string title, params IEntityInfo[] types)
    => throw new NotImplementedException();

  public ISelectTypeDialog CreateSelectTypeDialog(string title, bool withHiddenTypes, params IEntityInfo[] types)
    => throw new NotImplementedException();

  public bool CanCreate() => true;

  public IConfirmDialog CreateConfirmDialog(string text, string description, string title)
    => throw new NotImplementedException();

  public ISearchCriteriaDialog<T> CreateSearchDialog<T>(string title) where T : IEntity
    => throw new NotImplementedException();

  IFileSelectionDialog<IFileObject> IDialogService.CreateFileSelectionDialog()
    => throw new NotImplementedException();

  public IFileSelectionDialog<IEnumerable<IFileObject>> CreateFileManySelectionDialog()
    => throw new NotImplementedException();

  public ICustomDialogInstance CreateCustomDialog(ICustomDialog customDialog)
    => throw new NotImplementedException();

  public ICompositeCustomDialog CreateCompositeCustomDialog()
    => throw new NotImplementedException();
}
