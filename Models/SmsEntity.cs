namespace SimpleApi.Models;

public class SmsEntity
{
    public int Id { get; set; }               // Primary Key
    public string Phone { get; set; } = "";   // Zorunlu
    public string Message { get; set; } = ""; // Zorunlu
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}