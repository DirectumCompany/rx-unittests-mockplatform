# Мок-платформа: юнит-тесты

## Мок-платформа

Прикладной код Directum RX нельзя выполнить в тесте напрямую: сущности представлены генерируемыми платформой интерфейсами и прокси-типами, репозитории обращаются к базе данных, а значительная часть API платформы — статические классы. Поэтому изоляция обеспечивается собственной мок-платформой.

Мок-платформа подменяет:

- репозитории сущностей — реализацией в памяти, поэтому Employees.Create() работает без базы данных;
- сами сущности — прокси-объектами с настроенными свойствами, дочерними коллекциями, состоянием и метаданными;
- сервисы платформы — диалоги, гиперссылки, блокировки, лицензии, подписи, хранилище, метаданные, запуск задач и асинхронных обработчиков;
- произвольные методы прикладного кода и статические члены платформы — через подмену кода методов библиотекой Harmony.

## Инициализация проекта и изоляция тестов

В каждом юнит-тестовом проекте обязателен файл Environment. Он регистрирует модули прикладного решения и включает автоматический сброс мок-окружения:

```csharp
using MockPlatform;

[assembly: ResetMockEnvironment]

namespace UnitTests
{
  [SetUpFixture]
  public class ModuleNameEnvironment
  {
    [OneTimeSetUp]
    public static void Initialize() => ModuleManager.RegisterModules("Sungero.Company", "Sungero.Docflow");
  }
}
```

Атрибут [assembly: ResetMockEnvironment] действует только в той сборке, в которой объявлен, поэтому добавляется в каждый проект отдельно. Перед каждым тестом он очищает репозитории сущностей, снимает все стабы функций, очищает журнал, сбрасывает текущего пользователя, открытые гиперссылки, последний показанный диалог и зарегистрированные инициализаторы типов.

Следствия:

- писать [SetUp] с очисткой репозиториев или стабов не нужно — это дублирование автоматического сброса;
- [SetUp] и [TearDown] добавляются только для того, чего сброс не делает: общие для класса стабы, регистрация инициализаторов типов, освобождение IDisposable-ресурсов;
- тесты не полагаются на данные, созданные другими тестами, и не зависят от порядка выполнения.

## Именование

- Класс теста называется именем тестируемого метода, без суффиксов Tests и Fixture.
- Тестовый метод: {Метод}_{ПокрываемыйКейс}_{Результат}; полная форма предпочтительна, краткая форма {Метод}_{ПокрываемыйКейс} допустима, только если результат однозначно следует из кейса. Префикс с именем тестируемого метода обязателен. Правила формулировки кейса и результата — в подразделе «Описание кейса в имени метода и [Description]».
- Переменные именуются описательно; однобуквенные имена допустимы только в лямбда-выражениях LINQ.
- Магические числа и строки выносятся в именованные константы.

Примеры:

```csharp
LoginIsAvailable_UniqueLogin_ReturnsTrue

CheckPsrnLength_AlphanumericPsrn_ReturnsErrorMessage

GetEmployeesByPerson_OnlyClosedEmployee_ReturnsEmpty
```

## Атрибуты и документирование

- [Test] — на каждом тестовом методе.
- [Description("…")] на русском языке — обязателен, описывает покрываемый кейс в формате «{Условие} — {Ожидаемый результат}».
- XML-документация над тестовым методом не пишется — кейс описан в [Description]. Для вспомогательных методов и для класса, требующего пояснения, XML-документация обязательна.
- [TestCase] и [TestCaseSource] — для однотипных тестов, отличающихся только входными данными. Если внутри теста появляются условия, зависящие от значений параметров, тест разбивается на отдельные методы.
- [TestFixture] не требуется, но в рамках одного проекта применяется единообразно.

## Описание кейса в имени метода и [Description]

У юнит-теста нет родительского тест-кейса, поэтому имя тестового метода и [Description] — единственное описание покрываемого кейса. Они формулируются так, чтобы по списку тестов (Test Explorer, отчёт прогона) можно было оценить фактическое покрытие метода, не читая код тестов:

