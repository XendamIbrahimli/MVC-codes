
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using UniqloMVC.DataAccess;
using UniqloMVC.Helpers;
using UniqloMVC.Models;
using UniqloMVC.ViewModels.Slider;

namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =ConstantRoles.Slider)]
    public class SliderController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {            
            return View(await _context.Sliders.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateVM vm)
        {
            if (!vm.File.ContentType.Contains("image"))
                ModelState.AddModelError("File","File type must be image");
            if(vm.File.Length > 600 * 1024)
             ModelState.AddModelError("File", "File length must be less than 600kb");
            if (!ModelState.IsValid) return View();
             
            string newFileName=Path.GetRandomFileName()+Path.GetExtension(vm.File.FileName);

            
            
            using(Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs","sliders",newFileName)))
            {
               await vm.File.CopyToAsync(stream);
                
            }

            Slider slider = new Slider
            {
                ImageUrl = newFileName,
                Link = vm.Link,
                Subtitle = vm.Subtitle,
                Title = vm.Title,
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        
        public async Task<IActionResult> Delete(int? id)
        {
            var data= await _context.Sliders.FindAsync(id);
            _context.Sliders.Remove(data);
            await _context.SaveChangesAsync();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "sliders",data.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, SliderCreateVM vm)
        {
            if (!vm.File.ContentType.Contains("image"))
                ModelState.AddModelError("File", "File type must be image");
            if (vm.File.Length > 600 * 1024)
                ModelState.AddModelError("File", "File length must be less than 600kb");
            if (!ModelState.IsValid) return View();
            

            var data =await _context.Sliders.FindAsync(id);

            var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "sliders", data.ImageUrl);
            if(System.IO.File.Exists(OldFilePath))
            {
                System.IO.File.Delete(OldFilePath);
            }

            string NewFileName= Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);
            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", NewFileName)))
            {
                await vm.File.CopyToAsync(stream);

            }

            data.Title = vm.Title;
            data.Subtitle = vm.Subtitle;
            data.Link= vm.Link;
            data.ImageUrl = NewFileName;

            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Hide(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            slider.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Show(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            slider.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }
}
