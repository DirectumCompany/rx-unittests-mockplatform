# Примеры юнит-тестов

## Пример 1. Функция без зависимостей

Простейший случай — статическая общая функция, проверяющая формат строки. Подготавливать нечего, поэтому секция Arrange пуста, но маркер сохраняется. Кейсы разнесены по отдельным тестам, каждый со своим описанием.

```csharp
using Sungero.Domain.Shared;

namespace UnitTests.Commons.GenAIServiceSharedFunctions;

public class ValidateServiceUrl
{
  [Test]
  [Description("URL с протоколом https — URL валиден.")]
  public void ValidateServiceUrl_HttpsUrl_ReturnsTrue()
  {
    // Arrange

    // Act
    var result = Sungero.Commons.Shared.GenAIServiceFunctions
      .ValidateServiceUrl(@"https://serviceAI.my.ru/v1/chat/completions");

    // Assert
    Assert.That(result, Is.True);
  }

  [Test]
  [Description("URL с обратными слэшами — URL невалиден.")]
  public void ValidateServiceUrl_UrlWithBackSlashes_ReturnsFalse()
  {
    // Arrange

    // Act
    var result = Sungero.Commons.Shared.GenAIServiceFunctions
      .ValidateServiceUrl(@"http:\\serviceAI.my.ru\v1\chat\completions");

    // Assert
    Assert.That(result, Is.False);
  }
}
```

## Пример 2. Однотипные кейсы через [TestCaseSource]

Когда тесты отличаются только входными данными, их не копируют, а параметризуют. Наборы кейсов объявляются рядом с тестами или выносятся в файл {Метод}TestData.cs. Сообщение ассерта подставляет проверяемое значение, поэтому по отчёту сразу видно, какой кейс упал.

```csharp
namespace UnitTests.Commons.ModuleSharedFunctions
{
  [TestFixture]
  public class IsEmailFormatMatch
  {
    public static object[] MatchCases =
    [
      new object[] { "r@r.r" },
      new object[] { " r@r.ru " },
      new object[] { "Я@Я.рф" },
      new object[] { "r@r.r,r@r.r" },
      new object[] { "r@r.r r@r.r, r@r.r; r@r.r" }
    ];

    public static object[] NotMatchCases =
    [
      new object[] { "r @r.r" },
      new object[] { "@r@r.ru" },
      new object[] { "r@r" },
      new object[] { "r,r@r.r,rr@r.r" }
    ];

    [Test]
    [TestCaseSource(nameof(MatchCases))]
    [Description("Адрес в корректном формате — формат e-mail распознан.")]
    public void IsEmailFormatMatch_ValidEmail_ReturnsTrue(string emailInput)
    {
      // Arrange

      // Act
      var isMatch = Sungero.Commons.PublicFunctions.Module.IsEmailFormatMatch(emailInput);

      // Assert
      Assert.That(isMatch, Is.True, $"\"{emailInput}\" — валидный e-mail");
    }

    [Test]
    [TestCaseSource(nameof(NotMatchCases))]
    [Description("Адрес в некорректном формате — формат e-mail не распознан.")]
    public void IsEmailFormatMatch_InvalidEmail_ReturnsFalse(string emailInput)
    {
      // Arrange

      // Act
      var isMatch = Sungero.Commons.PublicFunctions.Module.IsEmailFormatMatch(emailInput);

      // Assert
      Assert.That(isMatch, Is.False, $"\"{emailInput}\" — не валидный e-mail");
    }
  }
}
```

## Пример 3. Ожидаемый текст из ресурсов модуля

Если метод возвращает текст сообщения, ожидаемое значение берётся из ресурсов модуля, а не дублируется строковым литералом: иначе тест сломается при любой правке формулировки. В этом же примере показан [TestCase] с описанием кейса в самом атрибуте.

```csharp
using Sungero.Parties;

namespace UnitTests.Parties.CompanyBaseSharedFunctions;

[TestFixture]
public class CheckPsrnLength
{
  [Test]
  [TestCase("1234567890123", Description = "ОГРН из цифр — ошибки нет.")]
  [TestCase("123456789012345", Description = "ОГРН ИП из цифр — ошибки нет.")]
  public void CheckPsrnLength_NumericPsrn_ReturnsEmptyString(string psrn)
  {
    // Arrange
    var functions = new Sungero.Parties.Shared.CompanyBaseFunctions();

    // Act
    var validationResult = functions.CheckPsrnLength(psrn);

    // Assert
    Assert.That(validationResult, Is.Empty);
  }

  [Test]
  [Description("ОГРН содержит буквы — возвращается текст ошибки.")]
  public void CheckPsrnLength_AlphanumericPsrn_ReturnsErrorMessage()
  {
    // Arrange
    var functions = new Sungero.Parties.Shared.CompanyBaseFunctions();

    // Act
    var validationResult = functions.CheckPsrnLength("123456789o123");

    // Assert
    Assert.That(validationResult, Is.EqualTo(CompanyBases.Resources.NotOnlyDigitsPsrn.ToString()));
  }
}
```

