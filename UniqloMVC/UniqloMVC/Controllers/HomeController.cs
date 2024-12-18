using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UniqloMVC.DataAccess;
using UniqloMVC.ViewModel;
using UniqloMVC.ViewModel.Common;
using UniqloMVC.ViewModel.Product;
using UniqloMVC.ViewModel.Slider;

namespace UniqloMVC.Controllers
{
    public class HomeController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            HomeVM vm= new HomeVM();
            vm.Sliders= await _context.Sliders
                .Where(x => !x.IsDeleted)
                .Select(x => new SliderItemVM
                {
                    ImageUrl = x.ImageUrl,
                    Link = x.Link,
                    Subtitle = x.Subtitle,
                    Title = x.Title
                }).ToListAsync();
            vm.Products = await _context.Products
                .Where(x => !x.IsDeleted)
                .Select(x => new ProductItemVM
                {
                    Discount = x.Discount,
                    Id = x.Id,
                    ImageUrl = x.CoverImage,
                    IsInStock = x.Quantity > 0,
                    Name = x.Name,
                    Price = x.SellPrice
                }).ToListAsync();
            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}

