using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_Utility;
using System.Data;
using Rocky_DataAccess;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_DataAccess.Repository.IRepository;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objlist = _prodRepo.GetAll(includeProperties: "Category,ApplicationType");
            return View(objlist);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName),
                ApplicationTypeSelectList = _prodRepo.GetAllDropdownList(WC.ApplicationTypeName)
            };

            if (id != null)
            {
                productVM.Product = _prodRepo.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
            }

            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
                return NotFound();

            Product product = _prodRepo.FirstOrDefault(u => u.Id == id, includeProperties: "Category,ApplicationType");

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodRepo.Find(id.GetValueOrDefault());
            if(obj == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath; 
            var oldFile = Path.Combine(upload, obj.Image); // полная строка имени файла

            if (System.IO.File.Exists(oldFile)) // проверка существования файла и его удаление
            {
                System.IO.File.Delete(oldFile);
            }

            _prodRepo.Remove(obj);
            _prodRepo.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //токен защиты от взлома
        public IActionResult Upsert(ProductVM obj)
        {
              var files = HttpContext.Request.Form.Files; // получение нового изображения
              string webRootPath = _webHostEnvironment.WebRootPath; // путь к папке wwwroot

              if (obj.Product.Id == 0) // 
              {
                    string upload = webRootPath + WC.ImagePath; // путь в папку с картинкми
                    string fileName = Guid.NewGuid().ToString(); // генерирование названия для файла
                    string extension = Path.GetExtension(files[0].FileName); // получение расширения файла

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create)) // создание нового файла
                    {
                        files[0].CopyTo(fileStream); // копирование файла
                    }

                    obj.Product.Image = fileName + extension;
                    _prodRepo.Add(obj.Product);
              }
              else
              {
                var objFromDb = _prodRepo.FirstOrDefault(u => u.Id == obj.Product.Id, isTracking: false); // получение сущности для работы с файлом

                if(files.Count > 0)
                {
                    string upload = webRootPath + WC.ImagePath; // путь в папку с картинкми
                    string fileName = Guid.NewGuid().ToString(); // генерирование названия для файла
                    string extension = Path.GetExtension(files[0].FileName); // получение расширения файла

                    var oldfile = Path.Combine(upload, objFromDb.Image); // получение полного пути файла

                    if (System.IO.File.Exists(oldfile)) // проверка существования файла и его удаление
                    {
                        System.IO.File.Delete(oldfile);
                    }

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create)) // создание нового файла
                    {
                        files[0].CopyTo(fileStream); // копирование файла
                    }

                    obj.Product.Image = fileName + extension;
                }
                else
                {
                    obj.Product.Image = objFromDb.Image; // если новое изображение не было загружено
                }
                _prodRepo.Update(obj.Product);
              }
            _prodRepo.Save();
              return RedirectToAction("Index");
        }
    }
}
