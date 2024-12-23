using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ChatBlazor.Models;

namespace ChatBlazor.Controllers
{
    
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }
            return BadRequest(result.Errors);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest("Invalid login attempt.");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