- [Description] строится по формату «{При каком эквивалентном условии} — {Ожидаемый результат}». Условие опускается, если у метода нет ветвления или результат от входных данных не зависит;
- Ожидаемый результат один — основной и обобщённый, без перечисления всех проверяемых значений. Детали результата раскрывает состав ассертов;
- Слова «Проверка», «Проверить», «Тест» в описании не используются: каждый тест по определению — проверка, и такие слова не несут информации о кейсе;
- Контекст, который уже задан расположением теста, не повторяется: модуль, сущность и тип функции видны из namespace, тестируемый метод — из имени класса;
- Имя метода и [Description] описывают один и тот же кейс: {ПокрываемыйКейс} соответствует условию, {Результат} — ожидаемому результату.

Пример. Не рекомендуется:

```csharp
[Description("Проверка недоступности логина, когда логин используется в другом ящике с тем же сервисом обмена.")]
```

Рекомендуется:

```csharp
[Description("Логин занят другим ящиком того же сервиса обмена — логин недоступен.")]
public void LoginIsAvailable_DuplicateLogin_ReturnsFalse()
```

## Шаблон Arrange-Act-Assert

Юнит-тесты пишутся по паттерну AAA. Паттерн Given-When-Then, применяемый в функциональных и интеграционных тестах, в юнит-тестах не используется.

- Тело теста разделено маркерами // Arrange, // Act, // Assert.
- Между секциями — одна пустая строка.
- Маркер ставится, даже если секция пустая.
- Содержимое секций соответствует назначению: подготовка данных и стабов, вызов тестируемого метода, проверки. Ассерты в секции Act не допускаются.

```csharp
[Test]
[Description("Логин не используется в других ящиках — логин доступен.")]
public void LoginIsAvailable_UniqueLogin_ReturnsTrue()
{
  // Arrange
  var currentBox = BusinessUnitBoxes.Create();
  currentBox.Login = "current_login";

  // Act
  var result = currentBox.GetServerFunctions<BusinessUnitBoxFunctions>().LoginIsAvailable("new_login");

  // Assert
  Assert.That(result, Is.True);
}
```

## Переиспользование кода

- Громоздкая и повторяющаяся подготовка данных выносится в приватные вспомогательные методы того же класса, собранные после тестовых методов в блок #region Helper методы / #endregion.
- Код, нужный нескольким тестовым классам, разделяется по роли: создание и настройка сущностей — в Managers/{Сущность}Manager.cs, процессы и операции над уже созданными сущностями и общие приёмы работы с мок-платформой — в Helpers/{Процесс}Helper.cs.
- Создавать сущности в Helper нельзя — для этого есть Manager.
- Обращаться к вспомогательным методам чужого тестового класса нельзя.
- Базовые классы тестов не используются.
- Вспомогательные классы обязаны быть stateless: никаких изменяемых статических полей, кэшей, счётчиков и флагов «уже инициализировано». Разрешены только неизменяемые данные — const и readonly.
- Тестовый метод должен читаться целиком, ориентир — 20–30 строк. Разрастание теста означает, что в нём проверяется слишком много сценариев либо что подготовку данных пора вынести во вспомогательный метод.

## Ассерты

- Только constraint-модель NUnit: Assert.That(…) с Is.*, Has.*, Does.*, Throws.*.
- Устаревшие формы Assert.AreEqual, Assert.IsTrue, Assert.IsFalse, Assert.IsNull, Assert.IsNotNull запрещены.
- К ассерту добавляется поясняющее сообщение, если оно помогает понять суть падения.
- Связанные проверки одного результата группируются в Assert.Multiple — тогда падение первой проверки не скрывает остальные.
- Исключения проверяются через Assert.Throws<T> или Assert.That(() => …, Throws.TypeOf<T>()), а не через try/catch с ручной проверкой.
- Ожидаемые тексты сообщений берутся из ресурсов модуля, а не дублируются строковым литералом.
- Тестов без ассертов быть не должно.
- Набор ассертов покрывает все наблюдаемые результаты кейса: возвращаемое значение, исключения, побочные эффекты.

