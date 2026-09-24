namespace WebGenQRCode.Models.Account;

public class GoogleLoginRequestModel
{
    //Від гугл приходить токен,
    //який ми відправляємо на сервер для валідації
    public string Token { get; set; } = null!;
}
