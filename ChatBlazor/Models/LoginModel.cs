using System.ComponentModel.DataAnnotations;

namespace ChatBlazor.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
