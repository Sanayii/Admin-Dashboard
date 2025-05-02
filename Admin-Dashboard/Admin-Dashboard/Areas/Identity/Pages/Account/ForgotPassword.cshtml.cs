// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Admin_Dashboard.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(
            UserManager<AppUser> userManager,
            IEmailSender emailSender,
            ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [Display(Name = "Email Address")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for password reset");
                return Page();
            }

            _logger.LogInformation("Password reset initiated for {Email}", Input.Email);

            try
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                // Security: Don't reveal if user doesn't exist or email isn't confirmed
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogWarning("Password reset request for non-existent or unconfirmed email: {Email}", Input.Email);
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // In ForgotPassword.cshtml.cs
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code = token, userId = user.Id }, // Include userId
                    protocol: Request.Scheme);

                _logger.LogDebug("Generated password reset link: {Url}", callbackUrl);

                await SendPasswordResetEmail(Input.Email, callbackUrl);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing password reset for {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return Page();
            }
        }

        private async Task SendPasswordResetEmail(string email, string resetLink)
        {
            try
            {
                var subject = "Password Reset Request";
                var htmlMessage = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; padding: 20px;'>
                    <h2 style='color: #4e73df;'>Password Reset</h2>
                    <p>You requested to reset your password. Click the button below to proceed:</p>
                    <a href='{HtmlEncoder.Default.Encode(resetLink)}' 
                       style='background-color: #4e73df; color: white; padding: 12px 24px; 
                              text-decoration: none; border-radius: 5px; display: inline-block; 
                              margin: 15px 0; font-weight: bold;'>
                       Reset Password
                    </a>
                    <p style='color: #6c757d; font-size: 0.9em;'>
                        If you didn't request this, please ignore this email. The link will expire in 2 hours.
                    </p>
                </div>";

                await _emailSender.SendEmailAsync(email, subject, htmlMessage);
                _logger.LogInformation("Password reset email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw; // Re-throw to be caught by the outer try-catch
            }
        }
    }
}