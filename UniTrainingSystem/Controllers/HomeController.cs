using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniTrainingSystem.Data;
using UniTrainingSystem.Models;
using UniTrainingSystem.VIewModels;

namespace UniTrainingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private AppDbContext _db;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager
            , AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
        }

        public IActionResult Index()
        {
           
            return View();
        }
        public async Task<IActionResult> DeatilsUer()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View(new EditStudentViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeatilsUer(EditStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        public IActionResult Chat()
        {
            return View();
        }
        public IActionResult AddCompany()
        {
            var trainingTypes = _db.TrainingTypes.ToList();
            ViewBag.TrainingTypes = trainingTypes;

            return View();
        }
        [HttpPost]
        public IActionResult AddCompany(AddCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newCompany = new Company
                {
                    CompanyName = model.CompanyName,
                    Email = model.Email,
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,

                };

                _db.Company.Add(newCompany);
                _db.SaveChanges();

                foreach (var trainingTypeId in model.SelectedTrainingTypes)
                {
                    var companyTraining = new CompanyTraining
                    {
                        CompanyId = newCompany.CompanyId,
                        TrainingTypeId = trainingTypeId
                    };
                    var selectedTrainingTypes = Request.Form["SelectedTrainingTypes"].Select(int.Parse).ToList();

                    _db.CompanyTrainings.Add(companyTraining);

                }
                _db.SaveChanges();


                return RedirectToAction("DisplayAllComapny","Dashboard"); // ارجع إلى الصفحة التي تعرض الشركات
            }

            // إذا كان هناك أخطاء في النموذج، عد إلى صفحة الإضافة مع البيانات
            ViewBag.TrainingTypes = _db.TrainingTypes.ToList();
            return View(model);
        }

        public IActionResult ContactPage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ContactPage(Contact model)
        {
            if (ModelState.IsValid) { 
            _db.Contacts.Add(model);
            _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult companys()
        {
            var result = _db.Company.ToList();
            return View(result);
        }

        public IActionResult DetailsCompany(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var product = _db.Company.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            return View(product);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}