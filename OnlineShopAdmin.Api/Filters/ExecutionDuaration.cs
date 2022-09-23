using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.IO;

namespace OnlineShopAdmin.Filters
{
    public class ExecutionDuaration : ActionFilterAttribute
    {
        private Stopwatch _stopwatch = new Stopwatch();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            string actionname = context.ActionDescriptor.RouteValues["action"];
            string message = actionname + ": " +  _stopwatch.ElapsedMilliseconds.ToString() + "ms Date: " + DateTime.Now + Environment.NewLine; 
            File.AppendAllText("Duaration.txt",message);
        }
    }
}
