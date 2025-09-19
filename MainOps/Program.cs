using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Azure.Identity;
using MainOps.Configuration;
//using Pomelo.EntityFrameworkCore.MySql.Data.MySqlClient;
using MainOps.Data;
using MainOps.ExtensionMethods;
using MainOps.Models;
using MainOps.Resources;
using MainOps.Services;
using Microsoft.AspNetCore.Builder;
//using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
//using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
//using Pomelo.EntityFrameworkCore.MySql;
using MySqlConnector;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using Azure.Identity;


var builder = WebApplication.CreateBuilder(args);

// Bind the "GoogleMaps" section from config to GoogleMapsSettings
builder.Services.Configure<GoogleMapsSettings>(
    builder.Configuration.GetSection("GoogleMaps"));

// Load Key Vaults from configuration
var vaultSecretUri = builder.Configuration["KeyVault:SecretUri"];
var vaultClientId = builder.Configuration["KeyVault:ClientId"];
var vaultClientSecret = builder.Configuration["KeyVault:ClientSecret"];


// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add HTTP logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
});

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});


#if DEBUG
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
#else
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif

// Update JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });


try
{ // Load configuration
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddAzureKeyVault(vaultSecretUri, vaultClientId, vaultClientSecret)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();
}
catch (Exception ex)
{
    Console.WriteLine($"Key Vault error: {ex.Message}");
    // Optionally log to Application Insights
}



if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<MyAppSecretConfig>();
}



//Must uncomment on Release
//builder.AddAzureKeyVault(
//    configuration["KeyVault:SecretUri"],
//    configuration["KeyVault:ClientId"],
//    configuration["KeyVault:ClientSecret"]);
//var dom = builder.Build();
//Configuration = dom;



builder.Services.AddSingleton<LocService>();
builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(3600));
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Update security settings
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// Update HTTPS settings
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    new MySqlConnectionStringBuilder
    {
        Server = "handdipnew.mysql.database.azure.com",
        Database = "handdipnew",
        UserID = "root2@handdipnew",
        //Password = Configuration["conPass"],
        Password = "qz11ds15QZ11DS15",
        SslMode = MySqlSslMode.Required,
        AllowPublicKeyRetrieval = true // Added for newer MySQL versions
    }.ConnectionString;


builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure();
            mySqlOptions.EnableStringComparisonTranslations();
            // Add this to handle null values better
            //mySqlOptions..UseNewtonsoftJson(new JsonSerializerSettings
            //{
            //    NullValueHandling = NullValueHandling.Ignore
            //});
        }));



// Update Identity configuration with modern security defaults
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequiredLength = 12; // Increased from 8
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5; // Reduced from 10
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// Update authentication with modern defaults
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Reduced from 150
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("AuthMessageSenderOptions"));
builder.Services.Configure<AuthSFTPOptions>(builder.Configuration.GetSection("AuthSFTPOptions"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews(options =>
{
    options.EnableEndpointRouting = true;
})
.AddViewLocalization()
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) =>
    {
        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
        return factory.Create("SharedResource", assemblyName.Name);
    };
});

builder.Services.AddKendo();

builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    var supportedUICultures = new List<CultureInfo>
    {
        new("en-US"), new("de-DE"), new("da-DK"), new("nl-NL")
    };

    var supportedCultures = new List<CultureInfo>
    {
        new("en-US"), new("de-DE"), new("da-DK"), new("nl-NL")
    };

    foreach (var culture in supportedUICultures)
    {
        if (culture.TwoLetterISOLanguageName != "da")
        {
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.NumberGroupSeparator = ",";
        }
        else
        {
            culture.NumberFormat.CurrencyDecimalSeparator = ",";
            culture.NumberFormat.CurrencyGroupSeparator = ".";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.NumberGroupSeparator = ",";
        }
    }

    opts.DefaultRequestCulture = new RequestCulture("en-US");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedUICultures;
});

// Update session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add memory cache
builder.Services.AddMemoryCache();

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter",
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});


var app = builder.Build();


// Correct setup (since exe is directly under /Rotativa)
//var rootPath = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
//RotativaConfiguration.Setup(rootPath, "Rotativa");

// Guarantee a valid wwwroot path
var rootPath = app.Environment.WebRootPath;
if (string.IsNullOrEmpty(rootPath))
{
    // Fallback for Azure or when WebRootPath is null
    rootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
}

RotativaConfiguration.Setup(rootPath, "Rotativa");
app.UseStaticFiles();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpLogging();
app.UseResponseCompression();
app.UseRateLimiter();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 30; // 30 days
        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});
app.UseRouting();
app.UseSession();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] =
        "accelerometer=(), camera=(), geolocation=(self), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
    
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://maps.googleapis.com https://maps.gstatic.com https://ajax.googleapis.com https://cdnjs.cloudflare.com https://www.gstatic.com/charts/; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://ajax.googleapis.com https://www.gstatic.com/charts/; " +
        "img-src 'self' data: https://maps.gstatic.com https://maps.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "connect-src 'self' https://maps.googleapis.com https://maps.gstatic.com; " +
        "frame-src 'self' https://www.google.com; " +
        "img-src 'self' data: https://maps.googleapis.com https://maps.gstatic.com;";

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    MyIdentityDataInitializer.SeedData(userManager, roleManager);
    MyIdentityDataInitializer.SeedData(roleManager);
}

// Update Rotativa configuration for .NET 9
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// Add health check endpoint
//app.MapHealthChecks("/health");

// Add exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = app.Environment.IsDevelopment() ? ex.ToString() : "An error occurred"
        });
    }
});

// Add graceful shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    // Perform cleanup
    Thread.Sleep(TimeSpan.FromSeconds(5));
});

//debug not production
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Your error handler route
    app.UseHsts();
}

app.Run();

public static class MyIdentityDataInitializer
{
    public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        SeedRoles(roleManager);
    }
    public static void SeedData(RoleManager<IdentityRole> roleManager)
    {
        SeedRoles(roleManager);
    }

    public static void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        Role[] roles = (Role[])Enum.GetValues(typeof(Role));
        foreach (var r in roles)
        {
            if (!roleManager.RoleExistsAsync(r.GetRoleName()).Result)
            {
                var role = new IdentityRole { Name = r.GetRoleName() };
                roleManager.CreateAsync(role).Wait();
            }
        }
    }
}
