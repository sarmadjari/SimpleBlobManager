using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add Azure AD authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Register AzureBlobService with dependencies
builder.Services.AddSingleton<AzureBlobService>(serviceProvider =>
{
    var auditLogService = serviceProvider.GetRequiredService<AuditLogService>();
    var blobServiceEndpoint = "https://simplestoragemanager.blob.core.windows.net";
    return new AzureBlobService(blobServiceEndpoint, auditLogService);
});

// Register AuditLogService
builder.Services.AddSingleton<AuditLogService>(serviceProvider =>
{
    var tableEndpoint = "https://simplestoragemanager.table.core.windows.net";
    return new AuditLogService(tableEndpoint);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute(); // Maps controller actions using default routing

app.Run();