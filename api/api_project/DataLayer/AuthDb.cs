
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

public interface IAuthDb {
    public Task<bool> TryAuthenticate(UserLogin userLogin);
}

public class AuthDb: IAuthDb, IDisposable {
    
    private readonly CosmosClient _dbClient;

    public AuthDb(ICosmosClientFactory cosmosClientFactory) {
        _dbClient = cosmosClientFactory.NewClient();
    }

    public async Task<bool> TryAuthenticate(UserLogin userLogin) {
        var container = _dbClient.GetDatabase("main-db").GetContainer("main-container");
        var query = container.GetItemLinqQueryable<User>();
        var result = query.Where(x => x.Username == userLogin.Username && x.Password == userLogin.Password).Select(x => x.id).ToFeedIterator();
        while (result.HasMoreResults) {
            var page = await result.ReadNextAsync();
            if (page.Count == 1) {
                return true;
            }
            return false;
        }
        return false;
    }

    public void Dispose() {
        _dbClient.Dispose();
    }

}