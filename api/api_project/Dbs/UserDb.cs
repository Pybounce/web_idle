using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.VisualBasic;

public interface IUserDb {
    public Task<bool> IsUsernameTaken(string username);
    public Task<(bool IsSuccess, User? Result)> TryCreate(UserCreate userCreate);
    public Task<(bool IsSuccess, string ResultId)> TryGetIdFromCredentials(string username, string password);
}

public class UserDb: IUserDb, IDisposable {

    private readonly CosmosClient _dbClient;
    private readonly Container _userContainer;
    public UserDb(ICosmosClientFactory cosmosClientFactory) {
        _dbClient = cosmosClientFactory.NewClient();
        _userContainer = _dbClient.GetDatabase("main-db").GetContainer("users");
    }

    public async Task<bool> IsUsernameTaken(string username) {
        var query = _userContainer.GetItemLinqQueryable<User>();
        var result = query.Where(x => x.Username == username).ToFeedIterator();
        while (result.HasMoreResults) {
            var page = await result.ReadNextAsync();
            return page.Count > 0;
        }
        return false;
    }

    public async Task<(bool IsSuccess, User? Result)> TryCreate(UserCreate userCreate) {
        if (await IsUsernameTaken(userCreate.Username)) { return (false, null); }

        var newUser = new User() {
            id = userCreate.Username,   //TODO: Make idCreation or something
            Username = userCreate.Username,
            Password = userCreate.Password
        };
        await _userContainer.UpsertItemAsync<User>(newUser);
        // TODO: Add additional check after inserting that there still is only one, for race conditions
        // Cosmos has a unique key field but it's only unique per partition
        return (true, newUser);
    }

    public async Task<(bool IsSuccess, string ResultId)> TryGetIdFromCredentials(string username, string password) {
        var query = _userContainer.GetItemLinqQueryable<User>();
        var result = query.Where(x => x.Username == username && x.Password == password).ToFeedIterator();
        while (result.HasMoreResults) {
            var page = await result.ReadNextAsync();
            if (page.Count <= 0) { return (false, String.Empty); }
            else if (page.Count > 1) { throw new Exception("Multiple users found"); }   //ToDo exception types and handling
            
            return (true, page.Resource.First().id);
        }
        return (false, String.Empty);
    }

    public void Dispose() {
        _dbClient.Dispose();
    }
}