## Пример 4. Сущности и экземплярная функция

Как только методу нужны сущности, они создаются через репозиторий мок-платформы. Созданная сущность сразу попадает в репозиторий в памяти, поэтому тестируемый метод найдёт её обычным запросом — как в этом примере, где проверяется уникальность логина среди абонентских ящиков.

```csharp
using MockPlatform.Extensions;
using Sungero.ExchangeCore;
using Sungero.ExchangeCore.Server;

namespace UnitTests.ExchangeCore.BusinessUnitBoxServerFunctions
{
  /// <summary>
  /// Тесты метода LoginIsAvailable сущности BusinessUnitBox.
  /// </summary>
  public class LoginIsAvailable
  {
    [Test]
    [Description("Логин занят другим ящиком того же сервиса обмена — логин недоступен.")]
    public void LoginIsAvailable_DuplicateLogin_ReturnsFalse()
    {
      // Arrange

      // Создание сервиса обмена.
      var exchangeService = ExchangeServices.Create();
      exchangeService.Name = "Test Exchange Service";

      // Создание текущего абонентского ящика.
      var currentBox = BusinessUnitBoxes.Create();
      currentBox.Login = "current_login";
      currentBox.ExchangeService = exchangeService;

      // Создание другого абонентского ящика с логином, который мы хотим проверить.
      var otherBox = BusinessUnitBoxes.Create();
      otherBox.Login = "duplicate_login";
      otherBox.ExchangeService = exchangeService;

      var businessUnitBoxFunctions = currentBox.GetServerFunctions<BusinessUnitBoxFunctions>();

      // Act
      var result = businessUnitBoxFunctions.LoginIsAvailable("duplicate_login");

      // Assert
      Assert.That(result, Is.False);
    }
  }
}
```

Что здесь существенно:

- все сущности созданы через Create() — ни new, ни Mock.Of<T>();
- экземплярная функция вызывается через entity.GetServerFunctions<T>();
- переменная otherBox нужна не для ассерта, а как данные, на которых работает метод, — это единственный допустимый случай «неиспользуемой» переменной, и он объяснён комментарием;
- класс назван именем тестируемого метода, файл лежит в Tests/EntityServerFunctions/BusinessUnitBox/LoginIsAvailable.cs.

## Пример 5. Manager и Helper: куда выносить общий код

Общий код, нужный нескольким тестовым классам, делится по роли:

- Manager — создание и настройка сущностей. Готовит объекты в нужной для теста конфигурации и ничего с ними не делает.
- Helper — процессы и операции над уже созданными объектами, а также общие для тестов приёмы работы с мок-платформой. По сути это сервис. Создавать сущности в Helper нельзя.

Manager — небольшой статический класс с именованными способами создания сущности:

```csharp
using MockPlatform.Extensions;
using Sungero.Company;

namespace UnitTests.DocflowApproval.Environment;

/// <summary>
/// Создание сотрудников для unit-тестов DocflowApproval.
/// </summary>
public static class EmployeeManager
{
  /// <summary>
  /// Создать сотрудника с указанным идентификатором.
  /// </summary>
  /// <param name="id">Идентификатор сотрудника.</param>
  /// <returns>Сотрудник.</returns>
  public static IEmployee Create(long id)
  {
    var employee = Employees.Create();
    employee.SetId(id);
    return employee;
  }
}
```

В тесте он занимает одну строку Arrange, и по коду сразу видно, какие данные нужны сценарию:

```csharp
using Sungero.DocflowApproval.Server;
using UnitTests.DocflowApproval.Environment;

namespace UnitTests.DocflowApproval.ModuleServerFunctions.ApprovalBlock;

/// <summary>
/// Тесты метода UpdateAprovalBlockPerformers.
/// </summary>
public class UpdateAprovalBlockPerformers
{
  [Test]
  [Description("Первый круг согласования без доработок — все согласующие остаются исполнителями.")]
  public void UpdateAprovalBlockPerformers_FirstApprovalCircleWithoutRework_KeepsAllPerformers()
  {
    // Arrange
    var firstApprover = EmployeeManager.Create(1);
    var secondApprover = EmployeeManager.Create(2);

    var block = BlockManager.CreateApprovalBlock([firstApprover, secondApprover]);
    var functions = new ModuleFunctions();

    // Act
    functions.UpdateAprovalBlockPerformers(block, null, null);

    // Assert
    Assert.That(block.Performers, Has.Count.EqualTo(2), "В блоке должны остаться оба исполнителя.");
    Assert.That(block.Performers.Select(performer => performer.Id),
      Is.EquivalentTo(new[] { firstApprover.Id, secondApprover.Id }),
      "Состав исполнителей блока не должен измениться.");
  }
}
```

Helper выполняет процесс над уже созданными сущностями. Здесь это вызов действия задания и подмена диалога подтверждения — техника, одинаковая для всех тестов действий этой сущности, поэтому её и вынесли из тестовых классов:

