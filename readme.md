# API для приложение "Крестики-нолики" 

# Запуск
```
docker-compose up --build
```
**Замечание**: для перезагрузки базы данных выполните команду `docker-compose down -v`

# Архитектура приложения

- решение содержит проект `MinimalAPI` и проект `xUnit`

# Основные функции

 - конфигурация приложения выполняется через переменные окружения
 - unit и интеграционные тесты работают локально по команде `dotnet test`
 - интеграционные тесты используют технологию `TestContainters`, для чего нужен Docker 
 - метрики покрытия API тестами на github pages

[![unit and integration Tests](https://github.com/artemovsergey/TicTacToe/actions/workflows/dotnet-test.yaml/badge.svg)](https://github.com/artemovsergey/TicTacToe/actions/workflows/dotnet-test.yaml)
 - настроен ci/cd pipeline на github action на запуск тестов:

 - осуществлено развертывание на VPS:

   [![deploy](https://github.com/artemovsergey/TicTacToe/actions/workflows/deploy.yml/badge.svg)](https://github.com/artemovsergey/TicTacToe/actions/workflows/deploy.yml)
   
 - глобальная обработка ошибок через `middleware` и стандартизированная модель ответа для исключений 
 - асинхронная работа с данными с использованием токена отмены
 - взаимодействие с базой данных `Postgres` осуществляется через `EntityFrameWorkCore` посредством механизма миграций.
 - реализован выбор размерности поля игры
 - полностью настроен `openapi` через `swagger` 
 - предусмотрена валидация на входные данные
 - состояние игры храниться базе, что способствует восстановлению процесса игры
 - реализована проверка состояния игры на идемпотентность с помощью `ETag` в заголовках ответов

# Покрытие тестами 

[![pages-build-deployment](https://github.com/artemovsergey/TicTacToe/actions/workflows/pages/pages-build-deployment/badge.svg)](https://github.com/artemovsergey/TicTacToe/actions/workflows/pages/pages-build-deployment)

- установить инструмент `dotnet tool install -g dotnet-reportgenerator-globaltool`
- установить `coverlet.msbuild` в проект xUnit
- `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura`
- перейти в тестовый проект и создать отчет: `reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html`
- отчет находится в папке `coveragerepost` в виде index.html, а также развернут https://artemovsergey.github.io/TicTacToe/

 # Новое

 - добавлены новые переменные окружения в виде **вероятности замены хода** и **частоты шага вероятности**. Теперь можно настраивать любую валидную вероятность и номер хода, на котором вероятность может сработать.
 - для проверки работоспособности API реализована связь с фронтендом на Angular.
