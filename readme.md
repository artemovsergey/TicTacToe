# Приложение "Крестики-нолики" 

[![.NET Tests](https://github.com/artemovsergey/TicTacToeApp/actions/workflows/dotnet-test.yaml/badge.svg?branch=master&event=push)](https://github.com/artemovsergey/TicTacToeApp/actions/workflows/dotnet-test.yaml)

[![deploy](https://github.com/artemovsergey/TicTacToeApp/actions/workflows/deploy.yml/badge.svg)](https://github.com/artemovsergey/TicTacToeApp/actions/workflows/deploy.yml)

# Запуск приложения

```
docker-compose up --build
```

**Замечание**: если база данных уже существует выполните команду `docker-compose down -v`, а затем сделайте сборку заново

# Архитектура приложения

- решение содержит проект `MinimalAPI` и проект `xUnit`

# Основные функции

 - конфигурация приложения через переменные окружения локально и в production
 - юнит и интеграционные тесты локально `dotnet test`
 - интеграционные тесты используют технологию `TestContainters` для чего нужен Docker 
 - покрытие > 30%
 - настроен ci/cd pipeline на github action на запуск тестовы и развертывание на VPS
 - глобальная обработку ошибок через middleware и стандартизированная модель ответа для исключений 
 - асинхронная работа с данными с использованием токена отмены
 - взаимодействие с базой данных Postgres осуществляется через EntityFrameWorkCore посредством механизма миграций.
 - реализован выбор размерности поля игры
 - полностью настроен openapi через swagger 
 - предусмотрена валидация на входные данные
 - состояние игры храниться базе, что способствует восстановлению игры в любое время
 - реализована проверка состояния игры с помощью ETag в заголовках ответов

 # Новое

 - добавлены новые переменные окружения в виде **вероятности замены хода** и **частоты шага вероятности**. Теперь можно настраивать любую валидную веятность и номер хода, на какой вероятность может сработать.
 - для проверки работоспособности API реализована связь с фронтендом на React https://tic-tac-toe-frontend-five.vercel.app/