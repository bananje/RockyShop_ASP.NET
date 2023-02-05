using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Rocky_Models;
using Rocky_DataAccess;
using Rocky_Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Rocky_Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Rocky_DataAccess.Repository.IRepository;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IProductRepository _prodRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HomeController(ILogger<HomeController> logger,IProductRepository prodRepo,ICategoryRepository categoryRepo, IEmailSender emailSender)
        {
            _logger = logger;
            _prodRepo = prodRepo;
            _categoryRepo = categoryRepo;
            _emailSender = emailSender;
        }
         
        public IActionResult Index()
        {

            HomeVM homeVM = new HomeVM()
            {
                Products = _prodRepo.GetAll(includeProperties: "Category,ApplicationType"),
                Categories = _categoryRepo.GetAll()
            };
            return View(homeVM);
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCard> shoppingCartList = new List<ShoppingCard>();
            if (HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart) != null &&
                (HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart).Count() > 0)) // проверка, есть ли в сессии есть элементы
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart); // сессия существует, взять объект и добавить в сессию 
            }

            var itemToRemove = shoppingCartList.SingleOrDefault( r => r.ProductId== id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList); // установка сессии
            return RedirectToAction(nameof(Index)); // перенаправление на метод
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCard> shoppingCartList = new List<ShoppingCard>();
            if (HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart) != null &&
                (HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart).Count() > 0)) // проверка, есть ли в сессии есть элементы
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart); // сессия существует, взять объект и добавить в сессию 
            }

            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = _prodRepo.FirstOrDefault(u => u.Id == id,includeProperties: "Category,ApplicationType"),
                ExistInCart = false
            };

            foreach (var item in shoppingCartList)
            {
                if(item.ProductId == id)
                {
                    DetailsVM.ExistInCart= true;
                }
            }
            return View(DetailsVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCard> shoppingCartList = new List<ShoppingCard>();
            if(HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart) != null && 
                (HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart).Count() > 0)) // проверка, есть ли в сессии есть элементы
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart); // сессия существует, взять объект и добавить в сессию 
            }
            shoppingCartList.Add(new ShoppingCard { ProductId= id }); // добавление элемента в сессию, если блок if обнаружил элемент, то добавляем их здесь, если нет , то добавляем только новый элемент
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList); // установка сессии
            return RedirectToAction(nameof(Index)); // перенаправление на метод
        }
    }
}