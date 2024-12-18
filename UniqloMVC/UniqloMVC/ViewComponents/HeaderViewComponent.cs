using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniqloMVC.DataAccess;
using UniqloMVC.ViewModel.Basket;

namespace UniqloMVC.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        readonly UniqloDbContext _context;

        public HeaderViewComponent(UniqloDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basketIds=JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");
            var prods=await _context.Products.Where(x => basketIds.Select(y => y.Id).Any(y => y == x.Id)).Select(x => new BasketHeaderItemVM
            {
                Id = x.Id,
                Discount = x.Discount,
                ImageUrl = x.CoverImage,
                SellPrice = x.SellPrice,
                Name = x.Name,
            }).ToListAsync();
            foreach (var product in prods)
            {
                product.Count=basketIds!.FirstOrDefault(x=>x.Id==product.Id)!.Count;
            }
            return View(prods);
        }
    }
}