Не следует сравнивать результат целиком с длинной локализованной строкой, содержащей дату или идентификатор, и проверять тексты и порядок записей журнала. Такие тесты проверяют реализацию, а не поведение, и ломаются от любой правки ресурса или формата вывода.

## Рецепты мок-платформы

### Создание сущностей

Сущности создаются только через репозиторий соответствующего типа. Создавать их через new или Mock.Of<T>() нельзя — такая сущность не попадёт в репозиторий и не будет найдена тестируемым кодом. Через new допустимо создавать только объекты сторонних библиотек.

```csharp
var businessUnit = BusinessUnits.Create();
businessUnit.TIN = "1234567890";

var employee = Employees.Create();
// Установка ИД сущности с помощью хелпера мок-платформы.
employee.SetId(1);

var counterparty = Sungero.Parties.Counterparties.Create();
counterparty.Name = "Тестовый контрагент";
```

### Вызов тестируемой функции

```csharp
// Экземплярная функция сущности — серверная, общая или клиентская.
// _obj = entity внутри кода метода LoginIsAvailable().
var result = entity.GetServerFunctions<BusinessUnitBoxFunctions>().LoginIsAvailable("new_login");
entity.GetSharedFunctions<CaseFileFunctions>().SetRequiredProperties(); // Метод ничего не возвращает.
var client = entity.GetClientFunctions<CounterpartyFunctions>().ValidateExchangeAction(args);

// Публичная модульная функция.
var boxes = new Sungero.ExchangeCore.Server.ModuleFunctions()
  .GetCounterpartyDepartmentBoxes(counterparty, box, "branch123");

// Статическая публичная функция сущности.
var units = Sungero.Company.PublicFunctions.BusinessUnit.GetBusinessUnits(tin, trrc);

// Remote-функция модуля или сущности — вызывается так же, как обычная публичная.
var isAvailable = Sungero.Docflow.PublicFunctions.Module.Remote
  .IsModuleAvailableByLicense(moduleGuid);

// Непубличная модульная функция — через рефлексию.
var functions = new Sungero.Exchange.Server.ModuleFunctions();
var method = functions.GetType().GetMethod("SendServiceDocument",
  BindingFlags.NonPublic | BindingFlags.Instance);
Assert.That(method, Is.Not.Null, "Не найден метод SendServiceDocument.");
method.Invoke(functions, new object[] { signedDocuments, box, reglamentDocumentType });
```

Рефлексию для непубличных методов выносят в приватный вспомогательный метод класса или в общий {Метод}ReflectionHelper. Проверка найденного метода на null обязательна: без неё переименование метода в прикладном коде превратится в неинформативное падение. Набор аргументов Invoke должен точно повторять сигнатуру метода: SendServiceDocument принимает три параметра и ничего не возвращает, поэтому результат вызова не присваивается.

### Стабы функций

Стаб подменяет реализацию конкретного метода на время теста. Стаб оборачивается в using var — тогда он снимается на выходе из области видимости; кроме того, все стабы снимаются автоматическим сбросом перед следующим тестом. Параметры лямбды типизируются явно: без этого нужная перегрузка не находится. Реестр стабов ключуется именем метода, поэтому в одном тесте допустим только один стаб на каждое имя: два стаба одноимённых методов разных классов конфликтуют — работает первый, а Dispose() второго снимает регистрацию первого.

