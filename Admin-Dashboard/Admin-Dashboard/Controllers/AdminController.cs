using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Admin_Dashboard.ViewModels;
namespace Admin_Dashboard.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> logger;
        private readonly UnitOFWork unitOFWork;
        public AdminController(ILogger<AdminController> logger, UnitOFWork unitOFWork)
        {
            this.logger = logger;
            this.unitOFWork = unitOFWork;
        }

        public IActionResult Index()
        {
            var admins = unitOFWork._adminRopo.getAll();
            return View(admins);
        }



        [HttpGet]
        public IActionResult Edit(string id)
        {
            var admin = unitOFWork._adminRopo.getById(id);

            if(admin == null) 
                return NotFound();

            var viewModel = new AdminViewModel
            {
                Id= admin.Id,
                FName= admin.FName,
                LName= admin.LName,
                Age= admin.Age,
                Email= admin.Email,
                City= admin.City,
                Street= admin.Street,
                Government= admin.Government,       
                Phones= admin.UserPhones.Select(p => p.PhoneNumber).ToList(),
                Salary= admin.Salary,
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(AdminViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var admin = unitOFWork._adminRopo.getById(viewModel.Id);

                if (admin == null)
                    return NotFound();

                admin.FName = viewModel.FName;
                admin.LName = viewModel.LName;
                admin.Age = viewModel.Age;
                admin.Email = viewModel.Email;
                admin.City = viewModel.City;
                admin.Street = viewModel.Street;
                admin.Government = viewModel.Government;
                admin.Salary = viewModel.Salary;

                // Clear old phones and add new ones
                admin.UserPhones.Clear();
                foreach (var phone in viewModel.Phones)
                {
                    admin.UserPhones.Add(new UserPhone
                    {
                        PhoneNumber = phone,
                        UserId = admin.Id
                    });
                }

                unitOFWork._adminRopo.edit(admin);
                

                unitOFWork.save();
                return RedirectToAction("Index");
            }

            logger.LogError(" ModelState is invalid. Errors:");

            foreach (var state in ModelState)
            {
                var key = state.Key;
                foreach (var error in state.Value.Errors)
                {
                    logger.LogError("Field: {Field}, Error: {Error}", key, error.ErrorMessage);
                }
            }

            return View(viewModel);
        }



        [HttpPost]
        public IActionResult Delete(string id)
        {
            var admin = unitOFWork._adminRopo.getById(id);
            if (admin != null)
            {
                admin.IdNavigation.IsDeleted = true;
                unitOFWork._adminRopo.edit(admin);
                unitOFWork.save();
            }
            return RedirectToAction("Index");
        }



    }
}
