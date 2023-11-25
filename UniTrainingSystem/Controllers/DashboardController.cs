using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using UniTrainingSystem.Data;
using UniTrainingSystem.Models;
using UniTrainingSystem.VIewModels;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace UniTrainingSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private AppDbContext _db;

        public DashboardController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager
            , AppDbContext db)
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
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            ViewBag.name = model.FullName;
            return View(model);

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DisplayAllStudent()
        {
            var user = await _db.Users.ToListAsync();

            return View(user);
        }


        public async Task<IActionResult> DeleteStudent(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("DisplayAllStudent");
            }
            else
            {
                return View();
            }
        }


        [HttpGet]
        public async Task<IActionResult> UpdateStudent(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return View(new EditStudentViewModel
            {
                FullName = user.FullName,
                Email = user.Email,


            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStudent(EditStudentViewModel model, string email)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.Email = model.Email;


                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("DisplayAllStudent");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
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


                return RedirectToAction("Index"); // ارجع إلى الصفحة التي تعرض الشركات
            }
            // إذا كان هناك أخطاء في النموذج، عد إلى صفحة الإضافة مع البيانات
            ViewBag.TrainingTypes = _db.TrainingTypes.ToList();
            return View(model);
        }

        public IActionResult DisplayAllComapny()
        {
            var r = _db.Company.ToList();
            return View(r);
        }
        public IActionResult DeleteCompany(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var pro = _db.Company.Find(id);
            if (pro == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(pro);
        }
        [HttpPost]
        public IActionResult DeleteCompany(Company model)
        {
            _db.Company.Remove(model);
            _db.SaveChanges();
            return RedirectToAction("DisplayAllComapny");

        }





        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList", "Dashboard");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }
        public IActionResult RolesList()
        {
            var myRoles = _roleManager.Roles.OrderByDescending(x => x.Name);

            return View(myRoles);
        }
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return View("NotFoundRole");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name

            };
            //Return all Users
            foreach (var user in _userManager.Users)
            {
                //if user in this role>add UserName prop EditRoleViewModel

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMsg = "$No Role with this ID={model.Id}";
                return View("NotFoundRole");
            }
            else
            {
                role.Name = model.RoleName;
                var updatee = await _roleManager.UpdateAsync(role);
                if (updatee.Succeeded)
                {
                    return RedirectToAction("RolesList");

                }
            }
            return View(model);


        }
        public IActionResult NotFoundRole()
        {
            return View();
        }





        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMsg = "$No Role with this ID={model.Id}";
                return View("NotFoundRole");
            }
            var model = new List<UsersRole>();
            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UsersRole
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.isSelected = true;
                }
                else
                {
                    userRoleViewModel.isSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UsersRole> model, string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMsg = "$No Role with this ID={model.Id}";
                return View("NotFoundRole");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult reseult;
                if (model[i].isSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    reseult = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (model[i].isSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    reseult = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (reseult.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else

                        return RedirectToAction("RolesList");
                }
            }
            return RedirectToAction("RolesList");
        }

        public async Task<IActionResult> DetailsAdmin()
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
        public IActionResult ViewAllTrainingType()
        {
            var r = _db.TrainingTypes.ToList();
            return View(r);
        }
        public IActionResult DeleteTraining(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var pro = _db.TrainingTypes.Find(id);
            if (pro == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(pro);
        }
        [HttpPost]
        public IActionResult DeleteTraining(TrainingType model)
        {
            _db.TrainingTypes.Remove(model);
            _db.SaveChanges();
            return RedirectToAction("ViewAllTrainingType");

        }


        public async Task<IActionResult> UpdateTraining(int? id)
        {
            var user = await _db.TrainingTypes.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new TrainingType
            {
                TypeName = user.TypeName,



            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTraining(TrainingType model, int id)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.TrainingTypes.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.TypeName = model.TypeName;



                _db.TrainingTypes.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("ViewAllTrainingType");
            }

            return View(model);
        }

        public IActionResult AddNewTrainingType()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddNewTrainingType(TrainingType model)
        {
            if (ModelState.IsValid)
            {
                _db.TrainingTypes.Add(model);
                _db.SaveChanges();
                return RedirectToAction("ViewAllTrainingType");

            }
            return View();
        }

        public IActionResult UpdateCompany(int id)
        {
            var company =_db.Company.Include(c => c.CompanyTrainings)
                                              .ThenInclude(ct => ct.TrainingType)
                                              .FirstOrDefault(c => c.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            var viewModel = new EditCompanyViewModel
            {
                
                CompanyName = company.CompanyName,
                Email = company.Email,
                Description = company.Description,
                PhoneNumber = company.PhoneNumber,
                SelectedTrainingTypes = company.CompanyTrainings.Select(ct => ct.TrainingTypeId).ToList()
            };

            // احصل على كل أنواع التدريب لعرضها في الفورم
            ViewBag.TrainingTypes = _db.TrainingTypes.ToList();

            return View(viewModel);
        }

        // عملية التحديث بعد تقديم النموذج
        [HttpPost]
        public IActionResult UpdateCompany(EditCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = _db.Company.Include(c => c.CompanyTrainings).FirstOrDefault(c => c.CompanyId == model.CompanyId);

                if (company == null)
                {
                    return NotFound();
                }

                company.CompanyName = model.CompanyName;
                company.Email=model.Email;
                company.Description = model.Description;    
                company.PhoneNumber = model.PhoneNumber;
                // قم بتحديث أنواع التدريب المرتبطة
                company.CompanyTrainings.Clear();
                foreach (var trainingTypeId in model.SelectedTrainingTypes)
                {
                    company.CompanyTrainings.Add(new CompanyTraining
                    {
                        CompanyId = company.CompanyId,
                        TrainingTypeId = trainingTypeId
                    });
                }

                _db.SaveChanges();

                return RedirectToAction("Index"); // يمكنك توجيهها إلى أي صفحة تريد
            }

            // إعادة عرض الفورم مع الأخطاء إذا كان هناك أخطاء في النموذج
            ViewBag.TrainingTypes = _db.TrainingTypes.ToList();
            return View(model);
        }
    


    public IActionResult ViewContactPage()
        {
            var result = _db.Contacts.ToList();
            return View(result);
        }

        public IActionResult Responsse(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var co = _db.Contacts.Find(id);
            if (co == null)
            {
                return RedirectToAction("Index");
            }

            return View(co);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}

