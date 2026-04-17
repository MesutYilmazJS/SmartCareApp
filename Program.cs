using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using SmartCareApp.Data;
using SmartCareApp.Services;

var builder = WebApplication.CreateBuilder(args);

var supportedCulture = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = supportedCulture;
CultureInfo.DefaultThreadCurrentUICulture = supportedCulture;

var mvcBuilder = builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

var defaultDatabasePath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "smartcareapp.db");
var connectionString = builder.Configuration.GetConnectionString("SmartCareDb");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = $"Data Source={defaultDatabasePath}";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ITriageRiskService, TriageRiskService>();
builder.Services.AddSingleton<IProfileImageCatalogService, ProfileImageCatalogService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.AccessDeniedPath = "/";
        options.Cookie.Name = "SmartCare.Auth";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(supportedCulture),
    SupportedCultures = new[] { supportedCulture },
    SupportedUICultures = new[] { supportedCulture }
};

app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

var profileImageCatalog = app.Services.GetRequiredService<IProfileImageCatalogService>();
var imagesRootPath = profileImageCatalog.GetImagesRootPath();

if (!string.IsNullOrWhiteSpace(imagesRootPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imagesRootPath),
        RequestPath = "/media"
    });
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

await using (var scope = app.Services.CreateAsyncScope())
{
    Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "App_Data"));

    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var triageRiskService = scope.ServiceProvider.GetRequiredService<ITriageRiskService>();
    var shouldSeedDemoData = app.Configuration.GetValue("SmartCare:SeedDemoData", true);

    await dbContext.Database.MigrateAsync();

    if (shouldSeedDemoData)
    {
        await DemoDataSeeder.SeedAsync(dbContext, triageRiskService);
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
