using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SysModelBank.Data;
using SysModelBank.Services.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SysModelBank.Data.Models.Identity;
using SysModelBank.Services.Identity;
using SysModelBank.Data.Repositories;
using SysModelBank.Data.Repositories.Identity;
using SysModelBank.Data.Repositories.Settings;
using SysModelBank.Services.Settings;

namespace SysModelBank
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SysModelBankDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies());
            services.AddIdentity<User, Role>(options => 
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                
            })
                .AddEntityFrameworkStores<SysModelBankDbContext>();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            RegisterServices(services);

            services.AddRazorPages();
            services.ConfigureApplicationCookie(x =>
            {
                x.LoginPath = "/";
                x.LogoutPath = "/Identity";
                x.AccessDeniedPath = "/Error/AccessDenied";
            });

            services.AddAuthorization(options => 
            {
                options.AddPolicy(PolicyConstants.Admin, policy => 
                policy.RequireRole(Role.Admin));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=UserManagement}/{action=Index}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Identity}/{action=Index}/{id?}")
                    .RequireAuthorization();
                endpoints.MapRazorPages();
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionCodeRepository, TransactionCodeRepository>();
            services.AddScoped<ILogRepository, LogRepository>();

            // Services
            services.AddScoped<IBankLogger, BankLogger>();
            services.AddScoped<IUserService, UserService>();

            // Settings
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ICurrencyService, CurrencyService>();
        }
    }
}
