// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Admin_Dashboard.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found on ChangePassword GET.");
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                {
                    _logger.LogInformation("User does not have a password set.");
                    return RedirectToPage("./SetPassword");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ChangePassword GET.");
                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is not valid in ChangePassword POST.");
                    return Page();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found on ChangePassword POST.");
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        _logger.LogWarning("Change password failed: {Error}", error.Description);
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("User changed their password successfully.");
                StatusMessage = "Your password has been changed.";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ChangePassword POST.");
                ModelState.AddModelError(string.Empty, "Unexpected error occurred. Please try again.");
                return Page();
            }
        }

    }
}
