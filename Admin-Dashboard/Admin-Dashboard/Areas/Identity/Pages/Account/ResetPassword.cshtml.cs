using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace Admin_Dashboard.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(
            UserManager<AppUser> userManager,
            ILogger<ResetPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null, string email = null)
        {
            if (code == null)
            {
                _logger.LogWarning("Password reset attempted without token");
                return BadRequest("A valid reset token is required.");
            }

            Input = new InputModel
            {
                Code = code,
                Email = email // Pre-populate email if provided
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Reset password model state invalid");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset attempt for non-existent email: {Email}", Input.Email);
                // Don't reveal user doesn't exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            try
            {
                // Decode the token
                var decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(Input.Code));

                // Verify token before attempting reset
                var tokenValid = await _userManager.VerifyUserTokenAsync(
                    user,
                    _userManager.Options.Tokens.PasswordResetTokenProvider,
                    "ResetPassword",
                    decodedToken);

                if (!tokenValid)
                {
                    _logger.LogWarning("Invalid token provided for {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty,
                        "The password reset link is invalid or has expired. Please request a new reset link.");
                    return Page();
                }

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successful for {Email}", Input.Email);
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                // Handle specific errors
                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "InvalidToken":
                            ModelState.AddModelError(string.Empty,
                                "The reset token is invalid. Please request a new password reset.");
                            break;
                        case "PasswordMismatch":
                            ModelState.AddModelError(string.Empty,
                                "The password does not meet the requirements.");
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, error.Description);
                            break;
                    }
                    _logger.LogWarning("Password reset error for {Email}: {Error}", Input.Email, error.Description);
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid token format for {Email}", Input.Email);
                ModelState.AddModelError(string.Empty,
                    "The reset link is malformed. Please request a new password reset.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password reset for {Email}", Input.Email);
                ModelState.AddModelError(string.Empty,
                    "An unexpected error occurred. Please try again.");
            }

            return Page();
        }
    }
}