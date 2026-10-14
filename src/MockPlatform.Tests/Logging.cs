using NLog;
using Logger = Sungero.Core.Logger;
using LogManager = MockPlatform.Logging.LogManager;

namespace MockPlatform.Tests;

/// <summary>
/// Тесты для проверки логирования.
/// </summary>
/// <remarks>
/// Класс LogManager был создан для хранения логов в памяти, что позволяет
/// проверять логирование в юнит-тестах. Доступны следующие методы:
/// - LogManager.GetLogs() - получить все записи логов
/// - LogManager.GetMessages() - получить все сообщения логов
/// - LogManager.GetLogsByLevel(level) - получить записи логов по уровню
/// - LogManager.Clear() - очистить все логи
/// </remarks>
public class Logging
{
  [SetUp]
  public void SetUp() => LogManager.Clear();

  #region Тесты функциональности Logger

  /// <summary>
  /// Проверить запись отладочного сообщения.
  /// </summary>
  [Test]
  public void Logger_ShouldLogDebugMessage()
  {
    // Act - записываем отладочное сообщение.
    Logger.Debug("Test debug message");

    // Assert - проверяем, что сообщение записано.
    var logs = LogManager.GetLogs();

    Assert.That(logs, Has.Count.EqualTo(1), "Должна быть 1 запись в логе");
    Assert.Multiple(() =>
    {
      Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Debug), "Уровень должен быть Debug");
      Assert.That(logs[0].Message, Is.EqualTo("Test debug message"), "Сообщение должно совпадать");
    });
    Assert.That(logs[0].Exception, Is.Null, "Исключение должно быть null");
  }

  /// <summary>
  /// Проверить запись отладочного сообщения с форматированием.
  /// </summary>
  [Test]
  public void Logger_ShouldLogDebugFormatMessage()
  {
    // Act - записываем отладочное сообщение с форматированием.
    Logger.DebugFormat("Test {0} with {1}", "debug", "formatting");

    // Assert - проверяем, что сообщение записано с правильным форматированием.
    var logs = LogManager.GetLogs();
    Assert.That(logs, Has.Count.EqualTo(1), "Должна быть 1 запись в логе");
    Assert.Multiple(() =>
    {
      Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Debug), "Уровень должен быть Debug");
      Assert.That(logs[0].Message, Is.EqualTo("Test debug with formatting"), "Сообщение должно быть отформатировано");
    });
  }

  /// <summary>
  /// Проверить запись ошибки без исключения.
  /// </summary>
  [Test]
  public void Logger_ShouldLogErrorMessage()
  {
    // Act - записываем ошибку.
    Logger.Error("Test error message");

    // Assert - проверяем, что сообщение записано.
    var logs = LogManager.GetLogs();
    Assert.That(logs, Has.Count.EqualTo(1), "Должна быть 1 запись в логе");
    Assert.Multiple(() =>
    {
      Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Error), "Уровень должен быть Error");
      Assert.That(logs[0].Message, Is.EqualTo("Test error message"), "Сообщение должно совпадать");
    });
    Assert.That(logs[0].Exception, Is.Null, "Исключение должно быть null");
  }

  /// <summary>
  /// Проверить запись ошибки с исключением.
  /// </summary>
  [Test]
  public void Logger_ShouldLogErrorWithException()
  {
    // Arrange - создаем исключение.
    var exception = new InvalidOperationException("Test exception");

    // Act - записываем ошибку с исключением.
    Logger.Error("Test error with exception", exception);

    // Assert - проверяем, что сообщение и исключение записаны.
    var logs = LogManager.GetLogs();
    Assert.That(logs, Has.Count.EqualTo(1), "Должна быть 1 запись в логе");
    Assert.Multiple(() =>
    {
      Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Error), "Уровень должен быть Error");
      Assert.That(logs[0].Message, Is.EqualTo("Test error with exception"), "Сообщение должно совпадать");
    });
    Assert.That(logs[0].Exception, Is.Not.Null, "Исключение не должно быть null");
    Assert.That(logs[0].Exception, Is.EqualTo(exception), "Исключение должно совпадать");
  }

  /// <summary>
  /// Проверить запись ошибки с форматированием.
  /// </summary>
  [Test]
  public void Logger_ShouldLogErrorFormatMessage()
  {
    // Act - записываем ошибку с форматированием.
    Logger.ErrorFormat("Error {0}: {1}", "code", 500);

    // Assert - проверяем, что сообщение записано с правильным форматированием.
    var logs = LogManager.GetLogs();
    Assert.That(logs, Has.Count.EqualTo(1), "Должна быть 1 запись в логе");
    Assert.Multiple(() =>
    {
      Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Error), "Уровень должен быть Error");
      Assert.That(logs[0].Message, Is.EqualTo("Error code: 500"), "Сообщение должно быть отформатировано");
    });
  }

  /// <summary>
  /// Проверить запись ошибки с форматированием и исключением.
  /// </summary>
  [Test]
  public void Logger_ShouldLogErrorFormatWithException()
  {
    // Arrange - создаем исключение.
    var exception = new ArgumentException("Invalid argument");

    // Act - записываем ошибку с форматированием и исключением.
    Logger.ErrorFormat("Error in {0} at {1}", exception, "TestMethod", "line 42");

    // Assert - проверяем, что сообщение и исключение записаны.
    var logs = LogManager.GetLogs();
    Assert.That(logs, Has.Count.EqualTo(1), "Должна быть 1 запись в логе");
    Assert.Multiple(() =>
    {
      Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Error), "Уровень должен быть Error");
      Assert.That(logs[0].Message, Is.EqualTo("Error in TestMethod at line 42"), "Сообщение должно быть отформатировано");
    });
    Assert.That(logs[0].Exception, Is.Not.Null, "Исключение не должно быть null");
    Assert.That(logs[0].Exception, Is.EqualTo(exception), "Исключение должно совпадать");
  }

  /// <summary>
  /// Проверить очистку логов.
  /// </summary>
  [Test]
  public void Logger_ShouldClearLogs()
  {
    // Arrange - добавляем несколько записей в лог.
    Logger.Debug("Test message 1");
    Logger.Debug("Test message 2");
    Logger.Error("Test message 3");

    // Act - очищаем логи.
    var logsBeforeClear = LogManager.GetLogs();
    Assert.That(logsBeforeClear, Has.Count.EqualTo(3), "Должно быть 3 записи в логе перед очисткой");

    LogManager.Clear();

    // Assert - проверяем, что логи очищены.
    var logsAfterClear = LogManager.GetLogs();
    Assert.That(logsAfterClear, Is.Empty, "Лог должен быть пустым после очистки");
  }

  /// <summary>
  /// Проверить фильтрацию логов по уровню.
  /// </summary>
  [Test]
  public void Logger_ShouldFilterLogsByLevel()
  {
    // Arrange - добавляем записи разных уровней.
    Logger.Debug("Debug message");
    Logger.Debug("Debug message");
    Logger.Debug("Debug message");
    Logger.Error("Error message");
    Logger.Debug("Another debug message");

    // Act & Assert - проверяем фильтрацию.
    var debugLogs = LogManager.GetLogsByLevel(LogLevel.Debug);
    Assert.That(debugLogs, Has.Count.EqualTo(4), "Должно быть 4 отладочных записи");

    var errorLogs = LogManager.GetLogsByLevel(LogLevel.Error);
    Assert.That(errorLogs, Has.Count.EqualTo(1), "Должна быть 1 запись об ошибке");
  }

  /// <summary>
  /// Проверить получение всех сообщений логов.
  /// </summary>
  [Test]
  public void Logger_ShouldReturnAllMessages()
  {
    // Arrange - добавляем записи.
    Logger.Debug("First message");
    Logger.Debug("Second message");
    Logger.Error("Third message");

    // Act - получаем все сообщения.
    var messages = LogManager.GetMessages();

    // Assert - проверяем количество и содержание.
    Assert.That(messages, Has.Count.EqualTo(3), "Должно быть 3 сообщения");
    Assert.That(messages, Does.Contain("First message"), "Должно содержать 'First message'");
    Assert.That(messages, Does.Contain("Second message"), "Должно содержать 'Second message'");
    Assert.That(messages, Does.Contain("Third message"), "Должно содержать 'Third message'");
  }

  /// <summary>
  /// Проверить запись временной метки в логе.
  /// </summary>
  [Test]
  public void Logger_ShouldRecordTimestamp()
  {
    // Act - записываем сообщение.
    Logger.Debug("Test message");

    // Arrange - запоминаем время после записи.
    var timeAfter = DateTime.Now;

    // Assert - проверяем, что временная метка находится в ожидаемом диапазоне.
    var logs = LogManager.GetLogs();
    Assert.That(logs, Has.Count.EqualTo(1), "Должна быть 1 запись в логе");
    Assert.That(logs[0].TimeStamp, Is.LessThanOrEqualTo(timeAfter), "Временная метка должна быть <= времени после записи");
  }

  /// <summary>
  /// Проверить порядок записей в логе.
  /// </summary>
  [Test]
  public void Logger_ShouldMaintainLogOrder()
  {
    // Act - добавляем записи в определенном порядке.
    Logger.Debug("First");
    Logger.Debug("Second");
    Logger.Debug("Third");
    Logger.Error("Fourth");

    // Assert - проверяем, что порядок сохранен.
    var messages = LogManager.GetMessages();
    Assert.That(messages, Has.Count.EqualTo(4), "Должно быть 4 сообщения");
    Assert.Multiple(() =>
    {
      Assert.That(messages[0], Is.EqualTo("First"), "Первое сообщение должно быть 'First'");
      Assert.That(messages[1], Is.EqualTo("Second"), "Второе сообщение должно быть 'Second'");
      Assert.That(messages[2], Is.EqualTo("Third"), "Третье сообщение должно быть 'Third'");
      Assert.That(messages[3], Is.EqualTo("Fourth"), "Четвертое сообщение должно быть 'Fourth'");
    });
  }

  #endregion
}