```csharp
namespace UnitTests.ExchangeCore.EntityActions.IncomingInvitationAssignment
{
  public static class IncomingInvitationAssignmentActionsHelper
  {
    public static void ExecuteReject(IIncomingInvitationAssignment assignment)
    {
      CallAction(
        assignment,
        nameof(IncomingInvitationAssignmentActions.Reject),
        (actions, args) => actions.Reject(args));
    }

    public static void MockConfirmationDialog(bool result, Action onCalled = null)
    {
      MockFunctionStubs.Setup<Sungero.Docflow.Client.ModuleClientPublicFunctions>(
        nameof(Sungero.Docflow.Client.ModuleClientPublicFunctions.ShowConfirmationDialog),
        (string message, string description, string title, string dialogId) =>
        {
          onCalled?.Invoke();

          return result;
        });
    }

    private static void CallAction(
      IIncomingInvitationAssignment assignment,
      string actionName,
      Action<IncomingInvitationAssignmentActions, Sungero.Workflow.Client.ExecuteResultActionArgs> execute)
    {
      var actions = new IncomingInvitationAssignmentActions();
      actions.SetEntity(assignment);

      var actionMetadata = new ActionMetadata(assignment.GetEntityMetadata());
      actionMetadata.Name = actionName;

      var actionInfo = new ActionInfo(actionMetadata);
      var args = new Sungero.Workflow.Client.ExecuteResultActionArgs(FormType.Card, assignment, actionInfo);

      execute(actions, args);
    }
  }
}
```

Тест после этого читается как сценарий: данные готовит Manager, процесс выполняет Helper, тестовый класс остаётся с Arrange из нескольких строк, одним вызовом в Act и проверками в Assert.

## Пример 6. Действие, текущий пользователь и права доступа

Действия сущностей требуют более богатого окружения: текущего пользователя, прав доступа и подмены диалога подтверждения. Общее для всех тестов класса окружение поднимается в [SetUp]. Обратите внимание: ни очистки репозиториев и стабов, ни освобождения текущего пользователя в классе нет — и то и другое делает автоматический сброс перед каждым тестом.

```csharp
using MockPlatform;
using MockPlatform.Core;
using MockPlatform.Stubs;
using Sungero.Company;
using Sungero.Company.Client;
using Sungero.Domain.Client;
using Sungero.Domain.Shared;
using DocflowClientFunctions = Sungero.Docflow.Client.ModuleClientPublicFunctions;

namespace UnitTests.Company.EntityActions.UserBase
{
  public class DisableUsersWorkInWebClient
  {
    [SetUp]
    public void SetUp()
    {
      MockCurrentUser.Set(UserBases.Create());

      RepositoryAccessRightsStubs.Setup<IUserBaseAccessRights>(typeof(UserBases),
        (mock) => { mock.Setup(ar => ar.CanUpdate()).Returns(true); });

      MockFunctionStubs.Setup<DocflowClientFunctions>(
        nameof(DocflowClientFunctions.ShowConfirmationDialog),
        (string _, string _, string _, string _) => true);
    }

    [Test]
    [Description("У пользователя включён признак «Работает в веб-клиенте» — признак снимается.")]
    public void DisableUsersWorkInWebClient_UserWorksInWebClient_AttributeDisabled()
    {
      // Arrange
      var userBase = UserBases.Create();
      userBase.IsWorkingInWebClient = true;

      // Act
      var actions = new UserBaseCollectionBulkActions();
      actions.SetEntityIds([new EntityIdentifier(userBase)]);
      var args = new ExecuteActionArgs(FormType.Collection, null);
      actions.DisableUsersWorkInWebClient(args);

      // Assert
      Assert.That(userBase.IsWorkingInWebClient, Is.False);
    }
  }
}
```

## Пример 7. Состояние и метаданные сущности

Если метод меняет состояние карточки — обязательность, доступность или видимость реквизитов, — у мок-сущности нужно предварительно поднять State и Info. Это делается инициализатором типа, зарегистрированным в [SetUp]: он применяется ко всем сущностям данного типа, созданным в тесте, в том числе внутри тестируемого кода. Инициализатор получает Mock<T>, а не саму сущность, поэтому внутри доступны Setup(...) и расширения вроде SetupStateAndInfo(). Возвращаемый IDisposable нужен, только если инициализатор требуется снять внутри теста: между тестами их снимает автоматический сброс. Данные готовит CaseFileManager — создание сущностей относится к Manager (см. Пример 5).

