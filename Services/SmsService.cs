namespace SimpleApi.Services;

public class SmsService : ISmsService
{
    public string Send(string phone, string message)
    {
        return $"SMS gönderildi → Numara: {phone}, Mesaj: {message}";
    }
}
