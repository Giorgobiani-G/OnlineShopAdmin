using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;

namespace OnlineShopAdmin.Filters
{
    public class CustomFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //context.ActionDescriptor.RouteValues["action"]
            var message = context.HttpContext.Request.Headers;
            
            var action = context.ActionDescriptor.RouteValues["action"];
            var controller = context.ActionDescriptor.RouteValues["controller"];
            File.AppendAllText("HttpRequests.txt", DateTime.Now.ToString() + Environment.NewLine);
            foreach (var item in message)
            {
              File.AppendAllText("HttpRequests.txt", item.Key + ":" + item.Value.ToString() + Environment.NewLine);
            }
            File.AppendAllText("HttpRequests.txt", "-----------------" + Environment.NewLine);
            //File.AppendAllText("HttpRequests.txt", message + Environment.NewLine);
        }
    }
}
