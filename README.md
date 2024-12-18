# Об архитектуре проекта
## Общая топология

```mermaid
flowchart TB

subgraph entrypoint[World]
    d1[Запросы из мира]:::description
end
entrypoint:::entrypoint

entrypoint--https://*.unicort.ru\nport 443-->nginx


subgraph dockerBridgeNetwork[docker bridge network]
    subgraph nginx[nginx]
        direction LR
        h2[reverse proxy]:::type
        d2[Принимает https трафик и редиректит его внутрь микросервисов по http \n\n maintainer: @greg-zamza]:::description
    end
    nginx:::internalContainer

    subgraph bot-service[bot-service]
        direction LR
        h3[Отвечает за работу telegram бота]:::type
        d3[maintainer: @greg-zamza]:::description
    end
    bot-service:::internalContainer

    subgraph app-frontend[app-frontend]
        direction LR
        h4[Фронтенд]:::type
        d4[описание]:::description
    end
    app-frontend:::internalContainer

    subgraph app-backend[app-backend]
        direction LR
        h5[Бэкенд]:::type
        d5[maintainer: @KostennikovDanil]:::description
    end
    app-backend:::internalContainer

    subgraph admin-panel[admin-panel]
        direction LR
        h6[Админ-панель на django]:::type
        d6[maintainer: @omud38]:::description
    end
    admin-panel:::internalContainer

    subgraph database[database]
        direction LR
        h7[postgreSQL]:::type
        d7[описание]:::description
    end
    database:::internalContainer

    %% Проксирование запросов из мира
    nginx--https://bot.unicort.ru -> \n http://bot-service:8000-->bot-service
    nginx--https://app.unicort.ru -> \n http://app-frontend:8080-->app-frontend
    nginx--https://back.unicort.ru -> \n http://app-backend:8080-->app-backend
    nginx--https://panel.unicort.ru -> \n http://admin-panel:8080-->admin-panel
    

    %% Связи контейнеров внутри bridge сети
    %% app-frontend--http://app-backend:8080-->app-backend
    %% app-backend--http://bot-service:8080-->bot-service
    %% admin-panel--http://bot-service:8080-->bot-service

    %% bot-service--database:5432-->database
    %% app-backend--database:5432-->database
    %% admin-panel--database:5432-->database
end

%% Element type definitions

classDef entrypoint fill:#08427b
classDef internalContainer fill:#1168bd, font-size:20px
classDef externalSystem fill:#999999

classDef type stroke-width:0px, color:#fff, fill:transparent, font-size:20px
classDef description stroke-width:0px, color:#fff, fill:transparent, font-size:16px
```
## Пояснение
1) `bridge-network` - это изолированная сеть, в которой находятся все контейнеры. Все они могут пинговать друг друга по имени контейнера вместо доменного имени. Например, запрос из `app-backend` в `database` будет проходить по `database:5432`
> [!IMPORTANT]
> Контейнеры доступны только внутри изолированной сети (у них нет торчащих наружу портов)! Всё проксируется только через `nginx`!

2) Если необходимо обращаться к контейнеру извне (например, с фронтенда в `app-backend`, это можно делать *исключительно по https и закреплённому за микросервисом доменному имени*. Например, запрос на бэкенд будет выглядеть так: `https://back.unicort.ru`
> [!WARNING]
> Не нужно хардкодить URL для обращения к микросервисам извне. URL на dev и prod средах отличаются и они могут меняться. *Берите URL и все остальные переменные, зависящие от dev/prod среды из __переменных окружения__ или из __.env__* .
>
> URL любого микросервиса вы можете получить из переменной `имя-сервиса-URL`
> 
> `const botServiceURL = process.env.bot-service-URL;`


Актуальные имена контейнеров можно узнать из схемы выше, которая постоянно обновляется. Список редиректов из nginx в контейнеры содержится в [README сервиса nginx](nginx/prod/README.md)  [(dev)](nginx/dev/README.md). Тут можно посмотреть названия переменных окружения и сами URL. 


## Правила оформления микросервисов
> [!IMPORTANT]
> - В каждом микросервисе должен быть файл `compose.yaml`, соотвествующий референсу
> - Нейминг должен соответствовать стандарту: lower case и разделение слов с помощью тире. Например: `app-frontend`
> - Название директории желательнно должно совпадать с названием микросервиса
> - Каждый микросервис должен содержаться в своей директории, даже если он состоит из одного compose.yaml, как database
# TennisProject
