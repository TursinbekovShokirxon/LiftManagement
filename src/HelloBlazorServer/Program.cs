using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Memory;
using Samples.HelloBlazorServer;

namespace Samples.HelloBlazorServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(cfg => {
                    cfg.Sources.Insert(0, new MemoryConfigurationSource() {
                        InitialData = new Dictionary<string, string?>()
                        {
                            { WebHostDefaults.ServerUrlsKey, "http://localhost:5005" },
                        }
                    });
                })
                .ConfigureWebHostDefaults(webHost => webHost
                    .UseDefaultServiceProvider((ctx, options) => {
                        options.ValidateScopes = ctx.HostingEnvironment.IsDevelopment();
                        options.ValidateOnBuild = true;
                    })
                    .UseStartup<Startup>())
                .Build();

            host.Run();
        }
    }
}