```csharp
using MockPlatform.Core;
using MockPlatform.Extensions;
using Sungero.Docflow;
using UnitTests.Docflow.Environment;
using SharedCaseFileFunctions = Sungero.Docflow.Shared.CaseFileFunctions;

namespace UnitTests.Docflow.CaseFileSharedFunctions;

/// <summary>
/// Тесты метода SetRequiredProperties.
/// </summary>
public class SetRequiredProperties
{
  private const string CaseFileIndex = "1";

  [SetUp]
  public void SetUp()
  {
    MockInstanceInitializer.AddTypeInitializer<ICaseFile>(inst => inst.SetupStateAndInfo());
  }

  [Test]
  [Description("Ведение дел в разрезе подразделений включено — подразделение обязательно.")]
  public void SetRequiredProperties_RequiredDepartmentEnabled_DepartmentIsRequired()
  {
    // Arrange.
    var businessUnit = BusinessUnitManager.CreateBusinessUnit();
    var caseFile = CaseFileManager.CreateCaseFile(CaseFileIndex, businessUnit);
    CaseFileManager.CreateRecordManagementSetting(isRequiredDepartment: true);

    // Act.
    caseFile.GetSharedFunctions<SharedCaseFileFunctions>().SetRequiredProperties();

    // Assert.
    Assert.That(caseFile.State.Properties.Department.IsRequired, Is.True,
      "При включенной настройке подразделение должно быть обязательным.");
  }
}
```

## Пример 8. Обработчик сущности: изменение свойства

Обработчик изменения значения реквизита тестируется так же, как в примере 9: обработчик создаётся напрямую, сущность привязывается к нему методом WithEntity, аргументы события собираются вручную. Состояние реквизита подменяется моком IPropertyState<string>, а серверные функции проверки и форматирования номера телефона — стабами:

```csharp
using Moq;
using MockPlatform.Core;
using MockPlatform.Extensions;
using Sungero.Domain.Shared;
using Sungero.Parties;
using HRPro.Hiring;

namespace UnitTests.Hiring.EntitySharedHandlers.Candidate;

/// <summary>
/// Тесты shared-обработчика PersonalPhoneChanged сущности Candidate.
/// </summary>
public class PersonalPhoneChanged
{
  [Test]
  [Description("Валидация номера пройдена, новый номер отличается от прежнего — " +
    "номер приводится к универсальному формату и сохраняется.")]
  public void PersonalPhoneChanged_ValidationPassedAndPhoneChanged_PhoneFormatted()
  {
    // Arrange
    var oldPhone = "+79111234567";
    var newPhone = "+79111234568";
    var expectedFormattedPhone = "+79111234568";
    var candidate = Candidates.Create();
    var person = People.Create();
    candidate.Person = person;
    candidate.PersonalPhone = oldPhone;

    var propertyStateMock = new Mock<IPropertyState<string>>();
    propertyStateMock.Setup(p => p.OriginalValue).Returns(oldPhone);
    propertyStateMock.Setup(p => p.PreviousValue).Returns(oldPhone);
    Mock.Get(candidate)
      .Setup(c => c.State.Properties.PersonalPhone)
      .Returns(propertyStateMock.Object);

    var args = CreateArgs(candidate.State.Properties.PersonalPhone, newPhone, candidate);

    using var checkPhoneStub =
      MockFunctionStubs.Setup<HRPro.Hiring.Server.ModuleFunctions>(
        nameof(HRPro.Hiring.Server.ModuleFunctions.CheckPersonalPhoneNumber),
        (string phoneNumber, IPerson phoneOwner) => new List<string>());

    using var formatPhoneStub =
      MockFunctionStubs.Setup<DirRX.EssPlatform.Server.ModuleFunctions>(
        nameof(DirRX.EssPlatform.Server.ModuleFunctions.PhoneNumberToUniversalFormat),
        (string phoneNumber) => expectedFormattedPhone);

    var handlers = new CandidateSharedHandlers();
    handlers.WithEntity(candidate);

    // Act
    handlers.PersonalPhoneChanged(args);

    // Assert
    Assert.That(candidate.PersonalPhone, Is.EqualTo(expectedFormattedPhone));
  }

  #region Helper методы

  /// <summary>
  /// Создать аргументы события изменения телефона.
  /// </summary>
  /// <param name="propertyState">Состояние реквизита.</param>
  /// <param name="newValue">Новое значение телефона.</param>
  /// <param name="candidate">Кандидат.</param>
  /// <returns>Аргументы события изменения значения.</returns>
  private StringPropertyChangedEventArgs CreateArgs(IPropertyState<string> propertyState,
    string? newValue, HRPro.Hiring.ICandidate candidate)
  {
    return new StringPropertyChangedEventArgs(propertyState, newValue, candidate);
  }

  #endregion
}
```

## Пример 9. Обработчики сущности: ошибки, предупреждения и подсказки

Обработчик сущности тестируется так же, как функция: обработчик создаётся напрямую, сущность привязывается к нему методом WithEntity, аргументы события собираются вручную. Сообщения, добавленные через e.AddError, e.AddWarning и e.AddInformation, попадают в один и тот же список GetValidationMessages() и различаются по MessageType: подсказка — Information, предупреждение — Warning, ошибка — Error.

Серверный обработчик сохранения:

