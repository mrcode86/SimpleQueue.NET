using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace SimpleQueue.Kafka.Test;

public class TestBase
{
    protected IConfiguration Configuration;
    protected string QueueConnectionString;

    [SetUp]
    public void Setup()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        QueueConnectionString = Configuration.GetConnectionString("MyConnectionString");
    }
}

public class TestMessage : BaseMessage
{
    public string? Text { get; set; }
}