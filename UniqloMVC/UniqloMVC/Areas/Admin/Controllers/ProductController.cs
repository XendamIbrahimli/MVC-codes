using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Reflection.Metadata;
using UniqloMVC.DataAccess;
using UniqloMVC.Extentions;
using UniqloMVC.Helpers;
using UniqloMVC.Models;
using UniqloMVC.ViewModel.Product;


namespace UniqloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=ConstantRoles.Product)]
    public class ProductController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Include(p=>p.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x=>!x.IsDeleted).ToListAsync();
            return View();  
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            if (vm.OtherFiles != null && vm.OtherFiles.Any())
            {
                if (vm.OtherFiles.All(x => x.IsValidType("Image")))
                {
                    var fileNames=vm.OtherFiles.Where(x => !x.IsValidType("Image")).Select(x => x.FileName);
                    ModelState.AddModelError("OtherFiles", string.Join(", ", fileNames)+ " are(is) not image");
                }
                if (vm.OtherFiles.All(x => x.IsValidSize(3000)))
                {
                    var fileNames=vm.OtherFiles.Where(x=>!x.IsValidSize(300)).Select(x => x.FileName);
                    ModelState.AddModelError("OtherFiles", string.Join(", ", fileNames)+ "must be less than 300kb");
                }
            }
            if (vm.CoverFile!=null)
            {
                if(!vm.CoverFile.IsValidType("image"))
                    ModelState.AddModelError("CoverFile", "File type must be an image");
                if (vm.CoverFile.IsValidSize(300))
                    ModelState.AddModelError("CoverFile", "File type must be less than 300 kb");
                
            }
            if(!ModelState.IsValid)
                return View();

            Product product = vm;//in Product
            product.CoverImage= await vm.CoverFile.UploadAsync("wwwroot", "imgs", "products");
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            foreach(var item in vm.OtherFiles)
            {
                var Filepath=Path.GetRandomFileName()+Path.GetExtension(item.FileName);
                using(Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath,"imgs","products",Filepath)))
                {
                    await item.CopyToAsync(stream);   
                }

                ProductImage productImage = new()
                {
                    FileUrl = Filepath,
                    ProductId = product.Id
                };

                await _context.ProductImages.AddAsync(productImage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
              return NotFound();
            
            var product= await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            
           _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            var productPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "products", product.CoverImage);
            if (System.IO.File.Exists(productPath))
            {
                System.IO.File.Delete(productPath);
            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, ProductCreateVM vm)
        {
            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                    ModelState.AddModelError("CoverFile", "File type must be an image");
                if (!vm.CoverFile.IsValidSize(300))
                    ModelState.AddModelError("CoverFile", "File type must be less than 300 kb");

            }
            if (!ModelState.IsValid)
                return View();

            var product = await _context.Products.FindAsync(id);

            var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "products", product.CoverImage);

            List<ProductImage> List = [];
            foreach (var item in vm.OtherFiles)
            {
                string fileName = await item.UploadAsync(_env.WebRootPath, "imgs", "products");
                List.Add(new ProductImage
                {
                    FileUrl=fileName,
                    Product=product
                });
            }
            if (System.IO.File.Exists(OldFilePath))
            {
                System.IO.File.Delete(OldFilePath);
            }

            string FileName= Path.GetRandomFileName() + Path.GetExtension(vm.CoverFile.FileName);
            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "products", FileName)))
            {
                await vm.CoverFile.CopyToAsync(stream);
            }

            product.Name=vm.Name;
            product.Description=vm.Description;
            product.CostPrice=vm.CostPrice;
            product.SellPrice=vm.SellPrice;
            product.Quantity=vm.Quantity;
            product.Discount=vm.Discount;
            product.CategoryId=vm.CategoryId;
            product.CoverImage=FileName;

            await _context.ProductImages.AddRangeAsync(List);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