```csharp
using MockPlatform.Extensions;
using Sungero.Domain;
using Sungero.Parties;
using Sungero.Parties.DueDiligenceStatus;

namespace UnitTests.Parties.EntityServerHandlers.DueDiligenceStatus;

[TestFixture]
public class BeforeSave
{
  [Test]
  [Description("Некорректный формат цвета — " +
    "возвращается ошибка валидации.")]
  public void BeforeSave_InvalidColorFormat_ProducesValidationError()
  {
    // Arrange
    var dueDiligenceStatus = this.CreateDueDiligenceStatus("A1B2C3");
    var args = new BeforeSaveEventArgs(dueDiligenceStatus);

    // Act
    new DueDiligenceStatusServerHandlers().WithEntity(dueDiligenceStatus).BeforeSave(args);

    // Assert
    var messages = args.Entity.GetValidationMessages();
    var errorMessage = messages.Select(x => x.Body).Single().ToString();
    var expectedError = DueDiligenceStatuses.Resources.IncorrectColor.ToString();
    Assert.That(errorMessage, Is.EqualTo(expectedError),
      "Ожидалось сообщение об ошибке формата цвета.");
  }

  #region Helper методы

  /// <summary>
  /// Создать вариант результата проверки контрагента с указанным цветом.
  /// </summary>
  /// <param name="color">Цвет.</param>
  /// <returns>Вариант результата проверки контрагента.</returns>
  private IDueDiligenceStatus CreateDueDiligenceStatus(string color)
  {
    var dueDiligenceStatus = DueDiligenceStatuses.Create();
    dueDiligenceStatus.Name = "Test BeforeSave";
    dueDiligenceStatus.Color = color;
    dueDiligenceStatus.Status = Status.Active;

    return dueDiligenceStatus;
  }

  #endregion
}
```

Клиентский обработчик обновления карточки добавляет подсказку. Аргументы клиентских событий лежат в отдельной сборке, поэтому файл начинается с extern alias, а типы указываются с квалификатором ClientBase::

```csharp
extern alias ClientBase;
using CommonLibrary;
using MockPlatform.Core;
using MockPlatform.Extensions;
using Sungero.ExchangeCore;

namespace UnitTests.ExchangeCore.EntityClientHandlers.ExchangeDocumentType;

public class Refresh
{
  [SetUp]
  public void SetUp()
  {
    MockInstanceInitializer
      .AddTypeInitializer<IExchangeDocumentType>(inst => inst.SetupStateAndInfo());
  }

  [Test]
  [Description("Sid типа документа заполнен — " +
    "карточка доступна только для чтения.")]
  public void Refresh_SidNotEmpty_FormIsReadOnly()
  {
    // Arrange.
    var docType = ExchangeDocumentTypes.Create();
    docType.Sid = "some-sid";

    var handlers = new ExchangeDocumentTypeClientHandlers();
    handlers.WithEntity(docType);
    var args = new ClientBase::Sungero.Presentation.FormRefreshEventArgs(
      docType, new ClientBase::Sungero.Domain.Client.TitleBuilder());

    // Act.
    handlers.Refresh(args);

    // Assert.
    var messages = docType.GetValidationMessages();
    Assert.Multiple(() =>
    {
      Assert.That(docType.State.IsEnabled, Is.False);
      Assert.That(messages, Has.Count.EqualTo(1));
      Assert.That(messages[0].MessageType, Is.EqualTo(ValidationMessageType.Information));
      Assert.That(messages[0].ToString(),
        Is.EqualTo(ExchangeDocumentTypes.Resources.DefaultTypeReadOnly.ToString()));
    });
  }
}
```

Что здесь существенно:

- сообщения всех трёх уровней читаются одним GetValidationMessages(), а уровень проверяется по MessageType — отдельного способа проверить «подсказку» нет;
- аргументы клиентских событий берутся из разных сборок: FormRefreshEventArgs и TitleBuilder лежат в клиентской сборке и требуют extern alias ClientBase с квалификатором ClientBase::, а Sungero.Presentation.*ValueInputEventArgs доступны без алиаса;
- если обработчик меняет доступность, видимость или обязательность реквизитов, у мок-сущности предварительно поднимаются State и Info (Пример 7);
- каталоги: Tests/EntityServerHandlers/{Сущность}/ и Tests/EntityClientHandlers/{Сущность}/.

## Пример 10. Диалог: сборка, контролы и заголовки

До диалога, собранного прикладным кодом, тест добирается двумя способами. Если диалог возвращает отдельный метод-фабрика, его вызывают напрямую и приводят результат к типу InputDialog мок-платформы:

