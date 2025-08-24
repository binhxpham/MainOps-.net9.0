using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using Microsoft.AspNetCore.Identity;
using MainOps.Models;
using MainOps.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Reflection;
using MainOps.Resources;
using MainOps.ExtensionMethods;
using Microsoft.IdentityModel.Protocols;
using MySql.Data.MySqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.AspNetCore.Http;
using Rotativa.AspNetCore;
using MainOps.Models.ReportClasses;
using System.Threading;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Features;

namespace MainOps
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<MyAppSecretConfig>();
            }
            builder.AddAzureKeyVault(
                configuration["KeyVault:SecretUri"],
                configuration["KeyVault:ClientId"],
                configuration["KeyVault:ClientSecret"]);
            var dom = builder.Build();
            Configuration = dom;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSingleton<LocService>();
            services.Configure<HostOptions>(
                opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(3600));
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;

            });
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            var thesecret = Configuration["AppSecret"];
            
            services.AddSession();
            //
            var mysqlbuilder = new MySqlConnectionStringBuilder
            {
                Server = "handdipnew.mysql.database.azure.com",
                Database = "handdipnew",
                UserID = "root2@handdipnew",
                Password = Configuration["conPass"],
                SslMode = MySqlSslMode.Required,
            };
            services.AddDbContext<DataContext>(options =>
                options.UseMySql(mysqlbuilder.ConnectionString));



            // identity
            services.AddIdentity<ApplicationUser,
                IdentityRole>(config => { config.SignIn.RequireConfirmedEmail = true; })
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders()
                .AddAccountTotpTokenProvider();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            // If you want to tweak Identity cookies, they're no longer part of IdentityOptions.
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");

            // If you don't want the cookie to be automatically authenticated and assigned to HttpContext.User, 
            // remove the CookieAuthenticationDefaults.AuthenticationScheme parameter passed to AddAuthentication.
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => {
                        options.LoginPath = "/Account/Login";
                        options.LogoutPath = "/Account/Logout";
                        options.ExpireTimeSpan = TimeSpan.FromDays(150);
                    });
            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AuthMessageSenderOptions"));
            services.Configure<AuthSFTPOptions>(Configuration.GetSection("AuthSFTPOptions"));
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(180);
                // options.ExcludedHosts.Add("example.com");
                // options.ExcludedHosts.Add("www.example.com");
            });

            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("SharedResource", assemblyName.Name);
                    };
                });
            services.AddKendo();

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedUICultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("de-DE"),
                        new CultureInfo("da-DK"),
                        new CultureInfo("nl-NL"),

                    };
                    var supportedCultures = new List<CultureInfo>
                    {
                        //new CultureInfo("de-DE"),
                        new CultureInfo("en-US"),
                        new CultureInfo("de-DE"),
                        new CultureInfo("da-DK"),
                        new CultureInfo("nl-NL"),
                    };
                    foreach (CultureInfo cI in supportedUICultures)
                    {
                        if(cI.TwoLetterISOLanguageName != "da")
                        {
                            cI.NumberFormat.CurrencyDecimalSeparator = ".";
                            cI.NumberFormat.NumberDecimalSeparator = ".";
                            cI.NumberFormat.NumberGroupSeparator = ",";
                        }
                        else
                        {
                            cI.NumberFormat.CurrencyDecimalSeparator = ",";
                            cI.NumberFormat.CurrencyGroupSeparator = ".";
                            cI.NumberFormat.NumberDecimalSeparator = ".";
                            cI.NumberFormat.NumberGroupSeparator = ",";
                        }
                       

                    }
                    foreach (CultureInfo cI in supportedCultures)
                    {
                        if(cI.TwoLetterISOLanguageName != "da") { 
                        cI.NumberFormat.CurrencyDecimalSeparator = ".";
                        cI.NumberFormat.CurrencySymbol = "€";
                        cI.NumberFormat.NumberDecimalSeparator = ".";
                        cI.NumberFormat.NumberGroupSeparator = ",";
                        }
                        else
                        {
                            cI.NumberFormat.CurrencyDecimalSeparator = ",";
                            cI.NumberFormat.CurrencyGroupSeparator = ".";
                            cI.NumberFormat.CurrencySymbol = "DKK";
                            cI.NumberFormat.NumberDecimalSeparator = ".";
                            cI.NumberFormat.NumberGroupSeparator = ",";
                        }
                    }
                    opts.DefaultRequestCulture = new RequestCulture("en-US");
                    opts.DefaultRequestCulture.Culture.NumberFormat.NumberDecimalSeparator = ".";
                    opts.DefaultRequestCulture.Culture.NumberFormat.CurrencyDecimalSeparator = ".";
                    opts.DefaultRequestCulture.Culture.NumberFormat.NumberGroupSeparator = ",";
                    // Formatting numbers, dates, etc.
                    opts.SupportedCultures = supportedCultures;
                    // UI strings that we have localized.
                    opts.SupportedUICultures = supportedUICultures;

                });
        }

        public void Configure(IApplicationBuilder app,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                //app.UseExceptionHandler("/Home/Error");
            }
            //app.UseStatusCodePages(); not removed for any particualy reason.
            app.UseSession();
            app.UseAuthentication();
            MyIdentityDataInitializer.SeedData(userManager, roleManager);
            RotativaConfiguration.Setup(env, "..\\wwwroot\\Rotativa\\");
            app.UseStaticFiles();
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseHttpsRedirection();
            app.UseRequestLocalization(locOptions.Value);
            //app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
            app.Use(async (context, next) =>
            {
                //context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' scripts.hw.onlineshop.com scripts.tjaden-onlineshop.nl; style-src 'self' 'unsafe-inline' styles.hw.onlineshop.com styles.tjaden-onlineshop.nl; img-src 'self' 'unsafe-inline' images.hw-onlineshop.com images.tjaden-onlineshop.nl; frame-ancestors 'none';");
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
    public static class MyIdentityDataInitializer
    {
        
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            //SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            List<string[]> rolelist = new List<string[]>();
            rolelist.Add(new string[] { "Admin", "DivisionAdmin", "Manager", "ProjectMember", "Member", "Supervisor", "StorageManager", "AlarmTeam", "ExternalDriller", "Board" });
            rolelist.Add(new string[] { "Admin", "DivisionAdmin", "Manager", "ProjectMember", "Member" });
            rolelist.Add(new string[] { "DivisionAdmin", "Manager", "ProjectMember" });
            rolelist.Add(new string[] { "Guest" });
            rolelist.Add(new string[] { "Guest", "MemberGuest" });
            rolelist.Add(new string[] { "Member" });
            rolelist.Add(new string[] { "ProjectMember" });
            rolelist.Add(new string[] { "Member", "ProjectMember" });
            rolelist.Add(new string[] { });
            rolelist.Add(new string[] { "ProjectMember" }); //inactive
            string pw = "testTest1!";
            int i = 1;
            foreach (var roles in rolelist)
            {
                if (userManager.FindByEmailAsync(String.Concat("test", i.ToString(), "@test.test")).Result == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = String.Concat("test", i.ToString()),
                        NormalizedUserName = String.Concat("TEST", i.ToString()),
                        Email = String.Concat("test", i.ToString(), "@test.test"),
                        NormalizedEmail = String.Concat("TEST", i.ToString(), "@TEST.TEST"),
                        EmailConfirmed = true,
                        PhoneNumber = "88888888",
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        LockoutEnd = null,
                        LockoutEnabled = false,
                        AccessFailedCount = 0,
                        FirstName = String.Concat("Test", i.ToString()),
                        LastName = String.Concat("Testesen", i.ToString()),
                        MemberShipConfirmed = true,
                        DivisionId = 4,
                        PicturePath = null,
                        UserLog = null,
                        Active = true
                    };
                    if (i == rolelist.Count())
                    {
                        user.Active = false;
                    }
                    var result = userManager.CreateAsync(user, pw).Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRolesAsync(user, roles).Wait();
                    }
                }
                i += 1;

            }
        }
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            Role[] roles = (Role[])Enum.GetValues(typeof(Role));
            foreach (var r in roles)
            {
                if (!roleManager.RoleExistsAsync
                (r.GetRoleName()).Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = r.GetRoleName();
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

    }
}
