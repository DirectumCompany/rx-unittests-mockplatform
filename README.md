# rx-unittests-mockplatform

Мок-платформа для юнит-тестов прикладной Directum RX. Прикладной код вызывается из тестов NUnit без сервера и базы.

Ветка 26.2: решение проверено на Directum RX 26.2. Мок-платформа тесно связана с внутренним устройством платформы, поэтому на других версиях, и более ранних, и более поздних, скорее всего, понадобится поправить исходники. Обычно правки небольшие. Репозиторий удобно использовать как основу для своих тестов: забрать, подстроить под свою версию и развивать дальше.

## Запуск

Нужны Windows, .NET SDK 10 и среда выполнения .NET 8: платформа 26.2 работает на .NET 8, и её сборки не загружаются в более новых версиях .NET, поэтому проекты собираются под `net8.0`.

1. Склонировать репозиторий.
2. В `Directory.Build.props` указать две папки со сборками одной версии RX:
   - `LibWebServerPath` — путь до платформенных сборок веб-сервера;
   - `AppliedModulesPath` — путь до прикладных сборок.

   ```xml
   <LibWebServerPath>C:\Libs\WebServer</LibWebServerPath>
   <AppliedModulesPath>C:\Libs\Applied</AppliedModulesPath>
   ```

   В папке прикладных сборок должна лежать и интерфейсная сборка `Sungero.Domain.Interfaces.dll`. В пакете разработки `BaseSolution.dat` её нет, она генерируется DeploymentTool той же версии: `DeploymentToolCore.exe -b <папка прикладных сборок> -d BaseSolution.dat`.

3. Запустить тесты: `dotnet test rx-unittests-mockplatform.sln` или из Visual Studio.

В CI пути удобнее передать параметрами: `-p:LibWebServerPath=... -p:AppliedModulesPath=...`.

## Свой тестовый проект

Сделать по образцу `examples/Company.Unit.Tests`: csproj подключает `UnitTests.props` (NUnit, Moq, сборки RX и мок-платформа), а `Environment/CompanyEnvironment.cs` регистрирует модули, код которых вызывают тесты. Проект добавить в `rx-unittests-mockplatform.sln`.

Как устроена мок-платформа и как оформлять тесты — [docs/mock-platform.md](docs/mock-platform.md).

Примеры юнит-тестов с пояснениями — [docs/unit-test-examples.md](docs/unit-test-examples.md).

## Примеры

`examples/Company.Unit.Tests` — юнит-тесты модуля Company из RX.
