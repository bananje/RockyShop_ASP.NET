using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_Utility;
using Rocky_DataAccess;
using Rocky_Models;
using Rocky_Models.ViewModels;

namespace WebApplication3.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly AppDBContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ApplicationTypeController(AppDBContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }  
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> list = _db.ApplicationType;
            return View(list);
        }
        public IActionResult Upsert(int? id)
        {
            if (id != null)
            {
                var obj = _db.ApplicationType.Find(id);

                if (obj == null)
                    return NotFound();

                return View(obj);
            }
            return View(new ApplicationType());
        }

        [HttpPost,ActionName("Upsert")]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertPost(ApplicationType obj)
        {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + WC.ImagePath;

                if (obj.Id == 0)
                {
                    string filename = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var filestream = new FileStream(Path.Combine(upload, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    obj.Image = filename + extension;
                    _db.ApplicationType.Add(obj);
                }
                else
                {
                    var objFromDb = _db.ApplicationType.AsNoTracking().FirstOrDefault(i => i.Id == obj.Id);

                    if(files.Count > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        obj.Image = fileName + extension;
                    }
                    else
                    {
                        obj.Image = objFromDb.Image;
                    }
                    _db.ApplicationType.Update(obj);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");           
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}
