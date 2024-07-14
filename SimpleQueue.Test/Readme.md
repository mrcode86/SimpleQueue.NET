# SimpleQueue Tests

This repository contains unit tests for the `MessageHandler` classes using NUnit and Moq. The tests ensure that the message queue handler starts and stops correctly and that the message handler interacts with the message queue as expected.

## Project Structure

- `SimpleQueue.Tests`: Contains the unit tests for the message queue and message handler classes.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [NUnit](https://nunit.org/)
- [Moq](https://github.com/moq/moq4)

## Setup

To set up the project, ensure you have the necessary .NET SDK installed. You can install the required NuGet packages by running the following commands in the Package Manager Console:

```sh
Install-Package NUnit
Install-Package NUnit3TestAdapter
Install-Package Moq
```

## Running the Tests
You can run the tests using the .NET CLI or Visual Studio.

To run the tests using the .NET CLI, navigate to the `SimpleQueue.Tests` directory and run the following command:

```sh
dotnet test
```

To run the tests using Visual Studio, open the Test Explorer and click on the `Run All Tests` button.

## License
This project is licensed under the MIT License. See the [MIT](https://choosealicense.com/licenses/mit/) file for details.


