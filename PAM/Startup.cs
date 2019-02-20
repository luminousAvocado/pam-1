using System.Net;
using System.Net.Mail;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Services;

namespace PAM
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;

        public Startup(IHostingEnvironment env, IConfiguration config, ILoggerFactory loggerFactory)
        {
            _env = env;
            _logger = loggerFactory.CreateLogger<Startup>();
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connString = Configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));
            services.AddAutoMapper();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();

            services
                .AddFluentEmail(Configuration.GetValue<string>("Application:Email"))
                .AddRazorRenderer()
                .AddSmtpSender(() =>
                    new SmtpClient()
                    {
                        Host = Configuration.GetValue<string>("SMTP:Host"),
                        Port = Configuration.GetValue<int>("SMTP:Port"),
                        Credentials = new NetworkCredential(
                            Configuration.GetValue<string>("SMTP:Username"),
                            Configuration.GetValue<string>("SMTP:Password"))
                    }
                );
            services.AddSingleton(
                new EmailHelper()
                {
                    AppUrl = Configuration.GetValue<string>("Application:Url"),
                    AppEmail = Configuration.GetValue<string>("Application:Email"),
                    TemplateFolder = _env.ContentRootPath + "/Emails"
                }
            );

            if (Configuration.GetValue<bool>("ActiveDirectory:UseMockAD"))
            {
                services.AddSingleton<IADService, MockADService>();
                _logger.LogWarning("Using Mock ADService");
            }
            else
                services.AddSingleton<IADService, ADService>();

            services.AddScoped<OrganizationService>();
            services.AddScoped<UserService>();
            services.AddScoped<RequestService>();
            services.AddScoped<SystemService>();
            services.AddScoped<TreeViewService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                _logger.LogInformation("Development environment");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                _logger.LogInformation($"Environment: {_env.EnvironmentName}");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
