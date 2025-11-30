namespace HomeSmartLink.Mobile.Services;

public class SecureStorageService : ISecureStorageService
{
    public async Task<string?> GetAsync(string key)
    {
        try
        {
            return await SecureStorage.Default.GetAsync(key);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task SetAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }

    public bool Remove(string key)
    {
        return SecureStorage.Default.Remove(key);
    }

    public void RemoveAll()
    {
        SecureStorage.Default.RemoveAll();
    }
}
