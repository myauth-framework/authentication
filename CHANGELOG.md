# Лог изменений

Все заметные изменения в этом проекте будут отражаться в этом документе.

Формат лога изменений базируется на [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.5.7] - 2021-11-02

### Обновлено

* `MyLab.Log.Dsl` с поддержкой инъекции логгера

## [1.5.6] - 2021-07-02

### Изменено

* Переход на другой nuget `DSL`-логирования: `MyLab.LogDsl` -> `MyLab.Log.Dsl`

## [1.5.5] - 2020-04-29

### Добавлено

* Метод расширения `AddRequiredClaimsChecker` для добавления механизма проверки обязательных утверждений.

## [1.5.4] - 2020-04-29

### Добавлено

* Поддержка атрибута обязательного заголовка утверждения для привязки входных параметров метода контроллера ([RequiredClaimHeaderAttribute](./readme.md#RequiredClaimHeaderAttribute));
* Поддержка входных параметров с проверкой наличия требуемых заголовков утверждений ([IRequiredClaimsIndicator](./readme.md#IRequiredClaimsIndicator)).

## [1.4.4] - 2020-04-27

### Добавлено 

- Поддержка схемы аутентификации [MyAuth2](https://github.com/ozzy-ext-myauth/specification/blob/master/v2/myauth-authentication-2.md);
- Добавлен журнал изменений.
