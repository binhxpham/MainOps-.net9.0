using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MainOps.Models;
using MainOps.Models.ManageViewModels;
using MainOps.Services;
using MainOps.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MainOps.ExtensionMethods;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,ProjectMember,Member,International,Guest")]
    [Route("[controller]/[action]")]
    public class ManageController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(
            DataContext context,
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _environment = env;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage,
                photopath = user.PicturePath,
                UserLog = user.UserLog
                
            };
            
           
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }
            var firstname = user.FirstName;
            if (model.FirstName != firstname)
            {
                if(model.FirstName == null)
                {
                    user.FirstName = "";
                }
                user.FirstName = model.FirstName;
                _context.Update(user);
            }
            var lastname = user.LastName;
            if (model.LastName != lastname)
            {
                if (model.LastName == null)
                {
                    user.LastName = "";
                }
                user.LastName = model.LastName;
                _context.Update(user);
            }


            await _context.SaveChangesAsync();
            StatusMessage = "Your profile has been updated";
            
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLog(string UserLog)
        {
            
            var user = await _userManager.GetUserAsync(User);
            user.UserLog = UserLog;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' because they do not have 2FA enabled.");
            }

            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            return View(nameof(ShowRecoveryCodes), model);
        }

        
       
        

        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return View("deleteConfirmed");
        }

        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpPost]
        public async Task<IActionResult> RemoveUser(string id)
        {
            
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> PromoteAdmin(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteDivisionAdmin(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "DivisionAdmin");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveDivisionAdmin(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "DivisionAdmin");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> RemoveMember(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "Member");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> PromoteMember(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "Member");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> MakeGuest(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "Guest");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> RemoveGuest(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "Guest");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> MakeMemberGuest(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "MemberGuest");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> RemoveMemberGuest(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "MemberGuest");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeStorageManager(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "StorageManager");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveStorageManager(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "StorageManager");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeProjectMember(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "ProjectMember");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveProjectMember(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "ProjectMember");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeManager(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "Manager");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveManager(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "Manager");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeInternational(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "International");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveInternational(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "International");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeSupervisor(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.AddToRoleAsync(user, "Supervisor");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveSupervisor(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "Supervisor");
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        public async Task<IActionResult> UpdateUserRights(string id, bool Member,bool Manager, bool ProjectMember, bool Guest, bool DivisionAdmin, bool Admin, bool MemberGuest, bool StorageManager,bool Supervisor, bool International, bool AlarmTeam, bool ExternalDriller, bool Board)
        {
            var theuser = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByIdAsync(id);
            var rolenames = await (from userRole in _context.UserRoles
                             join role in _context.Roles on userRole.RoleId
                             equals role.Id
                             where userRole.UserId == user.Id
                             select role.Name).ToListAsync();
            if (Member == false && rolenames.IndexOf("Member") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "Member");
            }
            else if(Member == true && rolenames.IndexOf("Member") == -1)
            {
                await _userManager.AddToRoleAsync(user, "Member");
            }
            if (ExternalDriller == false && rolenames.IndexOf("ExternalDriller") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "ExternalDriller");
            }
            else if(ExternalDriller == true && rolenames.IndexOf("ExternalDriller") == -1)
            {
                await _userManager.AddToRoleAsync(user, "ExternalDriller");
            }
            if(ProjectMember == false && rolenames.IndexOf("ProjectMember") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "ProjectMember");
            }
            else if(ProjectMember == true && rolenames.IndexOf("ProjectMember") == -1)
            {
                await _userManager.AddToRoleAsync(user, "ProjectMember");
            }
            if (Guest == false && rolenames.IndexOf("Guest") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "Guest");
            }
            else if (Guest == true && rolenames.IndexOf("Guest") == -1)
            {
                await _userManager.AddToRoleAsync(user, "Guest");
            }
            if (MemberGuest == false && rolenames.IndexOf("MemberGuest") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "MemberGuest");
            }
            else if (MemberGuest == true && rolenames.IndexOf("MemberGuest") == -1)
            {
                await _userManager.AddToRoleAsync(user, "MemberGuest");
            }
            if (AlarmTeam == false && rolenames.IndexOf("AlarmTeam") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "AlarmTeam");
            }
            else if (AlarmTeam == true && rolenames.IndexOf("AlarmTeam") == -1)
            {
                await _userManager.AddToRoleAsync(user, "AlarmTeam");
            }
            if (User.IsInRole("Admin")) { 
                if (Admin == false && rolenames.IndexOf("Admin") >= 0)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                else if (Admin == true && rolenames.IndexOf("Admin") == -1)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            if(User.IsInRole("Admin") || User.IsInRole("DivisionAdmin")) { 
                if (DivisionAdmin == false && rolenames.IndexOf("DivisionAdmin") >= 0)
                {
                    await _userManager.RemoveFromRoleAsync(user, "DivisionAdmin");
                }
                else if (DivisionAdmin == true && rolenames.IndexOf("DivisionAdmin") == -1)
                {
                    await _userManager.AddToRoleAsync(user, "DivisionAdmin");
                }
            }
            if (StorageManager == false && rolenames.IndexOf("StorageManager") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "StorageManager");
            }
            else if (StorageManager == true && rolenames.IndexOf("StorageManager") == -1)
            {
                await _userManager.AddToRoleAsync(user, "StorageManager");
            }
            if (Supervisor == false && rolenames.IndexOf("Supervisor") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "Supervisor");
            }
            else if (Supervisor == true && rolenames.IndexOf("Supervisor") == -1)
            {
                await _userManager.AddToRoleAsync(user, "Supervisor");
            }
            if (Manager == false && rolenames.IndexOf("Manager") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "Manager");
            }
            else if (Manager == true && rolenames.IndexOf("Manager") == -1)
            {
                await _userManager.AddToRoleAsync(user, "Manager");
            }
            if (International == false && rolenames.IndexOf("International") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "International");
            }
            else if (International == true && rolenames.IndexOf("International") == -1)
            {
                await _userManager.AddToRoleAsync(user, "International");
            }
            if (Board == false && rolenames.IndexOf("Board") >= 0)
            {
                await _userManager.RemoveFromRoleAsync(user, "Board");
            }
            else if (Board == true && rolenames.IndexOf("Board") == -1)
            {
                await _userManager.AddToRoleAsync(user, "Board");
            }
            return RedirectToAction("UsersWithRoles");

        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> UsersWithRoles()
        {
            IEnumerable<ApplicationUserListViewModel> usersWithRoles;
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin")) { 
            usersWithRoles = (from user in _context.Users.Where(x=>x.Active.Equals(true))
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Active = user.Active,
                                      MembershipConfirmed = user.MemberShipConfirmed,
                                      EmailConfirmed = user.EmailConfirmed,
                                      RoleNames =  (from userRole in _context.UserRoles
                                                   join role in _context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   where userRole.UserId == user.Id
                                                   select role.Name).ToList()
                                  }).ToList()
                                  .Select(p => new ApplicationUserListViewModel()
                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      FirstName = p.FirstName,
                                      LastName = p.LastName,
                                      Active = p.Active,
                                      Email = p.Email,
                                      MemberShipConfirmed = p.MembershipConfirmed,
                                      EmailConfirmed = p.EmailConfirmed,
                                      Role = string.Join(",", p.RoleNames),


                                  }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            }
            else
            {
                
                usersWithRoles = (from user in _context.Users.Where(x=>x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(true))
                                      select new
                                      {
                                          UserId = user.Id,
                                          Username = user.UserName,
                                          Email = user.Email,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          Active = user.Active,
                                          MembershipConfirmed = user.MemberShipConfirmed,
                                          EmailConfirmed = user.EmailConfirmed,
                                          RoleNames = (from userRole in _context.UserRoles
                                                       join role in _context.Roles on userRole.RoleId
                                                       equals role.Id
                                                       where userRole.UserId == user.Id
                                                       select role.Name).ToList()
                                      }).ToList()
                                  .Select(p => new ApplicationUserListViewModel()
                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      FirstName = p.FirstName,
                                      LastName = p.LastName,
                                      Email = p.Email,
                                      Active = p.Active,
                                      MemberShipConfirmed = p.MembershipConfirmed,
                                      EmailConfirmed = p.EmailConfirmed,
                                      Role = string.Join(",", p.RoleNames),


                                  }).OrderBy(x=>x.LastName).ThenBy(x=>x.FirstName);
            }
            
            ViewData["ProjectId"] = new SelectList(_context.Projects.Include(x=>x.Division).Where(x=>x.Division.Id.Equals(theuser.DivisionId)), "Id", "Name");
            return View(usersWithRoles);
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> UsersWithRolesInactive()
        {
            IEnumerable<ApplicationUserListViewModel> usersWithRoles;
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                usersWithRoles = (from user in _context.Users.Where(x => x.Active.Equals(false))
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Active = user.Active,
                                      MembershipConfirmed = user.MemberShipConfirmed,
                                      EmailConfirmed = user.EmailConfirmed,
                                      RoleNames = (from userRole in _context.UserRoles
                                                   join role in _context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   where userRole.UserId == user.Id
                                                   select role.Name).ToList()
                                  }).ToList()
                                      .Select(p => new ApplicationUserListViewModel()
                                      {
                                          UserId = p.UserId,
                                          Username = p.Username,
                                          FirstName = p.FirstName,
                                          LastName = p.LastName,
                                          Active = p.Active,
                                          Email = p.Email,
                                          MemberShipConfirmed = p.MembershipConfirmed,
                                          EmailConfirmed = p.EmailConfirmed,
                                          Role = string.Join(",", p.RoleNames),


                                      }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            }
            else
            {

                usersWithRoles = (from user in _context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(false))
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Active = user.Active,
                                      MembershipConfirmed = user.MemberShipConfirmed,
                                      EmailConfirmed = user.EmailConfirmed,
                                      RoleNames = (from userRole in _context.UserRoles
                                                   join role in _context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   where userRole.UserId == user.Id
                                                   select role.Name).ToList()
                                  }).ToList()
                                  .Select(p => new ApplicationUserListViewModel()
                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      FirstName = p.FirstName,
                                      LastName = p.LastName,
                                      Email = p.Email,
                                      Active = p.Active,
                                      MemberShipConfirmed = p.MembershipConfirmed,
                                      EmailConfirmed = p.EmailConfirmed,
                                      Role = string.Join(",", p.RoleNames),


                                  }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "Name");
            return View("UsersWithRoles",usersWithRoles);
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpPost]
        public async Task<bool> SetInactive(string theId)
        {
            if(theId != null && theId != "")
            { 
                var user = await _userManager.FindByEmailAsync(theId);
                user.Active = false;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            else{
                return false;
            }
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpPost]
        public async Task<bool> SetActive(string theId)
        {
            if (theId != null && theId != "")
            {
                var user = await _userManager.FindByEmailAsync(theId);
                user.Active = true;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpPost]
        public async Task<bool> ConfirmMemberShip(string theId)
        {
            if (theId != null && theId != "")
            {
                var user = await _userManager.FindByEmailAsync(theId);
                await _userManager.AddToRoleAsync(user, "Member");
                user.MemberShipConfirmed = true;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpPost]
        public async Task<bool> ConfirmEmail(string theId)
        {
            if (theId != null && theId != "")
            {
                var user = await _userManager.FindByEmailAsync(theId);
                user.EmailConfirmed = true;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadPhoto()
        {
            var user = await _userManager.GetUserAsync(User);
            var folderpath = Path.Combine(_environment.WebRootPath.ReplaceFirst("/", ""), "Images", "People");
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            if (HttpContext.Request.Form.Files != null)
            {
                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        //Getting FileName
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        fileName = user.FirstName + user.LastName + fileName; 
                        // Combines two strings into a path.
                        fileName = folderpath + $@"\{fileName}";
                        user.PicturePath = fileName;
                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                        _context.Update(user);
                        await _context.SaveChangesAsync();

                    }
                }
            }
            return RedirectToAction("Index");
        }
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("identityapp"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        #endregion
    }
}
