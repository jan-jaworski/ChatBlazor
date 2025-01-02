//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//using Microsoft.Extensions.Configuration;
//using ChatBlazor.Models;
//using System.Text;

//namespace ChatBlazor.Controllers
//{
    
//    [Route("[controller]/[action]")]
//    [ApiController]
//    public class AccountController : Controller
//    {
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly SignInManager<IdentityUser> _signInManager;
//        private readonly IConfiguration _configuration;

//        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _configuration = configuration;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Register(RegisterModel model)
//        {
//            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
//            var result = await _userManager.CreateAsync(user, model.Password);

//            if (result.Succeeded)
//            {
//                await _signInManager.SignInAsync(user, isPersistent: false);
//                var token = GenerateJwtToken(user);
//                return Ok(new {Token = token});
//            }
//            return BadRequest(result.Errors);
//        }


//        [HttpPost]
//        public async Task<IActionResult> Login(LoginModel model)
//        {
            

//            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,isPersistent: false, lockoutOnFailure: false);

//            if (result.Succeeded)
//            {
//                var user = await _userManager.FindByEmailAsync(model.Email);
//                var token = GenerateJwtToken(user);
//                return Ok(new {Token = token});
//            }
//            return BadRequest("Invalid login attempt.");
//        }


//        [HttpPost]
//        public async Task<IActionResult> Logout()
//        {
//            await _signInManager.SignOutAsync();
//            return Ok();
//        }




//        private string GenerateJwtToken(IdentityUser user)
//        {
//            var claims = new List<Claim>{
//                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//                new Claim(ClaimTypes.NameIdentifier, user.Id)
//            };
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

//            var token = new JwtSecurityToken(
//                _configuration["JwtIssuer"],
//                _configuration["JwtIssuer"],
//                claims,
//                expires: expires,
//                signingCredentials: creds
//            );


//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
