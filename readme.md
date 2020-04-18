# MyAuth.Authentication

## Обзор

Библиотека, содержащая инструменты для аутентификации по идентификатру пользователя. 

Для платформ .NET Core 3.1+

### Аутентификация

Аутентификационные данные передаются стандартным способом - в заголовке `Authorization`. Схема аутентификации - `MyAuth1`. В качестве параметров аутентификации предаётся идентификатор пользователя в открытом виде. 

Пример заголовка:

```
Authorization: MyAuth1 de184f1550844738954f97b6b01b8e01
```

### Утверждения пользователя

Поддерживается передача утверждений пользователя. Они должны передаваться в заголовке `X-UserClaims`. Корректное содержание заголовка - `JSON`, поля которого интерпретируются как утверждения. 

Пример заголовка:

```
X-User-Claims: {"Claim":"ClaimVal","roles":["Admin","SimpleUser"],"name":"John"}
```

Утверждения пользователя:

```
Claim: ClaimVal
name: name
http://schemas.microsoft.com/ws/2008/06/identity/claims/role: Admin
http://schemas.microsoft.com/ws/2008/06/identity/claims/role: SuperUser
```

### Адаптация к утверждениям .NET Identity

Действуют следующие правила адаптации передаваемых параметров под утверждения .NET Identoty:

* идентификатор пользователя из заголовка Authorization:
  * http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
  * http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name
* утверждения из заголовка `X-UserClaims`
  * `Roles` => http://schemas.microsoft.com/ws/2008/06/identity/claims/role
  * `roles` => http://schemas.microsoft.com/ws/2008/06/identity/claims/role
  * `Role` => http://schemas.microsoft.com/ws/2008/06/identity/claims/role
  * `role` => http://schemas.microsoft.com/ws/2008/06/identity/claims/role
  * `Roles` => http://schemas.microsoft.com/ws/2008/06/identity/claims/role
  * `roles` => http://schemas.microsoft.com/ws/2008/06/identity/claims/role

### Использование

Полученные в результате аутентификации утверждения доступны в контроллере через свойство `Request.HttpContext.User.Claims`.

## Интеграция 

Интеграция механизмов безопасности осуществляется в классе `Startup`:

```C#
public class Startup
{
    ... 
        
    public void ConfigureServices(IServiceCollection services)
    {
		...
        services.AddMyAuthAuthentication();			// <---- 1 (обязательно)
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

1. Добавление сервисов аутентификации;
2. Добавления механизма аутентификации в конвейер обработки запроса;
3. Включение авторизации для возможности контроля успешной аутентификации.

## Авторизация

Авторизация в данном случае - контроль, позволяющий удостовериться, что поступил запрос, успешно прошедший аутентификацию - был передан идентификатор пользователя.

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
