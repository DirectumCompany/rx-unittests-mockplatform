using UnitTests.Company.Managers;

namespace UnitTests.Company.EmployeeServerFunctions
{
  /// <summary>
  /// Юнит тесты для функции GetEmployeesByName.
  /// Функция возвращает список активных сотрудников по имени.
  /// </summary>
  public class GetEmployeesByName
  {
    [Test]
    [Description("Передано полное ФИО сотрудника — возвращается этот сотрудник")]
    public void GetEmployeesByName_ExactFullName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(1, "Иванов", "Иван", "Иванович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Иванов Иван Иванович");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
        Assert.That(result.First().Name, Is.EqualTo("Иванов Иван Иванович"));
      });
    }

    [Test]
    [Description("Передана фамилия — возвращается сотрудник с этой фамилией")]
    public void GetEmployeesByName_LastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(2, "Петров", "Петр", "Петрович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Петров");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Фамилия И.О.» — возвращается сотрудник")]
    public void GetEmployeesByName_LastNameWithInitials_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(3, "Сидоров", "Сидор", "Сидорович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Сидоров С.С.");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «И.О. Фамилия» — возвращается сотрудник")]
    public void GetEmployeesByName_InitialsWithLastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(4, "Алексеев", "Алексей", "Алексеевич");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("А.А. Алексеев");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Сотрудника с переданным именем нет — возвращается пустой список")]
    public void GetEmployeesByName_UnknownName_ReturnsEmpty()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(5, "Иванов", "Иван");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Несуществующий");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
      });
    }

    [Test]
    [Description("Вместо имени передан null — возвращается null")]
    public void GetEmployeesByName_NullName_ReturnsNull()
    {
      // Arrange

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName(null);

      // Assert
      Assert.That(result, Is.Null);
    }

    [Test]
    [Description("Сотрудник с переданной фамилией закрыт — возвращается пустой список")]
    public void GetEmployeesByName_ClosedEmployee_ReturnsEmpty()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(6, "Закрытов", "Закрытый", status: Sungero.CoreEntities.DatabookEntry.Status.Closed);

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Закрытов");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
      });
    }

    [Test]
    [Description("Имя сотрудника с буквой «ё» передано через «е» — возвращается сотрудник")]
    public void GetEmployeesByName_YeInsteadOfYo_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(7, "Ёжиков", "Семён");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Ежиков Семен");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [TestCase("МИХАЙЛОВ")]
    [TestCase("михайлов")]
    [TestCase("МиХаЙлОв")]
    [Description("Фамилия передана в другом регистре — возвращается сотрудник")]
    public void GetEmployeesByName_DifferentCase_ReturnsEmployee(string name)
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(8, "Михайлов", "Михаил");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName(name);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
      });
    }

    [Test]
    [Description("Фамилия есть у нескольких сотрудников — возвращаются все эти сотрудники")]
    public void GetEmployeesByName_SharedLastName_ReturnsAllEmployees()
    {
      // Arrange
      var employee1 = EmployeeManager.CreateEmployee(9, "Смирнов", "Андрей");
      var employee2 = EmployeeManager.CreateEmployee(10, "Смирнов", "Дмитрий");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Смирнов");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Any(e => e.Id == employee1.Id), Is.True);
        Assert.That(result.Any(e => e.Id == employee2.Id), Is.True);
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Имя Фамилия» — возвращается сотрудник")]
    public void GetEmployeesByName_FirstNameLastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(11, "Николаев", "Николай");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Николай Николаев");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [TestCase("Викторов Виктор")]
    [TestCase("Викторов")]
    [Description("У сотрудника нет отчества, переданы фамилия и имя или только фамилия — возвращается сотрудник")]
    public void GetEmployeesByName_EmployeeWithoutMiddleName_ReturnsEmployee(string name)
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(12, "Викторов", "Виктор");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName(name);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передана пустая строка — возвращается пустой список")]
    public void GetEmployeesByName_EmptyName_ReturnsEmpty()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(13, "Тестов", "Тест");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName(string.Empty);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
      });
    }

    [Test]
    [Description("Передана строка из пробелов — возвращается пустой список")]
    public void GetEmployeesByName_WhitespaceName_ReturnsEmpty()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(14, "Романов", "Роман");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("   ");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Фамилия И О» — возвращается сотрудник")]
    public void GetEmployeesByName_LastNameWithInitialsWithoutDots_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(15, "Олегов", "Олег", "Олегович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Олегов О О");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Имя Отчество Фамилия» — возвращается сотрудник")]
    public void GetEmployeesByName_FirstNameMiddleNameLastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(16, "Игнатов", "Игорь", "Игоревич");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Игорь Игоревич Игнатов");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Фамилия И.» — возвращается сотрудник")]
    public void GetEmployeesByName_LastNameWithFirstInitial_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(17, "Константинов", "Константин", "Константинович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Константинов К.");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Части ФИО разделены двумя пробелами — возвращается сотрудник")]
    public void GetEmployeesByName_DoubleSpacesBetweenParts_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(18, "Павлов", "Павел", "Павлович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Павлов  Павел  Павлович");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано полное ФИО, есть однофамильцы и другие сотрудники — возвращается только сотрудник с совпадающим ФИО")]
    public void GetEmployeesByName_FullNameAmongSeveralEmployees_ReturnsOnlyMatching()
    {
      // Arrange
      // Сотрудник 1 - целевой
      var employee1 = EmployeeManager.CreateEmployee(19, "Васильев", "Василий", "Васильевич");
      // Сотрудник 2 - другой Васильев
      var employee2 = EmployeeManager.CreateEmployee(20, "Васильев", "Петр", "Иванович");
      // Сотрудник 3 - совсем другой
      var employee3 = EmployeeManager.CreateEmployee(21, "Иванов", "Иван");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Васильев Василий Васильевич");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee1.Id));
        Assert.That(result.First().Name, Is.EqualTo("Васильев Василий Васильевич"));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Фамилия И.О.», у однофамильцев разные инициалы — возвращаются только сотрудники с совпадающими инициалами")]
    public void GetEmployeesByName_InitialsAmongNamesakes_ReturnsOnlyMatching()
    {
      // Arrange
      var employee1 = EmployeeManager.CreateEmployee(22, "Антонова", "Анна", "Андреевна");
      var employee2 = EmployeeManager.CreateEmployee(23, "Антонова", "Борис", "Борисович");
      var employee3 = EmployeeManager.CreateEmployee(24, "Антонова", "Александр", "Алексеевич");

      // Act - ищем по инициалам А.А.
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Антонова А.А.");

      // Assert - должны вернуться employee1 (Анна Андреевна) и employee3 (Александр Алексеевич)
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Any(e => e.Id == employee1.Id), Is.True);
        Assert.That(result.Any(e => e.Id == employee3.Id), Is.True);
        Assert.That(result.Any(e => e.Id == employee2.Id), Is.False);
      });
    }

    [Test]
    [Description("Есть активный и закрытый сотрудники с одинаковым ФИО — возвращается только активный сотрудник")]
    public void GetEmployeesByName_ActiveAndClosedNamesakes_ReturnsOnlyActive()
    {
      // Arrange
      var employee1 = EmployeeManager.CreateEmployee(25, "Дмитриев", "Дмитрий");
      var employee2 = EmployeeManager.CreateEmployee(26, "Дмитриев", "Дмитрий", status: Sungero.CoreEntities.DatabookEntry.Status.Closed);

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Дмитриев Дмитрий");

      // Assert - должен вернуться только активный сотрудник
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee1.Id));
        Assert.That(result.First().Status, Is.EqualTo(Sungero.CoreEntities.DatabookEntry.Status.Active));
      });
    }

    [Test]
    [Description("Передана фамилия, есть сотрудники с похожими фамилиями — возвращается только сотрудник с точно совпадающей фамилией")]
    public void GetEmployeesByName_SimilarLastNames_ReturnsOnlyExactLastName()
    {
      // Arrange
      var employee1 = EmployeeManager.CreateEmployee(28, "Евгеньев", "Евгений");
      var employee2 = EmployeeManager.CreateEmployee(29, "Евгенов", "Евгений");
      var employee3 = EmployeeManager.CreateEmployee(30, "Евгеньева", "Евгения");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Евгеньев");

      // Assert - должен вернуться только employee1
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee1.Id));
        Assert.That(result.First().Person.LastName, Is.EqualTo("Евгеньев"));
      });
    }

    [Test]
    [Description("Передано полное ФИО, у однофамильцев отличаются имя или отчество — возвращается только сотрудник с совпадающими именем и отчеством")]
    public void GetEmployeesByName_NamesakesWithDifferentFirstOrMiddleName_ReturnsOnlyMatching()
    {
      // Arrange
      var employee1 = EmployeeManager.CreateEmployee(31, "Федоров", "Федор", "Федорович");
      var employee2 = EmployeeManager.CreateEmployee(32, "Федоров", "Федор", "Петрович");
      var employee3 = EmployeeManager.CreateEmployee(33, "Федоров", "Петр", "Федорович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Федоров Федор Федорович");

      // Assert - должен вернуться только employee1
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee1.Id));
        Assert.That(result.First().Person.FirstName, Is.EqualTo("Федор"));
        Assert.That(result.First().Person.MiddleName, Is.EqualTo("Федорович"));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Имя Фамилия», есть сотрудники с похожими именами и фамилиями — возвращается только сотрудник с совпадающими именем и фамилией")]
    public void GetEmployeesByName_FirstNameLastNameAmongSimilarEmployees_ReturnsOnlyMatching()
    {
      // Arrange
      var employee1 = EmployeeManager.CreateEmployee(34, "Георгиев", "Георгий", "Геннадьевич");
      var employee2 = EmployeeManager.CreateEmployee(35, "Георгиев", "Геннадий", "Георгиевич");
      var employee3 = EmployeeManager.CreateEmployee(36, "Геннадьев", "Георгий");

      // Act - поиск по формату Имя Фамилия
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Георгий Георгиев");

      // Assert - должен вернуться только employee1 (Георгий Георгиев)
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee1.Id));
        Assert.That(result.First().Person.FirstName, Is.EqualTo("Георгий"));
        Assert.That(result.First().Person.LastName, Is.EqualTo("Георгиев"));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Фамилия И» (инициал имени без точки) — возвращается сотрудник")]
    public void GetEmployeesByName_LastNameWithFirstInitialWithoutDot_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(38, "Германов", "Герман", "Германович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Германов Г");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «Фамилия ИИ» (инициалы слитно без точек) — возвращается сотрудник")]
    public void GetEmployeesByName_LastNameWithGluedInitialsWithoutDots_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(39, "Глебов", "Глеб", "Григорьевич");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Глебов ГГ");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «И. Фамилия» — возвращается сотрудник")]
    public void GetEmployeesByName_FirstInitialWithLastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(40, "Давыдов", "Давид");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Д. Давыдов");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «И И Фамилия» — возвращается сотрудник")]
    public void GetEmployeesByName_InitialsWithoutDotsWithLastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(42, "Егоров", "Егор", "Евгеньевич");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Е Е Егоров");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «И Фамилия» — возвращается сотрудник")]
    public void GetEmployeesByName_FirstInitialWithoutDotWithLastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(43, "Захаров", "Захар");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("З Захаров");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Передано ФИО в формате «ИИ Фамилия» (инициалы слитно без точек) — возвращается сотрудник")]
    public void GetEmployeesByName_GluedInitialsWithoutDotsWithLastName_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(44, "Ильин", "Илья", "Иванович");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("ИИ Ильин");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [TestCase("Салтыков-Щедрин Михаил")]
    [TestCase("Салтыков-Щедрин")]
    [Description("Фамилия сотрудника через дефис, переданы фамилия и имя или только фамилия — возвращается сотрудник")]
    public void GetEmployeesByName_HyphenatedLastName_ReturnsEmployee(string name)
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(45, "Салтыков-Щедрин", "Михаил");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName(name);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("Фамилия сотрудника содержит апостроф — возвращается сотрудник")]
    public void GetEmployeesByName_LastNameWithApostrophe_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(46, "О'Коннор", "Патрик");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("О'Коннор Патрик");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }

    [Test]
    [Description("У сотрудника нет отчества, передано ФИО в формате «Фамилия И.» — возвращается сотрудник")]
    public void GetEmployeesByName_EmployeeWithoutMiddleNameByLastNameWithFirstInitial_ReturnsEmployee()
    {
      // Arrange
      var employee = EmployeeManager.CreateEmployee(47, "Кириллов", "Кирилл");

      // Act
      var result = Sungero.Company.Server.EmployeeFunctions.GetEmployeesByName("Кириллов К.");

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(employee.Id));
      });
    }
  }
}
