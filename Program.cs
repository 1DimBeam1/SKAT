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
builder.Services.AddScoped<AlgorithmApiService>();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddHttpClient("EvaluationApiClient", client =>
{
    // Базовый URL вашего сервиса оценивания
    // Это значение лучше вынести в appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["EvaluationService:BaseUrl"]);
    // Можно добавить заголовки по умолчанию, если нужны (например, Content-Type)
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("ApiSettings", client =>
{
    // Базовый URL вашего сервиса оценивания
    // Это значение лучше вынести в appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:InterpretatorServiceUrl"]);
    // Можно добавить заголовки по умолчанию, если нужны (например, Content-Type)
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<EvaluationApiService>();

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
app.UseRouting(); // Убедитесь, что UseRouting есть перед UseEndpoints/MapGet

// Добавьте это перед app.MapRazorComponents<App>()
app.Use(async (context, next) =>
{
    // Перенаправляем только для корневого пути и если пользователь не аутентифицирован
    // Проверка аутентификации здесь может быть сложной, так как ClaimsPrincipal еще может быть не установлен
    // до того, как отработает Blazor. Проще всего перенаправлять всегда с корня на /login,
    // а /login уже сам решит, нужно ли перенаправлять аутентифицированного пользователя дальше.
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/login", permanent: false); // false для временного редиректа
        return; // Важно завершить обработку запроса здесь
    }
    await next();
});
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
