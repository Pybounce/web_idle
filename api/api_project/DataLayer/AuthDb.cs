
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

public interface IAuthDb {
    public Task<bool> TryAuthenticate(UserLogin userLogin);
    public Task<bool> TryCreateAccount(UserCreate userCreate);

}

public class AuthDb: IAuthDb, IDisposable {
    
    private readonly CosmosClient _dbClient;

    public AuthDb(ICosmosClientFactory cosmosClientFactory) {
        _dbClient = cosmosClientFactory.NewClient();
    }

    public async Task<bool> TryAuthenticate(UserLogin userLogin) {
        var container = _dbClient.GetDatabase("main-db").GetContainer("users");
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
    public async Task<bool> TryCreateAccount(UserCreate userCreate) {
        if (await IsUsernameTaken(userCreate.Username)) { return false; }

        var container = _dbClient.GetDatabase("main-db").GetContainer("users");
        var newUser = new User() {
            id = userCreate.Username,   //TODO: Make idCreation or something
            Username = userCreate.Username,
            Password = userCreate.Password
        };
        await container.UpsertItemAsync<User>(newUser);
        return true;
    }

    public async Task<bool> IsUsernameTaken(string username) {
        var container = _dbClient.GetDatabase("main-db").GetContainer("users");
        var query = container.GetItemLinqQueryable<User>();
        var result = query.Where(x => x.Username == username).ToFeedIterator();
        while (result.HasMoreResults) {
            var page = await result.ReadNextAsync();
            return page.Count > 0;
        }
        return false;
    }

    public void Dispose() {
        _dbClient.Dispose();
    }

}