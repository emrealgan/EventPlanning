using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class UserPermissionFilter : IActionFilter
{
    private readonly string _userIdRouteParameter;

    public UserPermissionFilter(string userIdRouteParameter = "id")
    {
        _userIdRouteParameter = userIdRouteParameter;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

        // Eğer kullanıcı admin ise kontrol yapmaya gerek yok, her şeye erişebilir
        if (userRole == "admin")
        {
            return;
        }

        // Kullanıcı 'user' ise yalnızca kendi verisine erişim izni almalı
        if (userRole == "user")
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Route verisinden hedef kullanıcı ID'sini al
            if (context.ActionArguments.TryGetValue(_userIdRouteParameter, out var targetUserId) &&
                targetUserId?.ToString() != userId)
            {
                context.Result = new ForbidResult("Bu verilere erişim izniniz yok.");
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { /* Sonrası için gerekli değil */ }
}
