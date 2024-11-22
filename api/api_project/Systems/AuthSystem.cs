public interface IAuthSystem {
    public Task<bool> TryAuthenticate(UserLogin userLogin);
}

public class AuthSystem: IAuthSystem {

    private readonly IUserDb _userDb;

    public AuthSystem(IUserDb userDb) {
        _userDb = userDb;
    }
    public async Task<bool> TryAuthenticate(UserLogin userLogin) {
        return (await _userDb.TryGetIdFromCredentials(userLogin.Username, userLogin.Password)).IsSuccess;
    }
    // TODO: rename above to GetToken

    // TODO: make new called GetCredentialsFromToken
        // may involve making a UserCredentials object

}



