using Microsoft.AspNetCore.Http;
using OnlineShopAdmin.Models;
using System;
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
                File.AppendAllText("Exceptions.txt",$"Excepton message: {ex.Message} {Environment.NewLine}ExceptonType: " +
                    $"{ex.GetType().FullName} {Environment.NewLine}LogDate: {DateTime.Now}" + 
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
