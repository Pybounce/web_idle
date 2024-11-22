
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

public interface ICosmosClientFactory {
    public CosmosClient NewClient();
}

public class CosmosClientFactory: ICosmosClientFactory {
    
    private readonly string _connectionString;
    public CosmosClientFactory(IConfiguration configuration) {
        _connectionString = configuration.GetConnectionString("CosmosDb")!;
    }

    public CosmosClient NewClient() {
        return new CosmosClient(_connectionString);
    }

}