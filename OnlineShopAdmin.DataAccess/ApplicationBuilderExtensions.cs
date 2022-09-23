using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineShopAdmin.DataAccess.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess
{
    public static class ApplicationBuilderExtensions
    {
        //public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder builder)
        //{
        //    using var serviceScope = builder.ApplicationServices.CreateScope();
        //    var adventureWorksLT2019Context = serviceScope.ServiceProvider.GetService<AdventureWorksLT2019Context>();

        //    adventureWorksLT2019Context?.Database.Migrate();

        //    return builder;
        //}
    }
}
