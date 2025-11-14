using WEBlaba2.Services;
using WEBlaba2.context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Сервисы

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlite("Data Source=WEBlaba2.db"));


builder.Services.AddDbContext<ClientContext>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddRazorPages();

// Сессии
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "RiveGauche.Session";
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // ← ДОЛЖНО БЫТЬ ПОСЛЕ UseRouting() И ДО UseAuthorization()
app.UseAuthorization();
app.MapRazorPages();

app.Run();