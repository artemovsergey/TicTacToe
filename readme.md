# Приложение "Крестики-нолики" 

[![.NET Tests](https://github.com/artemovsergey/TicTacToeApp/actions/workflows/dotnet-test.yaml/badge.svg?branch=master&event=push)](https://github.com/artemovsergey/TicTacToeApp/actions/workflows/dotnet-test.yaml)

 - сделать юнит и интеграционные тесты
 - настроить ci/cd
 - добавить общую обработку ошибок +
 - перейти на асинхронность +
 - добавить ct +
 - настроить openapi +
 - настроить валидацию  

# Report of Coverage Test

- dotnet tool install -g dotnet-reportgenerator-globaltool
- установить `coverlet.msbuild`
- dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
- reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html