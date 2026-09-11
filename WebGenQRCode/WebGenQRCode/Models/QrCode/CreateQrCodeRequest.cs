namespace WebGenQRCode.Models.QrCode;

public class CreateQrCodeRequest
{
    //Назва QR-коду
    public string Name { get; set; } = null!;
    //Посилання на яке буде здійснено редірект після сканування QR-коду
    public string TargetUrl { get; set; } = null!;
}
