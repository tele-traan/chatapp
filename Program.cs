using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.DB;
using System.Linq;
using ChatApp.Models;
namespace ChatApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContent>();
                Sample.Initialise(context);
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
    public static class Sample
    {
        public static void Initialise(DBContent content)
        {
            if (!content.Users.Any())
            {
                content.Users.AddRange(new User { Username = "" }, new User { Username = "" });
            }
        }
    }
}