```csharp
// Модульная функция с возвращаемым значением.
using var idsStub = MockFunctionStubs.Setup<Sungero.Company.Shared.ModuleFunctions>(
  "SplitToIds",
  (string idsString, string[] separators) => new List<long>());

// Модульная функция без возвращаемого значения.
using var substitutionStub = MockFunctionStubs.Setup<Sungero.Company.Server.ModuleFunctions>(
  "CreateSystemSubstitution",
  (IUser substitutedUser, IUser substitute) => { });

// Экземплярная функция сущности: первым параметром лямбды приходит сама сущность.
using var boxStub = MockFunctionStubs.SetupEntity<Sungero.ExchangeCore.Shared.BusinessUnitBoxFunctions>(
  "CheckBusinessUnitBoxActive",
  (IBusinessUnitBox _obj) => "123");

// Стаб, выбрасывающий исключение.
using var licenseStub = MockFunctionStubs.SetupEntity<Sungero.ExchangeCore.Server.BusinessUnitBoxFunctions>(
  "CheckExchangeConnectorLicense",
  (IBusinessUnitBox _obj) => { throw new Exception(); });

// Статический член платформы — фиксация времени.
MockFunctionStubs.Setup(typeof(Calendar), "get_Now", () => new DateTime(2026, 1, 1, 12, 0, 0));
MockFunctionStubs.Setup(typeof(Calendar), "get_Today", () => new DateTime(2026, 1, 1));
```

### Свойства и состояние сущности

Реквизиты мок-сущности задаются присваиванием. Свойства, которым присвоить значение нельзя — вычисляемые, служебные и признаки состояния, — подменяются через Mock.Get.

```csharp
// Свойства, которые нельзя присвоить.
Mock.Get(document).Setup(x => x.State.IsInserted).Returns(false);
Mock.Get(document).Setup(x => x.HasVersions).Returns(true);
Mock.Get(role).Setup(x => x.State.IsChanged).Returns(true);

// Права доступа конкретной сущности.
Mock.Get(document).Setup(x => x.AccessRights.CanUpdate()).Returns(true);
```

