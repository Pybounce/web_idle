
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

public interface ICosmosClientFactory {
    public CosmosClient NewClient();
}

public class CosmosClientFactory: ICosmosClientFactory {
    
    private readonly string _connectionString;
    public CosmosClientFactory(string connectionString) {
        _connectionString = connectionString;
    }

    public CosmosClient NewClient() {
        //TokenCredential credential = new DefaultAzureCredential();
        return new CosmosClient(_connectionString);
    }

}