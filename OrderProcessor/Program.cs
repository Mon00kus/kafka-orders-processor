using OrderProcessor;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<OrderProcessingWorker>();

var host = builder.Build();
host.Run();