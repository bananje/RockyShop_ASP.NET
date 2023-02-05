using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_Utility;
using System.Data;
using Rocky_DataAccess;
using Rocky_Models;
using Rocky_DataAccess.Repository.IRepository;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepo;

        public CategoryController(ICategoryRepository catRepo)
        {
            _catRepo= catRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> list = _catRepo.GetAll();
            return View(list);
        }

        public IActionResult Create()
        {          
            return View();
        }

        public IActionResult Delete(int id)
        {
            var obj = _catRepo.Find(id);
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
                return NotFound();

            var obj = _catRepo.Find(id.GetValueOrDefault());

            if(obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var obj = _catRepo.Find(id.GetValueOrDefault());

            _catRepo.Remove(obj);
            _catRepo.Save();
            return RedirectToAction("Index");
        }

        [HttpPost] 
        [ValidateAntiForgeryToken] //токен защиты от взлома
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid) // реализация валидации на стороне сервера
            {
                _catRepo.Add(obj);
                _catRepo.Save();
                TempData[WC.Success] = "cruto";
                return RedirectToAction("Index"); // перенапрвление на страницу с категориями
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //токен защиты от взлома
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid) // реализация валидации на стороне сервера
            {
                _catRepo.Update(obj);
                _catRepo.Save();
                return RedirectToAction("Index"); // перенапрвление на страницу с категориями
            }
            return View(obj);
        }       
    }
}
