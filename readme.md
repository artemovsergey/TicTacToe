# Приложение "Крестики-нолики"

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
- dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=json

- reportgenerator -reports:"coverage.json" -targetdir:"coveragereport" -reporttypes:TextSummary