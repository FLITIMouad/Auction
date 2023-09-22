using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using IdentityService.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public Index(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; }

        [BindProperty]
        public List<string> ExceptionsHandler { get; set;}= new List<string>();
        [BindProperty]
        public bool RegisterSuccess { get; set;}
        public IActionResult OnGet(string returnUrl)
        {
            Input = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };
            return Page();
        }

        public async Task<IActionResult> OnPost(){

            if (Input.Button != "register") return Redirect("~/");

            if(ModelState.IsValid){
                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = Input.Email,
                    EmailConfirmed = true
                };

                 var resut = await _userManager.CreateAsync(user,Input.Password);

                if (resut.Succeeded)
                    {
                        await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Name, Input.FullName));
                    RegisterSuccess = true;
                    }
                    else{
                   
                        foreach (var item in resut.Errors)
                        {
                        ExceptionsHandler.Add(item.Description);
                        }
                    }
            }

            return Page();
        }
    }
}
