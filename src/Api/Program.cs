using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.Extensions.Configuration.SystemsManager;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Api.Extensions;
using Api.Infrastructure.Context;
using Api.Infrastructure.Middleware;
using Domain.Events;
using Domain.Events.Contracts;
using Domain.Options;
using Domain.Repositories;
using Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager(config =>
{
    config.Path = "/chat-api";
    config.ParameterProcessor = new JsonParameterProcessor();
    config.ReloadAfter = TimeSpan.FromMinutes(5);
    config.Optional = true;
});

builder.Services.Configure<EventBusSettings>(builder.Configuration.GetSection("EventBusSettings"));
builder.Services.Configure<AwsWebSocketAdapterConfig>(builder.Configuration.GetSection("AwsWebSocketAdapterConfig"));
builder.Services.Configure<ApiKeyValidationSettings>(builder.Configuration.GetSection("ApiKeyValidationSettings"));

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Logging.ClearProviders();
// Serilog configuration        
var logger = new LoggerConfiguration()
    .WriteTo.Console(new JsonFormatter())
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .CreateLogger();
// Register Serilog
builder.Logging.AddSerilog(logger);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var option = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(option);

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
builder.Services.AddAWSLambdaHosting(Environment.GetEnvironmentVariable("ApiGatewayType") == "RestApi" ? LambdaEventSource.RestApi : LambdaEventSource.HttpApi);

builder.Services.AddScoped<IClearRoomRepository, ClearRoomRepository>();
builder.Services.AddScoped<IDeletedMessageRepository, DeletedMessageRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomNotificationRepository, RoomNotificationRepository>();
builder.Services.AddScoped<IUserBanRepository, UserBanRepository>();
builder.Services.AddScoped<IUserRoomRepository, UserRoomRepository>();
builder.Services.AddScoped<IRoomLastActivityRepository, RoomLastActivityRepository>();
builder.Services.AddScoped<IApiContext, ApiContext>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IPubSubServices, PubSubService>();
builder.Services.AddScoped<IEventBusManager, EventBusManager>();
builder.Services.AddScoped<ApiKeyValidatorMiddleware>();
var assemblies = GetAssembly();
foreach (var assembly in assemblies)
{
    builder.Services.AddClassesAsImplementedInterface(assembly, typeof(IConsumer<>), ServiceLifetime.Transient);
}

var app = builder.Build();

app.UseMiddleware<ApiKeyValidatorMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => await Results.Problem().ExecuteAsync(context)));
}

app.UseHttpsRedirection();
app.MapEndpointsCore(AppDomain.CurrentDomain.GetAssemblies());

app.Run();

static IEnumerable<Assembly> GetAssembly()
{
    yield return typeof(Program).Assembly;
    yield return typeof(IConsumer<>).Assembly;
}