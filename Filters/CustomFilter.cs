using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;

namespace OnlineShopAdmin.Filters
{
    public class HttpRequestInfo : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var message = context.HttpContext.Request.Headers;
            var method = context.HttpContext.Request.Method;
            string controllername = context.ActionDescriptor.RouteValues["controller"];
            string actionname = context.ActionDescriptor.RouteValues["action"];
            File.AppendAllText("HttpRequests.txt", DateTime.Now.ToString() + Environment.NewLine + $"ControllerName: {controllername} ActionName: {actionname} Method: {method}" + Environment.NewLine);
            foreach (var item in message)
            {
              File.AppendAllText("HttpRequests.txt",item.Key + ":" 
                  + item.Value.ToString() + Environment.NewLine);
            }

            File.AppendAllText("HttpRequests.txt", "-----------------" + Environment.NewLine);
        }
    }
}
