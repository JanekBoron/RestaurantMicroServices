using PaymentProcessor;
using Restaurant.MessageBus;
using Restaurant.Services.PaymentAPI.Extension;
using Restaurant.Services.PaymentAPI.Messaging;
using Restaurant.Services.PaymentAPI.RabbitMQSender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();
builder.Services.AddSingleton<IRabbitMQPaymentMessageSender,RabbitMQPaymentMessageSender>();
builder.Services.AddSingleton<RabbitMQPaymentConsumer>();
;builder.Services.AddControllersWithViews();


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