```csharp
using CommonLibrary;
using MockPlatform.Core;
using MockPlatform.Extensions;
using MockPlatform.Services.Dialogs;
using Sungero.Docflow;
using Sungero.FinancialArchive;
using Sungero.FinancialArchive.Client;
using UnitTests.Exchange.Helpers;
using HelpCodes = Sungero.Docflow.Constants.AccountingDocumentBase.HelpCodes;

namespace UnitTests.FinancialArchive.ReconciliationStatementClientFunctions;

public class CreateSellerTitleDialog
{
  [Test]
  [Description("Акт сверки — создаётся диалог " +
    "заполнения титула отправителя.")]
  public void CreateSellerTitleDialog_ReconciliationStatement_CreatesSenderTitleDialog()
  {
    // Arrange.
    var document = ReconciliationStatements.Create();
    var resources = AccountingDocumentBases.Resources;
    var expectedTitle = resources.PropertiesFillingDialog_SenderTitle.ToString();
    var expectedText = resources.PropertiesFillingDialog_Text_SenderTitle.ToString();

    // Act.
    var dialog = this.CallCreateSellerTitleDialog(document);

    // Assert.
    Assert.Multiple(() =>
    {
      Assert.That(dialog.Title, Is.EqualTo(expectedTitle),
        "Заголовок берётся из ресурса PropertiesFillingDialog_SenderTitle.");
      Assert.That(dialog.Text, Is.EqualTo(expectedText),
        "Текст берётся из ресурса PropertiesFillingDialog_Text_SenderTitle.");
      Assert.That(dialog.HelpCode, Is.EqualTo(HelpCodes.ReconciliationStatement));
    });
  }

  #region Helper методы

  private InputDialog CallCreateSellerTitleDialog(IReconciliationStatement document)
  {
    var functions = document.GetClientFunctions<ReconciliationStatementFunctions>();
    using (MockFunctionStubs.RestoreOriginalCodeFromIL<ReconciliationStatementFunctions>(
      nameof(CreateSellerTitleDialog)))
    {
      return (InputDialog)NonPublicFunctionsHelper
        .InvokeInstanceFromHierarchy<IInputDialog>(
          functions, nameof(CreateSellerTitleDialog), Type.EmptyTypes);
    }
  }

  #endregion
}
```

Если диалог показывается внутри тестируемого метода, последний показанный диалог берут из статического свойства InputDialog.Current и разбирают его контролы:

```csharp
// Act
ExecuteAction(company);

// Assert
Assert.That(InputDialog.Current, Is.Not.Null,
  "Должен быть отображён диалог выбора организации.");
Assert.That(InputDialog.Current.Controls, Has.Count.EqualTo(1),
  "Диалог должен иметь один контрол.");

var companyControl = (DropDownDialogValue)InputDialog.Current.Controls[0];
Assert.Multiple(() =>
{
  Assert.That(companyControl.IsRequired,
    "Контрол выбора организации должен быть обязательным.");
  Assert.That(companyControl.AvailableValues, Has.Count.EqualTo(2),
    "В списке должны быть обе найденные организации.");
});
```

Что здесь существенно:

