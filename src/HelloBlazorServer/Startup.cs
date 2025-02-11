using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Samples.HelloBlazorServer.Services;
using ActualLab.Fusion.Blazor;
using ActualLab.Fusion.Extensions;
using ActualLab.Fusion.UI;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using LiftManagement.HelloBlazorServer.Db;

namespace Samples.HelloBlazorServer;

public class Startup
{
    public void ConfigureServices(IServiceCollection services,IConfiguration cfg)
    {
#pragma warning disable ASP0000
        var tmpServices = services.BuildServiceProvider();
#pragma warning restore ASP0000

        // Fusion services
        var fusion = services.AddFusion();
        fusion.AddBlazor();
        fusion.AddFusionTime();
        fusion.AddService<LiftManagementService>();
        fusion.AddService<CounterService>();
        fusion.AddService<WeatherForecastService>();


        // Default update delay is set to 0.1s
        services.AddTransient<IUpdateDelayer>(c => new UpdateDelayer(c.UIActionTracker(), 0.1));

        //DbContext 
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(cfg.GetConnectionString("Shokir")));
        // ASP.NET Core / Blazor services
        services.AddRazorPages();
        services.AddServerSideBlazor(o => o.DetailedErrors = true);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }
        else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");

        });
    }
}
