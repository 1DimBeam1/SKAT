using Microsoft.EntityFrameworkCore;
using SKAT_Interface.Components;
using SKAT_Interface.Data;
using SKAT_Interface.Services;
using SKAT_Interface.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<DbTaskContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TaskDb")));

// Регистрация второго DbContext
builder.Services.AddDbContext<DbUsersContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UsersDb")));

builder.Services.AddAuthenticationCore();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SessionService>();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("ТребуетсяПреподаватель", policy => policy.RequireRole("Преподаватель"));
    options.AddPolicy("ТребуетсяОбучающийся", policy => policy.RequireRole("Обучающийся"));

    // Это говорит системе, что если требуется аутентификация,
    // и пользователь не аутентифицирован, то ничего не делать на уровне HTTP.
    // Blazor сам должен справиться с редиректом через AuthorizeRouteView.
    // Это может помочь избежать HTTP Challenge.
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Явно указываем, что для DefaultPolicy не должно быть Challenge.
    // Это более продвинутая настройка, может и не понадобиться.
    // options.InvokeHandlersAfterFailure = false; // По умолчанию true
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
//app.UseAuthentication(); // Если не используется Identity
app.UseAuthorization();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