- контрол диалога приводится к конкретному типу мок-платформы, и это даёт доступ к Value и IsRequired: StringDialogValue и MultilineStringDialogValue — строки, BooleanDialogValue — признак, DropDownDialogValue — выпадающий список, NavigationDialogValue<T> — выбор сущности, SelectManyDialogValue<T> — множественный выбор; у трёх последних есть ещё и AvailableValues;
- ожидаемые заголовки и тексты диалога сверяются с ресурсами модуля, а не со строковыми литералами;
- RestoreOriginalCodeFromIL нужен, когда тестируемый метод встроен компилятором в вызывающий (см. раздел «Известные ограничения мок-платформы» в файле [mock-platform.md](mock-platform.md#известные-ограничения-мок-платформы)); восстанавливать нужно каждый встроенный метод цепочки — в тесте OutgoingTaxInvoice.CreateSellerTitleDialog это сам метод и вложенные CreateUtdSellerTitleDialog и CreateUcdSellerTitleDialog;
- NonPublicFunctionsHelper — не часть мок-платформы, а вспомогательный класс тестового проекта (UnitTests.Exchange.Helpers); в новом ModuleName.Unit.Tests его нужно завести самому;
- InputDialog.Current хранит последний показанный диалог и не сбрасывается автоматическим сбросом окружения, поэтому проверять его нужно сразу после вызова тестируемого метода.

## Пример 11. Диалог: выбор кнопки и обработчик кнопки

Поведение пользователя в диалоге задаётся обработчиком показа TaskDialog.ShowHandler: он получает показанный диалог и возвращает кнопку, которую «нажал» пользователь. Сам обработчик снимается автоматическим сбросом окружения.

```csharp
[Test]
[Description("Пользователь отменил диалог подписания — " +
  "документ не подписывается.")]
public void TrySignDocumentBeforeSendAnswerToCounterparty_UserCancelsDialog_DocumentNotSigned()
{
  // Arrange
  var signBeforeCalled = false;
  TaskDialog.ShowHandler = CancelSignDialog;
  this.MockSignBeforeSendDocumentToCounterparty(() =>
  {
    signBeforeCalled = true;
    return string.Empty;
  });
  var context = SendAnswerToCounterpartyDialogManager.CreateContext();
  var controls = this.CreateControls();

  // Act
  var result = this.CallTrySignDocumentBeforeSendAnswerToCounterparty(context, controls);

  // Assert
  Assert.Multiple(() =>
  {
    Assert.That(result, Is.False, "При отмене диалога должен возвращаться false.");
    Assert.That(signBeforeCalled, Is.False, "Подписание не должно вызываться при отмене.");
  });
}

/// <summary>
/// Отменить диалог подписания — вернуть последнюю кнопку диалога.
/// </summary>
private static DialogButton CancelSignDialog(TaskDialog dialog)
  => dialog.Buttons.Items.Last();
```

Обработчик кнопки — то, что прикладной код передаёт в dialog.SetOnButtonClick, — тестируется прямым вызовом. Аргументы собираются вручную, а результат валидации создаётся отдельно: тест держит на него ссылку и по ней проверяет добавленные ошибки.

```csharp
// Arrange.
var validationResult = new InputDialogValidationResult();
var args = new InputDialogButtonClickEventArgs(
  validationResult, new CustomDialogButton("Save"), false);

// Act.
this.CallValidateSellerTitleDialog(document, dialogProperties, controls, args);

// Assert.
Assert.Multiple(() =>
{
  Assert.That(validationResult.IsValid, Is.False,
    "После AddError результат валидации должен быть невалидным.");
  Assert.That(validationResult.Messages.Single().ToString(),
    Is.EqualTo(ValidationErrorText),
    "В результат валидации должен добавляться текст полученной ошибки.");
});
```

Методы диалога SetOnButtonClick и SetOnRefresh в мок-платформе не реализованы, а InputDialog.Show() всегда возвращает кнопку Ok. Поэтому обработчики диалога вызываются из теста напрямую с собранными вручную аргументами, а сценарий «пользователь нажал другую кнопку» воспроизводится только для TaskDialog через ShowHandler. Вызов SetOnButtonClick или SetOnRefresh бросает NotImplementedException, поэтому прикладной метод, который их использует, упадёт ещё до показа диалога.

## Пример 12. Асинхронный обработчик

Асинхронный обработчик вызывается напрямую: аргументы создаются через new и заполняются, обработчик — new ModuleAsyncHandlers(). Запрос на повторный запуск проверяется по самим аргументам — Retry и NextRetryTime.

```csharp
using MockPlatform.Core;
using MockPlatform.Logging;
using Sungero.Notifications;
using Sungero.Notifications.Server.AsyncHandlerInvokeArgs;
using Sungero.Notifications.Structures.Module;
using PublicFunctions = Sungero.Notifications.Server.ModuleServerPublicFunctions;
using ServerFunctions = Sungero.Notifications.Server.ModuleFunctions;

namespace UnitTests.Notifications.ModuleAsyncHandlers;

public class WatchNotificationEntry
{
  [Test]
  [Description("Сообщение ещё не доставлено — обработчик запрашивает повторный запуск.")]
  public void WatchNotificationEntry_MessageNotDelivered_RequestsRetry()
  {
    // Arrange
    var processResult = this.CreateProcessResult(
      Sungero.Notifications.Constants.NotificationEntry.MessageStatus.Sent, retry: true);
    MockFunctionStubs.Setup<ServerFunctions>(
      nameof(PublicFunctions.GetMessageProcessResult),
      (long _) => processResult);

    var functions = new ServerFunctions();
    var nextRetryTime = new DateTime(2026, 1, 1, 12, 0, 0);
    MockFunctionStubs.Setup<ServerFunctions>(
      nameof(functions.GetNextWatchNotificationEntryRetryTime),
      (IProcessResult _) => nextRetryTime);

    var args = new WatchNotificationEntryInvokeArgs
    {
      NotificationEntryId = 1,
      MessageId = "1"
    };

    // Act
    new Sungero.Notifications.Server.ModuleAsyncHandlers().WatchNotificationEntry(args);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(args.Retry, Is.True,
        "Недоставленное сообщение должно приводить к повторному запуску обработчика.");
      Assert.That(args.NextRetryTime, Is.EqualTo(nextRetryTime),
        "Время повторного запуска берётся из расчётной функции модуля.");
    });
  }
}
```

Если обработчик ничего не возвращает, проверяется его побочный эффект. Когда единственный наблюдаемый результат — запись в журнале, проверяются факт записи и её уровень:

```csharp
// Assert
Assert.That(NotificationEntries.GetAll(), Is.Empty,
  "Обработанная запись без хранения истории должна быть удалена.");
Assert.That(LogManager.GetMessages(), Has.Some.Contains("was deleted"),
  "Удаление записи должно быть зафиксировано в журнале.");
```

Что здесь существенно:

- аргументы вызова асинхронного обработчика собираются вручную — фабрики для них мок-платформа не предоставляет;
- повторный запуск проверяется по args.Retry, args.NextRetryTime и args.RetryIteration, а не по факту реального перезапуска: очередь в мок-платформе не работает;
- прикладной код, который сам ставит обработчик в очередь, закрывается стабом вида (AsyncHandlerInvokeArgs _) => { };
- каталог: Tests/ModuleAsyncHandlers/.

## Пример 13. Доступность действия

```csharp
Доступность действия проверяется тем же способом, что и его выполнение, только аргументы — CanExecuteActionArgs. Свойства сущности, которые нельзя задать присваиванием, подменяются через Mock.Get.
using Moq;
using Sungero.Docflow;
using Sungero.Docflow.Client;
using Sungero.Domain.Client;
using Sungero.Domain.Shared;

namespace UnitTests.Docflow.EntityActions.OfficialDocument;

public class CanShowEntityAIProcessingInfo
{
  [Test]
  [Description("Сохранённый документ с версией — действие доступно.")]
  public void CanShowEntityAIProcessingInfo_SavedDocumentWithVersion_ActionIsAvailable()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    Mock.Get(document).Setup(x => x.State.IsInserted).Returns(false);
    Mock.Get(document).Setup(x => x.HasVersions).Returns(true);

    // Act
    var result = CanExecute(document);

    // Assert
    Assert.That(result, Is.True, "Действие должно быть доступно.");
  }

  [Test]
  [Description("Документ создан, но не сохранён — " +
    "действие недоступно.")]
  public void CanShowEntityAIProcessingInfo_JustCreatedDocument_ActionIsNotAvailable()
  {
    // Arrange
    var document = OfficialDocuments.Create();
    Mock.Get(document).Setup(x => x.State.IsInserted).Returns(true);
    Mock.Get(document).Setup(x => x.HasVersions).Returns(true);

    // Act
    var result = CanExecute(document);

    // Assert
    Assert.That(result, Is.False, "Действие не должно быть доступно.");
  }

  #region Helper методы

  /// <summary>
  /// Проверить доступность действия «Показать результат проверки генеративным ИИ».
  /// </summary>
  /// <param name="document">Документ.</param>
  /// <returns>Признак доступности действия.</returns>
  private static bool CanExecute(IOfficialDocument document)
  {
    var actions = new OfficialDocumentActions();
    actions.SetEntity(document);
    var args = new CanExecuteActionArgs(FormType.Card, document);
    return actions.CanShowEntityAIProcessingInfo(args);
  }

  #endregion
}
```

Что здесь существенно:

- для проверки доступности создаются CanExecuteActionArgs, для выполнения — ExecuteActionArgs или ExecuteResultActionArgs (примеры 5 и 6);
- State.IsInserted и HasVersions присваиванием не задаются — это признаки состояния сущности, их подменяют через Mock.Get;
- вызов действия вынесен в приватный метод: тесты отличаются только подготовкой данных и ожидаемым результатом.

## Пример 14. Изолированная функция

Изолированные функции лежат в отдельной сборке Sungero.{Модуль}.Isolated.dll, типы которой конфликтуют с одноимёнными типами прикладного модуля. Поэтому сборка подключается к тестовому проекту под псевдонимом:

```xml
<Reference Include="$(AppliedModulesPath)\Sungero.GenAIProcessing.Isolated.dll">
  <Aliases>GenAIProcessingIsolated</Aliases>
</Reference>
```

Тест объявляет этот псевдоним и вызывает метод изолированной сборки рефлексией:

```csharp
extern alias GenAIProcessingIsolated;

using System.Reflection;
using GenAIProcessingIsolated::Sungero.GenAIProcessing.Isolated
  .AIProcessingReportGenerator;
using HarmonyLib;

namespace UnitTests.GenAIProcessing.Isolated.AIProcessingReportGenerator
  .PdfAIProcessingInfoGenerator;

public class ParseResponse
{
  [Test]
  [Description("Ответ ИИ не в формате JSON — разбор завершается ошибкой.")]
  public void ParseResponse_NotJsonResponse_ThrowsArgumentException()
  {
    // Arrange
    var data = "# Результат обработки";

    // Act
    // Assert
    Assert.Throws<ArgumentException>(() => CallMethod(data));
  }

  #region Helper методы

  private static ResponseDTO CallMethod(string response)
  {
    try
    {
      var method = AccessTools.Method(
        typeof(GenAIProcessingIsolated::Sungero.GenAIProcessing.Isolated
          .AIProcessingReportGenerator.ResponseProcessing),
        "ParseResponse",
        [typeof(string)]);

      return (ResponseDTO)method.Invoke(null, [response]);
    }
    catch (TargetInvocationException ex)
    {
      throw ex.InnerException ?? ex;
    }
  }

  #endregion
}
```

Что здесь существенно:

- изолированная сборка видна тесту только через псевдоним: без extern alias и квалификатора типы либо не разрешаются, либо совпадают по имени с типами прикладного модуля;
- TargetInvocationException разворачивается до InnerException — иначе Assert.Throws увидит обёртку рефлексии, а не ожидаемое исключение;
- если прикладной код уходит в изолированную область через супервизор, в тесте включается using var isolatedArea = IsolatedAreaStubs.Setup(); — тогда изолированный код выполняется в процессе теста;
- каталог: Tests/Isolated/{Область}/{Класс}/{Метод}.cs.
