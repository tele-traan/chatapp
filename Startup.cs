using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;

using ChatApp.DB;
using ChatApp.Hubs;
using ChatApp.Repositories;

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

            services.AddDistributedMemoryCache();
            services.AddSession();

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ChatDbContext>(options => options.UseSqlServer(connection));
            services.AddDataProtection().
                PersistKeysToFileSystem(new DirectoryInfo("DataProtectionKeys"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<IGCUsersRepository, GCUsersRepository>();
            services.AddTransient<IRoomsRepository, RoomsRepository>();
            services.AddTransient<IRoomUsersRepository, RoomUsersRepository>();
            services.AddTransient<IBanInfoRepository, BanInfoRepository>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Auth/RegisterIndex");
                    options.AccessDeniedPath = new PathString("/Home/Index");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();

            if(env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=RegisterIndex}");
                endpoints.MapHub<AuthHub>("/authhub");
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<RoomHub>("/roomhub");
                endpoints.MapHub<ManageRoomHub>("/manageroomhub");
            });
        }
    }
}