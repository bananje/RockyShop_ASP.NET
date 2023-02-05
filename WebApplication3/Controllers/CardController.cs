using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;
using Rocky_DataAccess;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CardController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IInquiryDetailRepository inqDRepo,
                              IInquiryHeaderRepository inqHRepo,IApplicationUserRepository userRepo, IProductRepository prodRepo)
        {
            _userRepo= userRepo;
            _prodRepo= prodRepo;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _inqDRepo = inqDRepo;
            _inqHRepo = inqHRepo;
        }

        public IActionResult Index()
        {
            List<ShoppingCard> shoppingCards= new List<ShoppingCard>();
            if(HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart) != null &&
               HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart).Count() > 0 )
            {
                // сессия существуют
                shoppingCards = HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCards.Select(u => u.ProductId).ToList();
            List<Product> products = _prodRepo.GetAll(u => prodInCart.Contains(u.Id)).ToList();

            return View(products);
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCard> shoppingCards = new List<ShoppingCard>();
            if (HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart) != null &&
               HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart).Count() > 0)
            {
                // сессия существуют
                shoppingCards = HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart);
            }

            shoppingCards.Remove(shoppingCards.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCards);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {        
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); // получение имени пользователя, если он зашел в систему
            //var userId = User.FindFirstValue(ClaimTypes.Name); // второй способ

            List<ShoppingCard> shoppingCards = new List<ShoppingCard>();
            if (HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart) != null &&
               HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart).Count() > 0)
            {
                // сессия существуют
                shoppingCards = HttpContext.Session.Get<List<ShoppingCard>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCards.Select(u => u.ProductId).ToList();
            List<Product> products = _prodRepo.GetAll(u => prodInCart.Contains(u.Id)).ToList();

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = products
            };

            return View(ProductUserVM);        
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM ProductUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() // путь к htmlbody для сообщения
                + "templates" + Path.DirectorySeparatorChar.ToString() + "EmailMessageBody.html";

            var subject = "New Inquiry";
            string HtmlBody;
            using(StreamReader sr = System.IO.File.OpenText(PathToTemplate)) // открывем файл и записываем в переменную
            {
                HtmlBody = sr.ReadToEnd();
            }

            StringBuilder productListSB = new StringBuilder();
            foreach (var prod in ProductUserVM.ProductList) // перебираем все продукты
            {
                productListSB.Append($"- Name: {prod.Name} <span style='font-size:14px;'> (ID: {prod.Id}) </span> <br />");
            }

            string messageBody = string.Format(HtmlBody, ProductUserVM.ApplicationUser.FullName, // записываем в html документ
                                                         ProductUserVM.ApplicationUser.Email,
                                                         ProductUserVM.ApplicationUser.PhoneNumber,
                                                         productListSB.ToString());

            await _emailSender.SendEmailAsync("vgerman2004@mail.ru", subject, messageBody);

            InquiryHeader inquiryHeader = new InquiryHeader
            {
                ApplicationUserId = claim.Value,
                FullName = ProductUserVM.ApplicationUser.FullName,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
            };

            _inqHRepo.Add(inquiryHeader );
            _inqHRepo.Save();

            foreach (var prod in ProductUserVM.ProductList) // перебираем все продукты
            {
                InquiryDetail inquiryDetail = new InquiryDetail
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = prod.Id
                };
                _inqDRepo.Add(inquiryDetail);
            }
            _inqDRepo.Save(); 

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation(ProductUserVM ProductUserVM)
        {
            HttpContext.Session.Clear();
            return View();
        }
    }
}
