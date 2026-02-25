using MiniDatingApp.Components;
using MiniDatingApp.Data;
using Microsoft.EntityFrameworkCore;
using MiniDatingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<MatchService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.Urls.Add("http://0.0.0.0:8080");
}

// ?? IMPORTANT: Auto migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// ?? OPTIONAL: có th? gi? ho?c b?
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();