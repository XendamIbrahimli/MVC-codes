using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using UniqloMVC.DataAccess;
using UniqloMVC.Models;
using UniqloMVC.ViewModel.Basket;
using UniqloMVC.ViewModel.Details;

namespace UniqloMVC.Controllers
{
    public class ProductController : Controller
    {
        readonly UniqloDbContext _context;

        public ProductController(UniqloDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int Id)
        {
            var data = await _context.Products.Where(x => x.Id == Id && !x.IsDeleted).Include(x => x.Images)
                .Include(x=>x.Ratings).Include(x=>x.Comments).ThenInclude(x=>x.User).Select(x=>new ProductDetailsVM
            {
                ProductId=Id,
                Name = x.Name,
                Description = x.Description,
                Quantity = x.Quantity,
                SellPrice = x.SellPrice,
                CategoryId = x.CategoryId,
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                ImagesUrl=x.Images.Select(img=>img.FileUrl).ToList(),
                Ratings=x.Ratings.Select(rt=>rt.Rating).ToList(),
                Comments=x.Comments.Select(x=>new ProductComment
                {
                    Comment=x.Comment,
                    User=x.User,
                    CreatedTime=x.CreatedTime
                }).ToList()

            }).FirstOrDefaultAsync();

            ViewBag.Rating = 5;

            if(User.Identity?.IsAuthenticated ?? false)
            {
                string userId = User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier)?.Value;
                int rating=await _context.ProductRatings.Where(x=>x.UserId==userId && x.ProductId==Id).Select(x=>x.Rating).FirstOrDefaultAsync();
                
                ViewBag.Rating = rating== 0 ? 5 : rating;
            }

            return View(data);
        }
        

        public async Task<IActionResult> Rating(int productId, int rating)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var data = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();
            if(data is null)
            {
                await _context.ProductRatings.AddAsync(new Models.ProductRating
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = rating
                });
            }
            else
            {
                data.Rating = rating;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),new {id=productId});
        }

        public async Task<IActionResult> Comment(int productId, string comment)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var ExistUser=await _context.Users.Where(x=>x.Id==userId).FirstOrDefaultAsync();
            var data = await _context.ProductComments.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();
            if(data is null)
            {
                await _context.ProductComments.AddAsync(new Models.ProductComment
                {
                    User=ExistUser,
                    UserId = userId,
                    ProductId = productId,
                    Comment = comment,
                    CreatedTime=DateTime.Now
                });
            }
            else
            {
                data.Comment=comment;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new {Id=productId});
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            var basketItems = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");
            var item = basketItems.FirstOrDefault(x=>x.Id==id);
            if(item is null)
            {
                basketItems.Add(new BasketProductItemVM
                {
                    Count=1,
                    Id=id
                });
            }
            else
                item.Count++;


            Response.Cookies.Append("basket",JsonSerializer.Serialize(basketItems));
            return Ok();
        }
    }
}
