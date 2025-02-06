# PostProcessor Function App

This Azure Function processes data from an external API and stores it in Azure Storage Table.  It uses a composite key (PartitionKey and RowKey) to ensure uniqueness.

## Setup Instructions

1.  **Prerequisites:**
    *   Visual Studio 2022(Update to latest available version) or later (optional). 
    *   Make sure .NET SDK 8.0 runtime is installed
    *   Install Azure Functions Core Tools using Visual Studio 2022 Developer Command Prompt. You can install it using npm install -g azure-functions-core-tools@latest --unsafe-perm true
    *   Install Azurite (for local development). You can install it using npm: 'npm install -g azurite'
    *   Install Azure Storage Explorer. Download it from [https://azure.microsoft.com/en-us/products/storage/storage-explorer#Download-4]    
    
2.  **Clone the repository (or extract the ZIP file):**

    '''bash
    git clone <your_repository_url>
    '''

    or extract the ZIP file to your local machine.

3.  **Configure local settings:**
    *   Navigate to the 'DemoApp.Functions' project directory.
    *   Create a 'local.settings.json' file in the project directory if it doesn't exist.
    *   Add the following settings to 'local.settings.json' (replace placeholders with appropriate values):

    '''json
    {
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
        }
    }
    
    ***Important:** For local development, use '"UseDevelopmentStorage=true"'.  If you want to connect to a real Azure Storage account, replace it with your connection string.  Never commit real connection strings to your repository.

4.  **Run Azurite (for local development):**

    azurite --location %USERPROFILE%\.azurite --debug log.txt
    Keep this command prompt window open.

5.  **Run the Azure Function:**

    **Visual Studio:** Open the solution in Visual Studio and start debugging the 'DemoApp.Functions' project.
6.  **View data in Azure Storage Explorer:**

    *   Open Azure Storage Explorer.
    *   Connect to the local emulator. It should automatically detect and connect to Azurite running on the default port. If you are using custom ports, specify the connection string accordingly.
    *   You can now browse the Storage Table data created by your function.

## Code Explanation

This Azure Function fetches data from an external API, filters and transforms the data, and then stores the processed data in an Azure Storage Table.  
The function uses a composite key consisting of 'PartitionKey' and 'RowKey' to ensure that each record in the table is unique.  
The 'PartitionKey' allows for efficient querying and the 'RowKey' provides a unique identifier within the partition.

## To do for Production Improvements

**Authentication and Authorization:**  The external API should be secured with proper authentication and authorization mechanisms (e.g., API Keys, OAuth 2.0, Azure AD).
**Error Handling:** Implement more robust error handling, including retry policies with exponential backoff and circuit breakers to prevent cascading failures.  Log errors effectively using structured logging.
**Scalability:** Azure Functions scale automatically, but consider performance optimization techniques like caching.
**Configuration:** Store sensitive information (API keys, connection strings) securely in Azure Key Vault.
**CI/CD:** Implement a Continuous Integration/Continuous Deployment pipeline to automate the build, test, and deployment process.
**Unit and Integration Tests:** Write comprehensive unit and integration tests to cover all code paths and edge cases.
**Pagination**
