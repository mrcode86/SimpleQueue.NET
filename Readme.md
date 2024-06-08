# Queue Message Handling System

This repository contains a message handling system that interfaces with RabbitMQ to handle messages for different event types such as Added, Updated, and Deleted. It includes both the implementation and unit tests using .NET 8, ASP.NET Core, NUnit, and Moq.

## Project Structure

- `Queue`: Contains the core logic for message handling, including interfaces, models, and RabbitMQ implementation.
- `Queue.Consumers`: Contains the consumer implementations for different message types.
- `Queue.Models`: Contains the models for the different message types.
- `Queue.Tests`: Contains the unit tests for the message handling system.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [RabbitMQ](https://www.rabbitmq.com/)
- [NUnit](https://nunit.org/)
- [Moq](https://github.com/moq/moq4)

## Setup

1. **Install RabbitMQ**:
   Follow the instructions on the [RabbitMQ website](https://www.rabbitmq.com/download.html) to install RabbitMQ on your machine.

2. **Configure Connection String**:
   Update the `appsettings.json` file with your RabbitMQ connection string.

   ```json
   {
       "ConnectionStrings": {
           "RabbitMq": "your_rabbitmq_connection_string"
       }
   }

3. Install NuGet Packages:
Run the following commands in the Package Manager Console to install the required NuGet packages:
   ```sh
   Install-Package Microsoft.Extensions.DependencyInjection
   Install-Package Microsoft.Extensions.Logging
   Install-Package RabbitMQ.Client
   Install-Package NUnit
   Install-Package NUnit3TestAdapter
   Install-Package Moq
   ``` 

## Running the Tests
You can run the tests using the .NET CLI or Visual Studio.

To run the tests using the .NET CLI, navigate to the `Queue.Tests` directory and run the following command:

```sh
dotnet test
```

To run the tests using Visual Studio, open the Test Explorer and click on the `Run All Tests` button.

## Contributing
If you wish to contribute to this project, please fork the repository and submit a pull request. We welcome any contributions that improve the functionality, performance, or test coverage of the project.

## License
This project is licensed under the MIT License. See the [MIT](https://choosealicense.com/licenses/mit/) file for details.

## Contact
For any questions or issues, please open an issue in this repository.
