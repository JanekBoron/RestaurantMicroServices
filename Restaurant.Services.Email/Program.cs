using Microsoft.EntityFrameworkCore;
using Restaurant.Services.Email.DbContexts;
using Restaurant.Services.Email.Messaging;
using Restaurant.Services.Email.Extension;
using Restaurant.Services.Email.Messaging;
using Restaurant.Services.Email.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddTransient<IEmailRepository, EmailRepository>();
builder.Services.AddHostedService<RabbitMQPaymentConsumer>();
var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailRepository(optionBuilder.Options));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAzureServiceBusConsumer();

app.Run();
