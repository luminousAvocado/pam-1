using System.Net;
using System.Net.Mail;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
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
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            var connString = Configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));
            services.AddAutoMapper();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanProcessRequests", policyBuilder => policyBuilder.RequireClaim("ProcessingUnitId"));
                options.AddPolicy("CanReviewRequests", policyBuilder => policyBuilder.RequireClaim("IsApprover"));
                options.AddPolicy("IsAdmin", policyBuilder => policyBuilder.RequireClaim("IsAdmin"));
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
            // TEST AuditLog
            services.AddScoped<AuditLogService>();
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

            app.UsePathBase(Configuration.GetValue<string>("Application:PathBase"));
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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
