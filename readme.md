# MyAuth.HeaderAuthentication

## Обзор

Библиотека, содержащая инструменты для Аутентификации по заголовкам. 

Для платформ .NET Core 3.1+

### Заголовки аутентификации



### Время жизни сервиса

Для получения корректных данных, сервис, принимающий в конструкторе контекст пользователя, должен быть зарегистрирован в контейнере зависимостей как [Transient](https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1#transient) или [Scoped](https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1#scoped).

Соответственно, добавлять их в контейнер необходимо методами [AddTransient](https://docs.microsoft.com/ru-ru/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions.addtransient) или [AddScoped](https://docs.microsoft.com/ru-ru/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions.addscoped).

## Интеграция 

Интеграция механизмов безопасности осуществляется в классе `Startup`:

```C#
public class Startup
{
    ... 
        
    public void ConfigureServices(IServiceCollection services)
    {
		...
        services.AddInfonotSecurity();			// <---- 1 (обязательно)
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app.UseAuthentication() 				// <---- 2 (обязательно)
        app.UseAuthorization()					// <---- 3 (опционально)
        ...
    }
}
```

1. Добавление сервисов безопасности;
2. Включение аутентификации для активации механизма применения аутентификационных заголовков;
3. Включение авторизации для возможности контроля успешной аутентификации.

## Авторизация

Авторизация в данном случае - контроль, позволяющий удостовериться, что поступил запрос, содержащий информацию о контексте пользователя.

Для включения авторизации для контроллера или метода контроллера, необходимо пометить его атрибутом `Authorize`:

```C#
[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
	    [Authorize]								// <---
        [HttpGet("authorized")]
        public IActionResult GetAuthorized()
        {
            return Ok();
        }
}

[Authorize]										// <---
[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
        [HttpGet("authorized")]
        public IActionResult GetAuthorized()
        {
            return Ok();
        }
}
```

При использовании таких атрибутов в приложении необходимо включить авторизацию:

```C#
public class Startup
{
    ... 
        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app.UseAuthorization()					 
        ...
    }
}
```

## Контекст пользователя

```C#
/// <summary>
    /// Содержит данные пользовательсткого контекста запроса системы Инфонот
    /// </summary>
    public interface IInfonotClientContext
    {
        /// <summary>
        /// Информация о текущем пользователе
        /// </summary>
        IInfonotPerson User { get; }
        /// <summary>
        /// Информация о пользователе ЛК
        /// </summary>
        IInfonotPerson Account { get; }
        /// <summary>
        /// Идентификатор нотариальной палаты
        /// </summary>
        string ChamberId { get; }
        /// <summary>
        /// Наименование нотариальной палаты
        /// </summary>
        string ChamberName { get; }
        /// <summary>
        /// Проверяет соответствие пользователя роли
        /// </summary>
        bool IsInRole(string expectedRole);
    }
```

```C#
/// <summary>
    /// Содержит информацию о пользователе системы Инфонот
    /// </summary>
    public interface IInfonotPerson
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Имя [, отчество]
        /// </summary>
        string GivenName { get; }
        /// <summary>
        /// Фамилия
        /// </summary>
        string LastName { get; }
        /// <summary>
        /// Должность
        /// </summary>
        string Title{ get; }
    }

```