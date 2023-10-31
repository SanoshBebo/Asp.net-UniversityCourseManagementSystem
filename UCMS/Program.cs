using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using UCMS.Data;
using UCMS.Models.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UCMSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UCMSConnectionString")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Register the RoleManager
    .AddEntityFrameworkStores<UCMSDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProfessorPolicy", policy => policy.RequireRole("professor"));
    options.AddPolicy("StudentPolicy", policy => policy.RequireRole("student"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Create roles if they don't exist
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "admin", "professor", "student" };

    foreach (var roleName in roles)
    {
        if (!roleManager.RoleExistsAsync(roleName).Result)
        {
            var role = new IdentityRole(roleName);
            var result = roleManager.CreateAsync(role).Result;
            if (!result.Succeeded)
            {
                // Handle role creation errors
                throw new Exception($"Error creating the {roleName} role.");
            }
        }
    }
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
