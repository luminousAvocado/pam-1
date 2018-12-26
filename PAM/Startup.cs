using System.Net;
using System.Net.Mail;
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
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public Startup(IHostingEnvironment env, IConfiguration config, ILoggerFactory loggerFactory)
        {
            _env = env;
            _config = config;
            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connString = _config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services
                .AddFluentEmail(_config.GetValue<string>("Application:Email"))
                .AddRazorRenderer()
                .AddSmtpSender(() =>
                    new SmtpClient()
                    {
                        Host = _config.GetValue<string>("SMTP:Host"),
                        Port = _config.GetValue<int>("SMTP:Port"),
                        Credentials = new NetworkCredential(
                            _config.GetValue<string>("SMTP:Username"),
                            _config.GetValue<string>("SMTP:Password"))
                    }
                );

            services.AddSingleton(
                new EmailHelper()
                {
                    AppUrl = _config.GetValue<string>("Application:Url"),
                    AppEmail = _config.GetValue<string>("Application:Email"),
                    TemplateFolder = _env.ContentRootPath + "/Emails"
                }
            );
            services.AddSingleton<IADService, MockADService>();
            services.AddScoped<OrganizationService>();
            services.AddScoped<UserService>();
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

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Login}/{id?}");
            });
        }
    }
}
