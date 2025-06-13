using SKAT_Interface.Components;
using SKAT_Interface.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped<AlgorithmApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection(); // ≈сли InterpretatorService работает по HTTP, а Blazor по HTTPS,
                            //могут быть проблемы с mixed content.
                            // ƒл€ локальной разработки с HTTP API можно временно закомментировать
                            // или настроить API на HTTPS.
                            // ѕока что, если API на HTTP, а Blazor пытаетс€ на HTTPS, будут проблемы.
                            // ≈сли оба на HTTP локально, эту строку можно закомментировать дл€ простоты.

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
