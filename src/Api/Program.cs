using System.Reflection;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.Configuration.SystemsManager;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Api.Extensions;
using Api.Infrastructure.Context;
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
// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDefaultAWSOptions(new AWSOptions
{
    Profile = "serverless",
});

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

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
var assemblies = GetAssembly();
foreach (var assembly in assemblies)
{
    builder.Services.AddClassesAsImplementedInterface(assembly, typeof(IConsumer<>), ServiceLifetime.Transient);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapEndpointsCore(AppDomain.CurrentDomain.GetAssemblies());

app.Run();

static IEnumerable<Assembly> GetAssembly()
{
    yield return typeof(Program).Assembly;
    yield return typeof(IConsumer<>).Assembly;
}