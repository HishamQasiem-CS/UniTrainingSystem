using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniTrainingSystem.Data;
using UniTrainingSystem.Models;
using UniTrainingSystem.VIewModels;

namespace UniTrainingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private AppDbContext _db;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager
            ,AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
        }

        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(AddNewStudentViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName=model.FullName,
                };
                var result=await _userManager.CreateAsync(user,model.Password);
                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Login");   
                }
            }
            return View(model);

        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isLogin=await _signInManager.PasswordSignInAsync(model.Email,model.Password,false,false);
                if(isLogin.Succeeded)
                {
                    if(model.Email=="Admin@gmail.com" || model.Email == "Owner@gmail.com") {
                        
                        return RedirectToAction("Index", "Dashboard");

                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                ModelState.AddModelError(string.Empty, "Please Check User / Password");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            
            return RedirectToAction("Login");
        }
    }
}
