using Microsoft.AspNetCore.Identity;

namespace ChatBlazor.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        
        //relations 
        public string User1Id { get; set; }
        public IdentityUser User1 { get; set; }

        public string User2Id { get; set; }
        public IdentityUser User2 { get; set; }

        //messages nav
        public ICollection<Message> Messages { get; set; }
    }
}
