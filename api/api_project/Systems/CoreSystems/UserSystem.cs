
public interface IUserSystem {
    public Task<bool> TryCreate(UserCreate userCreate);
}

public class UserSystem: IUserSystem {
    private readonly IUserDb _userDb;

    public UserSystem(IUserDb userDb) {
        _userDb = userDb;
    }
    public async Task<bool> TryCreate(UserCreate userCreate) {
        return (await _userDb.TryCreate(userCreate)).IsSuccess;
    }

    // TODO: delete account

    // TODO: update password
}

