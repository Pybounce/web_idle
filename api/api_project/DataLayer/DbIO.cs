
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

public interface IDbIO {

}

public class DbIO: IDbIO {
    
    private readonly CosmosClient _dbClient;

    public DbIO() {
        _dbClient = CreateDbClient();
    }

    private CosmosClient CreateDbClient() {
        TokenCredential credential = new DefaultAzureCredential();
        return new CosmosClient("AccountEndpoint=https://skybounce-cosmosdb-web-idle.documents.azure.com:443/;AccountKey=E3HOo5nH8lHHXkK7h0icLPmJkSa5bJUZE9Nwuhz2KuYuidpIY8DA0xQpZZ29Mv7THlDKdhawJp0TACDbmo0fTA==;", credential);
    }

}