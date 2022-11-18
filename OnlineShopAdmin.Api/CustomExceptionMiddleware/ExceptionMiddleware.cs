using Microsoft.AspNetCore.Http;
using OnlineShopAdmin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace OnlineShopAdmin.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var values = ((object[])httpContext.Request.RouteValues.Values);
                string method = httpContext.Request.Method;
                string controller = values[0].ToString();
                string action = values[1].ToString();
              
                File.AppendAllText("Exceptions.txt",$"Exception thrown by controller-{controller} action-{action} method-{method}" +
                    $"{Environment.NewLine}" + $"Excepton message: {ex.Message} {Environment.NewLine}ExceptonType: " +
                    $"{ex.GetType().FullName} {Environment.NewLine}" + $"Inner Exception message: {ex.InnerException?.Message ?? "None"} {Environment.NewLine}" +
                    $"LogDate: {DateTime.Now}" +
                    Environment.NewLine + "---------" + Environment.NewLine);
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error",
            }.ToString());
        }
    }
}
