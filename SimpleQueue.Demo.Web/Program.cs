using SimpleQueue.Demo.ServiceDefaults;
using SimpleQueue.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddLogging(configure => configure.AddConsole());

// Add RabbitMq
builder.AddRabbitMQClient("rabbitMQ", null, static factory => factory.DispatchConsumersAsync = true);

// Auto register all message handlers
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.RegisterQueueHandlersAndServices(assemblies);

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();