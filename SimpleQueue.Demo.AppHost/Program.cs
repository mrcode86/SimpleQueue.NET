var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.AddRabbitMQ("rabbitMQ");

builder.AddProject<Projects.SimpleQueue_Demo_Web>("queue-demo-web").WithReference(rabbitMq);

builder.Build().Run();