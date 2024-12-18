using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniqloMVC.Helpers;

namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =ConstantRoles.Dashboard)]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
