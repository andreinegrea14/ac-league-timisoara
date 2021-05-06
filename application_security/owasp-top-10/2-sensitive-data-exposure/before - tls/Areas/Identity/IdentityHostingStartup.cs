using System;
using Injection.Areas.Identity.Data;
using Injection.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Injection.Areas.Identity.IdentityHostingStartup))]
namespace Injection.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<InjectionContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("InjectionContextConnection")));

                services.AddDefaultIdentity<InjectionUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<InjectionContext>();
            });
        }
    }
}