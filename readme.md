# MyAuth.Authentication

## Обзор

Библиотека, содержащая инструменты для обеспечения поддержки схемы аутентификации `MyAuth` для платформ .NET Core 3.1+.

При необходимости, ознакомьтесь со спецификацией [MyAuth](https://github.com/ozzy-ext-myauth/specification) аутентификации.

Полученные в результате аутентификации утверждения доступны в контроллере через свойство `Request.HttpContext.User.Claims`.

При этом действуют правила адаптации передаваемых утверждений под утверждения .NET Identoty:

* `sub` => http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
* `name` => http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name

* `sub` (если не указан `name`) => http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name

* `roles` (каждый элемент) => http://schemas.microsoft.com/ws/2008/06/identity/claims/role
* `role` => http://schemas.microsoft.com/ws/2008/06/identity/claims/role

Пример разбора заголовка:

```
Authorization: MyAuth1 sub="user-1232314", name="John", roles="employee,admin", my:age="50"
```

Утверждения в приложении:

```
http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier:	user-1232314
http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name:				John
http://schemas.microsoft.com/ws/2008/06/identity/claims/role: 			Admin
http://schemas.microsoft.com/ws/2008/06/identity/claims/role: 			SuperUser
my:age																	50
```

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
