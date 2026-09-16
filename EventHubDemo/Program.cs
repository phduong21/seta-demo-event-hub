using Azure.Messaging.EventHubs.Producer;
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

builder.Services.AddSingleton<IEventHubDiagnosticsService, EventHubDiagnosticsService>();
builder.Services.AddSingleton<IEventPublisherService, EventHubPublisherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
