namespace WebDevToCSharp.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username wajib diisi")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username harus antara 3-50 karakter")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password wajib diisi")]
    [Compare("Password", ErrorMessage = "Password dan Confirm Password tidak sama")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi")]
    public string Password { get; set; } = string.Empty;
}
