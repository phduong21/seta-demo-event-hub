using Azure.Messaging.EventHubs.Producer;
using EventHubDemo.Configuration;
using EventHubDemo.Interfaces;
using EventHubDemo.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
