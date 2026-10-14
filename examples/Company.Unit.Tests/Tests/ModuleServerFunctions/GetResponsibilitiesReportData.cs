using MockPlatform.Extensions;
using MockPlatform.Stubs;
using Moq;
using Sungero.Company;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using CompanyResources = Sungero.Company.Resources;
using ModuleFunctions = Sungero.Company.Server.ModuleFunctions;

namespace UnitTests.Company.ModuleServerFunctions
{
  /// <summary>
  /// Юнит-тесты для GetResponsibilitiesReportData.
  /// </summary>
  [TestFixture]
  public class GetResponsibilitiesReportData
  {
    private const string ComputedRoleName = "Вычисляемая роль 1";

    private const long DefaultEmployeeId = 1;

    private const long OtherEmployeeId = 2;

    /// <summary>
    /// Зарегистрировать репозиторий вычисляемых ролей в mock-контейнере.
    /// </summary>
    /// <remarks>
    /// Интерфейс IComputedRole не попадает в набор типов, для которых ModuleManager регистрирует
    /// mock-репозиторий автоматически, поэтому без ручной регистрации любое обращение к ComputedRoles
    /// (Create/GetAll/AccessRights) падает с ошибкой инициализации Repository<IComputedRole>;.
    /// </remarks>
    [OneTimeSetUp]
    public void RegisterComputedRolesRepository()
    {
      ModuleManager.Instance.Container.RegisterMockRepository(typeof(IComputedRole));
    }

    [Test]
    [Description("Есть права на справочник вычисляемых ролей и сотрудник входит в действующую вычисляемую роль — роль попадает в данные отчёта.")]
    public void GetResponsibilitiesReportData_ComputedRoleWithEmployee_AddedToReport()
    {
      // Arrange.
      var employee = CreateEmployee();
      var computedRole = CreateActiveComputedRole(ComputedRoleName, employee);
      Mock.Get(computedRole).Setup(role => role.ContainsRecipient(employee)).Returns(true);
      StubSectionsAccessRights(canRead: true);

      // Act.
      var result = new ModuleFunctions().GetResponsibilitiesReportData(employee);

      // Assert.
      var computedRoleLines = result
        .Where(line => line.Responsibility == CompanyResources.ComputedRoles)
        .ToList();
      Assert.Multiple(() =>
      {
        Assert.That(computedRoleLines, Is.Not.Empty,
                    "Секция «Вычисляемые роли» должна присутствовать в данных отчёта.");
        Assert.That(computedRoleLines.Any(line => line.Record == ComputedRoleName), Is.True,
                    "В данных отчёта должна быть строка с вычисляемой ролью, в которую входит сотрудник.");
      });
    }

    [Test]
    [Description("Сотрудник не входит в вычисляемую роль — роль не попадает в данные отчёта.")]
    public void GetResponsibilitiesReportData_ComputedRoleWithoutEmployee_NotAddedToReport()
    {
      // Arrange.
      var employee = CreateEmployee();
      var computedRole = CreateActiveComputedRole(ComputedRoleName, employee);
      Mock.Get(computedRole).Setup(role => role.ContainsRecipient(employee)).Returns(false);
      StubSectionsAccessRights(canRead: true);

      // Act.
      var result = new ModuleFunctions().GetResponsibilitiesReportData(employee);

      // Assert.
      var computedRoleLines = result
        .Where(line => line.Responsibility == CompanyResources.ComputedRoles)
        .ToList();
      Assert.Multiple(() =>
      {
        Assert.That(computedRoleLines, Is.Not.Empty,
                    "Секция «Вычисляемые роли» должна присутствовать в данных отчёта.");
        Assert.That(computedRoleLines.Any(line => line.Record == ComputedRoleName), Is.False,
                    "Вычисляемая роль без сотрудника не должна попадать в данные отчёта.");
      });
    }

    [Test]
    [Description("В выражении роли нет сотрудника — роль отсеивается предфильтром и не попадает в данные отчёта.")]
    public void GetResponsibilitiesReportData_ComputedRoleExpressionWithoutEmployee_FilteredOut()
    {
      // Arrange.
      var employee = CreateEmployee();
      var otherEmployee = CreateEmployee(id: OtherEmployeeId);
      var computedRole = CreateActiveComputedRole(ComputedRoleName, otherEmployee);

      // Если предфильтр по выражению не сработает, роль дойдёт до ContainsRecipient и попадёт в отчёт — тест это поймает.
      Mock.Get(computedRole).Setup(role => role.ContainsRecipient(employee)).Returns(true);
      StubSectionsAccessRights(canRead: true);

      // Act.
      var result = new ModuleFunctions().GetResponsibilitiesReportData(employee);

      // Assert.
      Assert.That(result.Any(line => line.Record == ComputedRoleName), Is.False,
                  "Роль, в выражении которой нет сотрудника, не должна попадать в данные отчёта.");
    }

