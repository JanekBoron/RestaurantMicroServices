using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Restaurant.MessageBus;
using Restaurant.Services.OrderAPI;
using Restaurant.Services.OrderAPI.DbContexts;
using Restaurant.Services.OrderAPI.Extension;
using Restaurant.Services.OrderAPI.Messaging;
using Restaurant.Services.OrderAPI.RabbitMQSender;
using Restaurant.Services.OrderAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddHostedService<RabbitMQCheckoutConsumer>();
builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

builder.Services.AddSingleton(new OrderRepository(optionBuilder.Options));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();
builder.Services.AddSingleton<IRabbitMQOrderMessageSender,RabbitMQOrderMessageSender>();
builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options => {
    options.Authority = "https://localhost:44367/";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "restaurant");
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant.Services.CouponAPI", Version = "v1" });
   
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer",
                },
                Scheme="oauth2",
                Name="Bearer",
                In=ParameterLocation.Header
            },
            new List<String>()
        },
    });

});


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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseAzureServiceBusConsumer();
app.Run();