Если тестируемый код читает State и Info целиком — доступность, видимость и обязательность реквизитов, — вместо точечных подмен поднимается инициализатор типа (см. пример 7 в файле [unit-test-examples.md](unit-test-examples.md#пример-7-состояние-и-метаданные-сущности)).

### Текущий пользователь и права доступа

```csharp
// Текущий пользователь на время теста.
var employee = Employees.Create();
using var currentUser = MockCurrentUser.Set(employee);

// Права доступа на репозиторий.
RepositoryAccessRightsStubs.Setup<IUserBaseAccessRights>(typeof(UserBases),
  mock => mock.Setup(ar => ar.CanUpdate()).Returns(true));

// Права репозитория и строгий режим доступа конкретной сущности.
using var departmentRights = RepositoryAccessRightsStubs.Setup<IDepartmentAccessRights>(
  typeof(Departments), mock => mock.Setup(ar => ar.CanRead()).Returns(true));
using var strictMode = RepositoryAccessRightsStubs.Setup<ISimpleDocumentAccessRights>(
  document.GetType(),
  mock => mock.Setup(ar => ar.StrictMode).Returns(Sungero.Core.AccessRightsStrictMode.None));
```

Права доступа у мок-сущностей и мок-репозиториев не настроены: без подмены обращение к AccessRights падает. Подменять нужно тот тип, через который к правам обращается тестируемый код: для прав на тип сущности — репозиторий (typeof(Departments)), для прав на конкретную сущность — её фактический тип (document.GetType()). Возвращаемый IDisposable снимает подмену на выходе из области видимости. В одном тесте можно подменить права только одного репозитория: второй вызов RepositoryAccessRightsStubs.Setup для другого репозитория молча не применяется — возвращается мок первой подмены. Пара «репозиторий + конкретная сущность через document.GetType()» работает.

### Диалоги и уведомления

Вызовы Dialogs.NotifyMessage и Dialogs.ShowMessage перехватываются мок-платформой: последний показанный диалог доступен через TaskDialog.Current.

```csharp
using MockPlatform.Services.Dialogs;

// Проверить, что уведомление было показано с нужным текстом.
Assert.That(TaskDialog.Current, Is.Not.Null);
Assert.That(TaskDialog.Current!.Text, Is.EqualTo(Sungero.Exchange.Resources.SomeResource.ToString()));

// Проверить, что уведомление НЕ показывалось: зафиксировать состояние до вызова.
var dialogBefore = TaskDialog.Current;
// ... Act ...
Assert.That(TaskDialog.Current, Is.SameAs(dialogBefore));

// Диалог, показанный прикладным кодом, и его контролы.
Assert.That(InputDialog.Current, Is.Not.Null);
var control = (DropDownDialogValue)InputDialog.Current.Controls[0];

// Кнопка, которую «нажал» пользователь.
TaskDialog.ShowHandler = dialog => dialog.Buttons.Items.Last();
```

Подробнее — примеры 10 и 11 в файле [unit-test-examples.md](unit-test-examples.md#пример-10-диалог-сборка-контролы-и-заголовки).

### Прочие проверки

```csharp
// Ошибки, добавленные через e.AddError(...).
var messages = entity.GetValidationMessages();
Assert.That(messages, Has.Count.EqualTo(1));
Assert.That(messages[0].MessageType, Is.EqualTo(ValidationMessageType.Error));

// Отмена действия.
Assert.Throws<OperationCanceledException>(() => ExecuteReject(assignment));

// Открытая гиперссылка и запущенные задачи.
Assert.That(HyperlinksImplementer.OpenedUri, Is.EqualTo(expectedUri));
Assert.That(SimpleTaskExtensions.GetStartedTasks(), Has.Count.EqualTo(1));

// Донастройка любой сущности, которая будет создана внутри тестируемого кода.
using var _ = MockInstanceInitializer.AddTypeInitializer<ISimpleTask>(
  inst => inst.Setup(x => x.Info.Properties.Subject.Length).Returns(200));
```

### Журнал

Записи журнала доступны через LogManager мок-платформы; журнал очищается автоматическим сбросом перед каждым тестом.

```csharp
using MockPlatform.Logging;

// Факт записи — по фрагменту сообщения.
Assert.That(LogManager.GetMessages(),
  Has.Some.Contains($"Power of attorney with Id {id} is locked"),
  "Пропуск заблокированной доверенности должен быть записан в журнал.");

// Записи заданного уровня.
var errors = LogManager.GetLogsByLevel(NLog.LogLevel.Error);
Assert.That(errors, Is.Empty, "Обработка не должна приводить к ошибкам в журнале.");
```

Журнал проверяется только тогда, когда он — единственный наблюдаемый результат метода. Сверяются факт записи и её уровень, но не полный текст сообщения и не порядок записей (см. «Антипаттерны»).

## Известные ограничения мок-платформы

- Метод e.Validate() у мок-сущностей всегда возвращает true, так как список валидаторов пуст. Ветку if (!e.Validate()) return; покрывают только подменой самого Validate(). Стаб действует глобально на весь тест, поэтому включается только в тесте, проверяющем провал валидации.
- Методы репозитория Copy, Union, GetCached и GetAllCached с условием не реализованы — если тестируемый код их использует, их подменяют стабом. GetAllCached() без условия работает и сводится к GetAll().
- Получение локализованного значения перечисления через Info.Properties.X.GetLocalizedValue() требует ручной подмены метаданных: без неё вызов не падает, а молча возвращает null, поэтому тест может пройти, ничего не проверив.
- TaskDialog.Current хранит последний показанный диалог и не сбрасывается внутри одного теста, поэтому проверять его нужно сразу после вызова тестируемого метода.
- Встраивание методов компилятором может «съесть» стаб вложенного вызова; в этом случае исходный код метода восстанавливается принудительно средствами мок-платформы.
- В диалоге мок-платформы реализована часть контролов: строка, многострочная строка, признак, выпадающий список и выбор сущностей. AddDate, AddInteger, AddDouble, AddIdentifier, AddHyperlink, AddFileSelect, AddFileSelectMany, AddPasswordString и AddProgressBar не реализованы. Не реализованы также TaskDialog.AddHyperlink, Dialogs.CreateConfirmDialog, CreateSelectTypeDialog, CreateSearchDialog, CreateFileSelectionDialog, CreateFileManySelectionDialog, CreateCustomDialog, CreateCompositeCustomDialog и свойство Info у репозитория.
- Методы диалога SetOnButtonClick и SetOnRefresh не реализованы — они бросают NotImplementedException, а InputDialog.Show() всегда возвращает кнопку Ok. Обработчики диалога вызываются из теста напрямую, а выбор кнопки пользователем воспроизводится только для TaskDialog через ShowHandler (см. пример 11 в файле [unit-test-examples.md](unit-test-examples.md#пример-11-диалог-выбор-кнопки-и-обработчик-кнопки)). Приём «взять InputDialog.Current после вызова тестируемого метода» применим только к диалогам, которые не регистрируют обработчик кнопки: иначе тестируемый метод упадёт до показа диалога.
- InputDialog.Current, в отличие от TaskDialog.Current, не сбрасывается автоматическим сбросом окружения — показанный диалог проверяется сразу после вызова тестируемого метода.
- Права доступа мок-сущностей и мок-репозиториев не настроены: обращение к AccessRights без подмены падает.
- Постановка асинхронного обработчика в очередь не выполняется — исполнитель асинхронных обработчиков в мок-платформе является заглушкой. Сам обработчик тестируется прямым вызовом (см. пример 12 в файле [unit-test-examples.md](unit-test-examples.md#пример-12-асинхронный-обработчик)).

Подмена валидации формы:

```csharp
private void MockValidate(bool isValid)
{
  MockFunctionStubs.Setup<Sungero.Domain.Client.BaseExecuteActionArgs>(
    nameof(Sungero.Domain.Client.BaseExecuteActionArgs.Validate),
    () => isValid);
}
```

## Антипаттерны

Перечисленное ниже встречается в существующих тестах и не должно тиражироваться.

- Устаревшие формы ассертов — Assert.AreEqual, Assert.IsTrue, Assert.IsNotNull. Используется только Assert.That с матчерами.
- Сравнение результата целиком с длинной локализованной строкой, содержащей дату, ссылку или идентификатор. Такой тест ломается от правки ресурса или формата даты; сравнивать нужно с ресурсом или проверять значимые фрагменты.
- Сверка полного текста и порядка записей журнала — это проверка реализации, а не поведения. Исключение — методы, у которых журнал является единственным наблюдаемым результатом (асинхронные обработчики, фоновые процессы): у них проверяются факт записи и её уровень, см. рецепт «Журнал».
- Неиспользуемые переменные в Arrange, за которыми скрывается побочный эффект регистрации сущности в репозитории. Если объект нужен как данные — это поясняется комментарием, если не нужен — удаляется.
- Копирование одинаковых блоков подготовки данных между тестами вместо вспомогательного метода.
- Очистка репозиториев и стабов в [SetUp] — дублирование автоматического сброса окружения.
- Использование DateTime.Now в тестовых данных и ассертах вместо фиксации времени стабом календаря.
- Имя тестового метода без префикса с именем тестируемого метода; namespace, не соответствующий типу функции.
- Общий базовый класс тестов с разделяемым состоянием. Переиспользование — только через Managers (создание сущностей) и Helpers (процессы и операции над ними).
- Использование мок-серверов с http запросами в юнит тестах.
- Перевод приватных методов в публичные только ради юнит-тестирования. Это нарушает один из фундаментальных принципов — проверять требования, а не реализацию. Приватные методы тестируются косвенно, как часть наблюдаемого поведения публичного метода.
- Загрязнение кода (code pollution) — добавление в рабочий код того, что нужно только для тестирования. В тестируемую функцию нельзя добавлять код, который нужен только для тестов.
- Ветвление if в тестах. Для каждой ветки пишется отдельный тест.
