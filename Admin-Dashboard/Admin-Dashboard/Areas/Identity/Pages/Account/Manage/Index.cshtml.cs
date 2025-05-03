using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Admin_Dashboard.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FName { get; set; }

            [Display(Name = "Last Name")]
            public string LName { get; set; }

            [Display(Name = "Age")]
            public int? Age { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Street")]
            public string Street { get; set; }

            [Display(Name = "Government")]
            public string Government { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                FName = user.FName,
                LName = user.LName,
                Age = user.Age,
                City = user.City,
                Street = user.Street,
                Government = user.Government,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // تحديث الحقول المخصصة
            if (Input.FName != user.FName)
            {
                user.FName = Input.FName;
            }

            if (Input.LName != user.LName)
            {
                user.LName = Input.LName;
            }

            if (Input.Age != user.Age)
            {
                user.Age = Input.Age;
            }

            if (Input.City != user.City)
            {
                user.City = Input.City;
            }

            if (Input.Street != user.Street)
            {
                user.Street = Input.Street;
            }

            if (Input.Government != user.Government)
            {
                user.Government = Input.Government;
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}