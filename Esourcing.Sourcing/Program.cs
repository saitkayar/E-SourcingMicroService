using Esourcing.Sourcing.Data;
using Esourcing.Sourcing.Repositories;
using Esourcing.Sourcing.Repositories.Interfaces;
using Esourcing.Sourcing.Settings;
using Esourcing.Sourcing.Settings.Interfaces;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Producer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<SourcingDatabaseSettings>(builder.Configuration.GetSection(nameof(SourcingDatabaseSettings)));
builder.Services.AddSingleton<ISourcingDatabaseSettings>(sp=>sp.GetRequiredService<IOptions<SourcingDatabaseSettings>>().Value);

builder.Services.AddTransient<ISourcingContext, SourcingContext>();
builder.Services.AddTransient<IAuctionRepository, AuctionRepository>();
builder.Services.AddTransient<IBidRepository, BidRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1",new OpenApiInfo { Title = "ESourcing.Sourcing", Version = "v1" });
});

#region eventbus

builder.Services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

    var factory = new ConnectionFactory()
    {
        HostName =builder.Configuration["EventBus:HostName"]
    };

    if (!string.IsNullOrWhiteSpace(builder.Configuration["EventBus:UserName"]))
    {
        factory.UserName = builder.Configuration["EventBus:UserName"];
    }

    if (!string.IsNullOrWhiteSpace(builder.Configuration["EventBus:Password"]))
    {
        factory.UserName = builder.Configuration["EventBus:Password"];
    }

    var retryCount = 5;
    if (!string.IsNullOrWhiteSpace(builder.Configuration["EventBus:RetryCount"]))
    {
        retryCount = int.Parse(builder.Configuration["EventBus:RetryCount"]);
    }

    return new DefaultRabbitMQPersistentConnection(factory, retryCount, logger);
});
builder.Services.AddSingleton<EventBusRabbitMQProducer>();



#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();


app.Run();

