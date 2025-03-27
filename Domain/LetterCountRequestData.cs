using System.Text.Json;

namespace Domain;

public class LetterCountRequestData
{
    public LetterCountRequestData(string vkUserId, JsonDocument value)
    {
        Id = Guid.NewGuid();
        VkUserId = vkUserId;
        Value = value;
    }

    public Guid Id { get; set; }
    public string VkUserId { get; set; }
    public JsonDocument Value { get; set; }
    
}