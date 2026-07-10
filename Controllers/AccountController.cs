using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebDevToCSharp.Services;
using WebDevToCSharp.ViewModels;

namespace WebDevToCSharp.Controllers;

public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;

    public AccountController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_userRepository.GetByEmail(model.Email) != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Email sudah terdaftar");
            return View(model);
        }

        var user = new Models.User
        {
            Username = model.Username,
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        _userRepository.Create(user);

        TempData["SuccessMessage"] = "Registrasi berhasil! Silakan login.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _userRepository.GetByEmail(model.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            ModelState.AddModelError(string.Empty, "Email atau password salah");
            return View(model);
        }

        // TODO: Generate token dan set cookie/session
        TempData["SuccessMessage"] = $"Selamat datang, {user.Username}!";
        return RedirectToAction("Index", "Home");
    }
}
