using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

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
            var user = admin.IdNavigation;

            var viewModel = new AdminUserViewModel
            {
                Admin = admin,
                User = user
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(AdminUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Admin.Id = viewModel.User.Id;

                unitOFWork._adminRopo.edit(viewModel.Admin);
                unitOFWork._customerRepo.db.Users.Update(viewModel.User);

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
