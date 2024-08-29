using CertificateAuth.Server.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
var builder = WebApplication.CreateBuilder(args);
//if(builder.Environment.IsProduction())
//builder.Configuration.AddAzureKeyVault(
//    new Uri("https://kvamalnidhi.vault.azure.net/"),
//    new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set the session timeout
    options.Cookie.HttpOnly = true; // Prevent client-side access to the cookie
    options.Cookie.IsEssential = true; // Make the session cookie essential
});



builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).AddCookie()
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "User.Read", "User.Read.All" })
    .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
    .AddInMemoryTokenCaches();


//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//        .AddMicrosoftIdentityWebApp(options =>
//        {
//            builder.Configuration.Bind("AzureAd",options);
//            options.Events ??= new OpenIdConnectEvents();
//            options.Events.OnRedirectToIdentityProvider += OnRedirectToIdentityProviderFunc;
//        })
//            .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "User.Read" })
//    .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
//    .AddInMemoryTokenCaches();


builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Use session
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CertificateAuth.Client._Imports).Assembly);


app.Map("/env", (context) =>
{
    var envName = builder.Environment.EnvironmentName;
    return context.Response.WriteAsync($"Environment: {envName}");
});

app.Run();

//async Task OnRedirectToIdentityProviderFunc(RedirectContext context)
//{
//    // Custom code here

//    // Don't remove this line
//    await Task.CompletedTask.ConfigureAwait(false);
//}