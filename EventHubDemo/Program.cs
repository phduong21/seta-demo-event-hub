using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using EventHubDemo.Configuration;
using EventHubDemo.Interfaces;
using EventHubDemo.Models.Responses;
using EventHubDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = string.Join("; ", context.ModelState.Values
            .SelectMany(state => state.Errors)
            .Select(error => error.ErrorMessage));

        return new BadRequestObjectResult(ApiResponse<object>.Fail(errors));
    };
});

builder.Services
    .AddOptions<EventHubOptions>()
    .Bind(builder.Configuration.GetSection(EventHubOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<EventHubOptions>>().Value;
    return new EventHubProducerClient(options.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var eventHub = sp.GetRequiredService<IOptions<EventHubOptions>>().Value;
    var storageConnection = builder.Configuration["Storage:ConnectionString"];

    var container = new BlobContainerClient(storageConnection, "checkpoints");
    container.CreateIfNotExists();

    return new EventProcessorClient(container, eventHub.ConsumerGroup, eventHub.ConnectionString);
});

builder.Services.AddSingleton<IEventHubDiagnosticsService, EventHubDiagnosticsService>();
builder.Services.AddSingleton<IEventPublisherService, EventHubPublisherService>();
builder.Services.AddSingleton<EventDispatcher>();
builder.Services.AddHostedService<EventHubConsumerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
