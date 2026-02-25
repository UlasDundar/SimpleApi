using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models;

public class SendSmsRequest
{
    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [MinLength(10, ErrorMessage = "Telefon numarası en az 10 karakter olmalı.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj zorunludur.")]
    [MinLength(3, ErrorMessage = "Mesaj en az 3 karakter olmalı.")]
    public string Message { get; set; } = string.Empty;
}
