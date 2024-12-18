using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAccess;
using UniqloMVC.Helpers;
using UniqloMVC.Models;
using UniqloMVC.ViewModel.Category;

namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =ConstantRoles.Category)]
    public class CategoryController(UniqloDbContext _context ):Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }
        
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if(!ModelState.IsValid)
                return View();

            Category category = new()
            {
                Name = vm.Name
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int Id)
        {
            bool isReferenced=await _context.Products.AnyAsync(p=>p.CategoryId==Id);
            if (isReferenced)
            {
                TempData["Error"] = "Cannot delete the category because it is refernced by products";
                return RedirectToAction(nameof(Index));
            }

            var data = await _context.Categories.FindAsync(Id);
            if (data != null)
            {
                _context.Categories.Remove(data);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id,CategoryCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View();

            var data=await _context.Categories.FindAsync(id);


            data.Name = vm.Name;
            await _context.SaveChangesAsync();
            

            return RedirectToAction(nameof(Index));
        } 
    }
}
