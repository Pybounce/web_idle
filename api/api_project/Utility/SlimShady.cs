

public class SlimShady {
    private SemaphoreSlim _semaphore;
    public SlimShady() {
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async Task LockAsync(Func<Task> handle) {
        await _semaphore.WaitAsync();
        try {
            await handle();
        }   
        finally {
            _semaphore.Release();
        }
    }
    public async Task<T> LockAsync<T>(Func<Task<T>> handle)
    {
        await _semaphore.WaitAsync();
        try {
            return await handle();
        }
        finally {
            _semaphore.Release();
        }
    }
}