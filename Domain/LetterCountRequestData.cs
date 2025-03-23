using System.Text.Json;

namespace Domain;

public class LetterCountRequestData
{
    public LetterCountRequestData(string vkUserId, string accessToken, JsonDocument value)
    {
        Id = Guid.NewGuid();
        VkUserId = vkUserId;
        AccessToken = EncryptionHelper.Encrypt(accessToken);
        Value = value;
    }

    public Guid Id { get; set; }
    public string VkUserId { get; set; }
    public string AccessToken { get; set; }
    public JsonDocument Value { get; set; }
    
    public string GetDecryptedAccessToken() => EncryptionHelper.Decrypt(AccessToken);
}