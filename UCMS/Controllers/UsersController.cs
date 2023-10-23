using Microsoft.AspNetCore.Mvc;

namespace UCMS.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        
    }
}
