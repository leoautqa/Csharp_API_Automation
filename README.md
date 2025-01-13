# API Test Automation with C#

This project implements API test automation using **C#**, **Visual Studio**, **SpecFlow**, and **RestSharp**. It follows Behavior-Driven Development (BDD) principles, enabling clear and structured test definitions. 

## Features

- **BDD Implementation**: Utilizing SpecFlow to define feature files and step definitions.
- **API Testing**: Seamless integration with RestSharp for crafting and sending HTTP requests.
- **Reporting**: ExtentReports for detailed test execution reports.
- **Data Generation**: Using Bogus for mock data creation.
- **Assertions**: FluentAssertions for clean and readable test validations.

## Prerequisites

1. **Visual Studio 2022** or later.
2. **.NET SDK 9.0** or compatible.
3. Basic knowledge of C# and SpecFlow.

## Installation

1. Clone the repository:
   ```bash
   git clone <repository_url>
   cd <repository_folder>
   ```
2. Open the project in Visual Studio.
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## Project Dependencies

The project relies on the following NuGet packages:

- **Bogus**: Data generation library.
- **ExtentReports**: Test reporting framework.
- **RestSharp**: Simplified HTTP client.
- **SpecFlow**: BDD framework for .NET.
- **NUnit**: Test framework.
- **FluentAssertions**: Assertion library for tests.

Full dependency details can be found in the `csproj` file.

## Project Structure

```plaintext
|-- Common
|   |-- CommonSteps.cs
|   |-- extendReport.cs
|   |-- hooks.cs
|   |-- variables.cs
|-- Features
|   |-- carrinho.feature
|   |-- login.feature
|   |-- Produtos.feature
|   |-- Usuarios.feature
|-- StepDefinitions
|   |-- carrinho_po.cs
|   |-- login_po.cs
|   |-- produtos.cs
|   |-- usuarios_po.cs
|-- Reports
|   |-- TestResults            # Location for generated reports
```

## Configuration

### ExtentReports Configuration

Reports are generated in the `TestResults` folder within the project directory. Modify the `TestResults` path in `extendReport.cs` if a custom location is needed.

### Running Tests

1. Build the project:
   ```bash
   dotnet build
   ```
2. Execute tests using the Test Explorer in Visual Studio or the `dotnet` CLI:
   ```bash
   dotnet test
   ```
3. Reports will be generated automatically after test execution.

## Writing Tests

1. Define a feature in a `.feature` file using Gherkin syntax.
   Example:
   ```gherkin
   Feature: User Management
       Scenario: Create a new user
           Given I have valid user data
           When I send a POST request to create the user
           Then the response status code should be 201
   ```

2. Implement the step definitions in a `.cs` file:
   ```csharp
   [Given("I have valid user data")]
   public void GivenIHaveValidUserData() {
       // Initialize user data
   }

   [When("I send a POST request to create the user")]
   public void WhenISendAPostRequestToCreateTheUser() {
       // Send POST request
   }

   [Then("the response status code should be 201")]
   public void ThenTheResponseStatusCodeShouldBe201() {
       // Assert the status code
   }
   ```

## Reporting

Reports are generated using **ExtentReports**. After running tests, check the `TestResults` folder for `AutomationStatusReport.html` for detailed results.

## Acknowledgments

- [SpecFlow Documentation](https://specflow.org/documentation/)
- [RestSharp Documentation](https://restsharp.dev/)
- [ExtentReports Documentation](https://www.extentreports.com/)

---

Happy Testing! 🚀