    [Test]
    [Description("Нет прав на справочник вычисляемых ролей — секция «Вычисляемые роли» не формируется, ошибок нет.")]
    public void GetResponsibilitiesReportData_NoAccessToComputedRoles_SectionOmitted()
    {
      // Arrange.
      var employee = CreateEmployee();
      var computedRole = CreateActiveComputedRole(ComputedRoleName, employee);
      Mock.Get(computedRole).Setup(role => role.ContainsRecipient(employee)).Returns(true);
      StubSectionsAccessRights(canRead: false);

      // Act.
      var result = new ModuleFunctions().GetResponsibilitiesReportData(employee);

      // Assert.
      var hasComputedRolesSection = result.Any(line => line.Responsibility == CompanyResources.ComputedRoles);
      Assert.That(hasComputedRolesSection, Is.False,
                  "Без прав на справочник вычисляемых ролей секция «Вычисляемые роли» не должна формироваться.");
    }

    #region Helper методы

    /// <summary>
    /// Создать сотрудника для теста.
    /// </summary>
    /// <param name="id">Идентификатор сотрудника.</param>
    /// <returns>Сотрудник.</returns>
    private static IEmployee CreateEmployee(long id = DefaultEmployeeId)
    {
      var employee = Employees.Create();
      employee.SetId(id);
      return employee;
    }

    /// <summary>
    /// Создать действующую вычисляемую роль.
    /// </summary>
    /// <param name="name">Имя роли (используется и как отображаемое значение).</param>
    /// <param name="recipient">Получатель, явно включённый в состав роли.</param>
    /// <returns>Вычисляемая роль.</returns>
    private static IComputedRole CreateActiveComputedRole(string name, IEmployee recipient)
    {
      var computedRole = ComputedRoles.Create();
      computedRole.Name = name;
      computedRole.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
      computedRole.Expression = BuildExpressionWithRecipient(recipient);
      Mock.Get(computedRole).Setup(role => role.DisplayValue).Returns(name);
      return computedRole;
    }

    /// <summary>
    /// Построить выражение вычисляемой роли, в состав которой явно включён получатель.
    /// </summary>
    /// <param name="recipient">Получатель, включаемый в состав роли.</param>
    /// <returns>строку expression.</returns>
    private static string BuildExpressionWithRecipient(IEmployee recipient)
    {
      return string.Format("\"Tokens\":[{{\"Id\":{0},\"TypeId\":\"{1}\"}}]",
                           recipient.Id,
                           recipient.TypeDiscriminator.GetOriginalTypeGuid());
    }

    /// <summary>
    /// Застабить права на чтение для всех справочников-секций отчёта единым значением.
    /// </summary>
    /// <param name="canRead">Право на чтение.</param>
    /// <remarks>
    /// Мок-репозиторий по умолчанию бросает NotImplementedException при обращении к AccessRights,
    /// поэтому патчим геттеры всех задействованных методом репозиториев.
    /// </remarks>
    private static void StubSectionsAccessRights(bool canRead)
    {
      StubCanRead<IDepartmentAccessRights>(typeof(Departments), canRead);
      StubCanRead<IBusinessUnitAccessRights>(typeof(BusinessUnits), canRead);
      StubCanRead<IRoleAccessRights>(typeof(Roles), canRead);
      StubCanRead<IComputedRoleAccessRights>(typeof(ComputedRoles), canRead);
      StubCanRead<IManagersAssistantAccessRights>(typeof(ManagersAssistants), canRead);
      StubCanRead<ISubstitutionAccessRights>(typeof(Substitutions), canRead);
    }

    /// <summary>
    /// Застабить право на чтение справочника.
    /// </summary>
    /// <typeparam name="TAccessRights">Тип прав доступа справочника.</typeparam>
    /// <param name="repositoryType">Тип репозитория справочника.</param>
    /// <param name="canRead">Право на чтение.</param>
    private static void StubCanRead<TAccessRights>(Type repositoryType, bool canRead)
      where TAccessRights : class, IEntityAccessRights
    {
      RepositoryAccessRightsStubs.Setup<TAccessRights>(repositoryType,
        mock => mock.Setup(accessRights => accessRights.CanRead()).Returns(canRead));
    }

    #endregion
  }
}
