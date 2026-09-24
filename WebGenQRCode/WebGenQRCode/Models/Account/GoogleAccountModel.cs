using System.Text.Json.Serialization;

namespace WebGenQRCode.Models.Account;

//Модель для збереження даних користувача,
//який авторизувався через гугл
public class GoogleAccountModel
{
    [JsonPropertyName("id")]
    public string GoogleId { get; set; } = string.Empty;
    [JsonPropertyName("given_name")]
    public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("family_name")]
    public string LastName { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("picture")]
    public string Picture { get; set; } = string.Empty;
}
