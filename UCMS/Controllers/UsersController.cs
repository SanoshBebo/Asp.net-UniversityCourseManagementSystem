// UserController.cs
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UCMS.Data;
using UCMS.Models.Domain;

public class UsersController : Controller
{
    private readonly UCMSDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public UsersController(UCMSDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Hash the password (you should use a password hashing library)
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create a new User entity
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = model.Name,
                Email = model.Email,
                Password = hashedPassword,
                Role = model.Role
            };

            // Add the user to the database
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Redirect to a success page or login page
            return RedirectToAction("Login");
        }

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                if (user.Role == "professor")
                {
                    return RedirectToAction("ProfessorHome");
                }
                else if (user.Role == "student")
                {
                    return RedirectToAction("StudentHome");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    public IActionResult ProfessorHome()
    {
        return View();
    }

    public IActionResult StudentHome()
    {
        return View();
    }
}
