using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatApp.Hubs;
using ChatApp.DB;
using Microsoft.EntityFrameworkCore;
namespace ChatApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR();
            services.AddTransient<ChatHub>();
            services.AddDistributedMemoryCache();
            services.AddSession();
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DBContent>(options => options.UseSqlServer(connection));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Register}/{id?}");
                endpoints.MapHub<AuthHub>("/authhub");
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<RoomHub>("/roomhub");
            });
        }
    }
}
