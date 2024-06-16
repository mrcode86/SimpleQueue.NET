using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Amazon.SQS;

namespace Queue.AmazonSQS.Test;

public class SetupTests : TestBase
{
    private IServiceCollection _services;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
    }

    [Test]
    public void ConfigureAmazonSqs_ShouldRegisterServices()
    {
        _services.ConfigureAmazonSqs(QueueConnectionString);

        var provider = _services.BuildServiceProvider();

        var sqsClient = provider.GetService<IAmazonSQS>();
        Assert.That(sqsClient, Is.Not.Null, "IAmazonSQS should be registered.");

        var queue = provider.GetService<IMessageQueue<TestMessage>>();
        Assert.That(queue, Is.Not.Null, "IMessageQueue<TestMessage> should be registered.");
    }

    [Test]
    public void ConfigureAmazonSqs_ShouldRegisterConsumers()
    {
        _services.ConfigureAmazonSqs(QueueConnectionString, true);

        var provider = _services.BuildServiceProvider();

        var hostedServices = provider.GetServices<IHostedService>().ToList();
        Assert.That(hostedServices.Any(), Is.True, "IHostedService should be registered when consumers are activated.");

        // Log the types of hosted services to debug
        Console.WriteLine($"Total hosted services: {hostedServices.Count}");
        foreach (var service in hostedServices)
        {
            Console.WriteLine(service.GetType().FullName);
        }

        Assert.That(hostedServices.Any(), Is.True);
    }
}